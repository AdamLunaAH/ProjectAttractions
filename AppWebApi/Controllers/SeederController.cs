using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

using Services;
using Configuration;
using Configuration.Options;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SeederController : Controller
    {
        readonly DatabaseConnections _dbConnections;
        readonly ISeederService _service;
        readonly ILogger<SeederController> _logger;
        readonly VersionOptions _versionOptions;


        //GET: api/seeder/seed?count={count}
        [HttpGet()]
        [ActionName("SeedAll")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> SeedAll()
        {
            try
            {

                _logger.LogInformation($"{nameof(SeedAll)}");
                await _service.SeedAllAsync();

                return Ok("Seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SeedAll)}: {ex.Message}");
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


        public SeederController(ISeederService service, ILogger<SeederController> logger,
                DatabaseConnections dbConnections, IOptions<VersionOptions> versionOptions)
        {
            _service = service;
            _logger = logger;
            _dbConnections = dbConnections;
            _versionOptions = versionOptions.Value;
        }
    }
}

