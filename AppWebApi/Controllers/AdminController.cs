using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Services;
using Configuration;
using Configuration.Options;
using Microsoft.Extensions.Options;
using DbContext;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : Controller
    {
        readonly DatabaseConnections _dbConnections;
        readonly IAdminService _service;
        readonly ILogger<AdminController> _logger;
        readonly VersionOptions _versionOptions;
        private readonly MainDbContext _dbContext;

        //GET: api/admin/environment
        [HttpGet()]
        [ActionName("Environment")]
        [ProducesResponseType(200, Type = typeof(DatabaseConnections.SetupInformation))]
        public IActionResult Environment()
        {
            try
            {
                var info = _dbConnections.SetupInfo;

                _logger.LogInformation($"{nameof(Environment)}:\n{JsonConvert.SerializeObject(info)}");
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Environment)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet()]
        [ActionName("Version")]
        [ProducesResponseType(typeof(VersionOptions), 200)]
        public IActionResult Version()
        {
            try
            {
                _logger.LogInformation($"{nameof(Version)}:\n{JsonConvert.SerializeObject(_versionOptions)}");
                return Ok(_versionOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving version information");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/admin/log
        [HttpGet()]
        [ActionName("Log")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LogMessage>))]
        public async Task<IActionResult> Log([FromServices] ILoggerProvider _loggerProvider)
        {
            //Note the way to get the LoggerProvider, not the logger from Services via DI
            if (_loggerProvider is InMemoryLoggerProvider cl)
            {
                return Ok(await cl.MessagesAsync);
            }
            return Ok("No messages in log");
        }

        [HttpGet]
        [ActionName("GenerateInitSql")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GenerateInitSql()
        {
            try
            {
                // 1️⃣ Generate SQL from EF Core model
                var sql = _dbContext.Database.GenerateCreateScript();

                // 2️⃣ Build static relative path to the target folder
                // assuming solution structure:
                //   AppWebApi/
                //   DbContext/SqlScripts/sqlserver/
                // var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                // var targetFolder = Path.Combine(projectRoot, "DbContext", "SqlScripts", "sqlserver");
                // var rootPath = AppContext.BaseDirectory; // runtime root of your app
                var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

                var targetFolder = Path.Combine(projectRoot, "DbContext", "SqlScripts", "sqlserver");
                Directory.CreateDirectory(targetFolder); // ensure folder exists

                // ensure the folder exists
                Directory.CreateDirectory(targetFolder);

                // 3️⃣ Create a timestamped filename
                var fileName = $"initDatabase_{DateTime.UtcNow:yyyyMMdd_HHmmss}.sql";
                var filePath = Path.Combine(targetFolder, fileName);

                // 4️⃣ Write SQL to file
                await System.IO.File.WriteAllTextAsync(filePath, sql);

                // 5️⃣ Log the result
                _logger.LogInformation($"Init SQL script generated at: {filePath}");

                // return simple confirmation
                return Ok(new
                {
                    message = "Database SQL script successfully generated.",
                    path = filePath
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating init SQL script");
                return BadRequest(new { message = ex.Message });
            }
        }




        public AdminController(IAdminService service, ILogger<AdminController> logger,
                DatabaseConnections dbConnections, IOptions<VersionOptions> versionOptions, MainDbContext dbContext)
        {
            _service = service;
            _logger = logger;
            _dbConnections = dbConnections;
            _versionOptions = versionOptions.Value;
            _dbContext = dbContext;
        }
    }
}

