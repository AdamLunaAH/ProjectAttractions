using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Models.DTO;

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
        [HttpPost()]
        [ActionName("SeedAll")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<SupUsrInfoAllDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> SeedAll()

        {
            try
            {

                _logger.LogInformation($"{nameof(SeedAll)}");
                var result = await _service.SeedAllAsync();
                return Ok(result);

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

        [HttpDelete()]
        [ActionName("RemoveSeededData")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<SupUsrInfoAllDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> RemoveSeededData()
        {
            try
            {
                _logger.LogInformation($"{nameof(RemoveSeededData)}");
                var result = await _service.RemoveSeededDataAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RemoveSeededData)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete()]
        [ActionName("RemoveAllData")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<SupUsrInfoAllDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> RemoveAllData()
        {
            try
            {
                _logger.LogInformation($"{nameof(RemoveAllData)}");
                var result = await _service.RemoveAllDataAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RemoveAllData)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
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

