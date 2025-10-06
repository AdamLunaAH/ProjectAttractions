using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

using Models;
using Models.DTO;
using Services;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AttractionsController : Controller
    {
        readonly IAttractionsService _service = null;
        readonly ILogger<AttractionsController> _logger = null;

        [HttpGet()]
        [ActionName("ReadEverything")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttractions>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadEverything(string seeded = "both", string flat = "true",
            string filter = null, string pageNr = "0", string pageSize = "10")
        {
            try
            {
                bool? seededArg = seeded.ToLower().Trim() switch
                {
                    "true" => true,
                    "false" => false,
                    "both" => null,
                    "null" => null,
                    _ => throw new ArgumentException("Invalid seeded value")
                };
                bool flatArg = bool.Parse(flat);
                int pageNrArg = int.Parse(pageNr);
                int pageSizeArg = int.Parse(pageSize);

                _logger.LogInformation($"{nameof(ReadEverything)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                    $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");

                var resp = await _service.ReadAttractionsAsync(seededArg, flatArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadEverything)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet()]
        [ActionName("Attraction with no address")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttractions>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> AttractionNoAddress(string seeded = "both", string flat = "true",
            string filter = null, string pageNr = "0", string pageSize = "10", string noAddress = "true")
        {
            try
            {
                bool? seededArg = seeded.ToLower().Trim() switch
                {
                    "true" => true,
                    "false" => false,
                    "both" => null,
                    "null" => null,
                    _ => throw new ArgumentException("Invalid seeded value")
                };
                bool flatArg = bool.Parse(flat);
                int pageNrArg = int.Parse(pageNr);
                int pageSizeArg = int.Parse(pageSize);
                bool noAddressArg = bool.Parse(noAddress);

                _logger.LogInformation($"{nameof(AttractionNoAddress)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                    $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}, {nameof(noAddressArg)}");

                var resp = await _service.ReadAttractionsNoAddressAsync(seededArg, flatArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg, noAddressArg);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(AttractionNoAddress)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet()]
        [ActionName("Attraction with no reviews")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttractions>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> AttractionNoReviews(string seeded = "both", string flat = "true",
    string filter = null, string pageNr = "0", string pageSize = "10", string noReview = "true")
        {
            try
            {
                bool? seededArg = seeded.ToLower().Trim() switch
                {
                    "true" => true,
                    "false" => false,
                    "both" => null,
                    "null" => null,
                    _ => throw new ArgumentException("Invalid seeded value")
                };
                bool flatArg = bool.Parse(flat);
                int pageNrArg = int.Parse(pageNr);
                int pageSizeArg = int.Parse(pageSize);
                bool noReviewArg = bool.Parse(noReview);

                _logger.LogInformation($"{nameof(AttractionNoReviews)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                    $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}, {nameof(noReviewArg)}");

                var resp = await _service.ReadAttractionsNoReviewsAsync(seededArg, flatArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg, noReviewArg);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(AttractionNoReviews)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/attractions/readitem
        [HttpGet()]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(IAttractions))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(string id = null, string flat = "false")
        {
            try
            {
                var checkGuid = Guid.TryParse(id, out var guid);
                if (!checkGuid)
                {
                    return BadRequest(new ResponseItemDto<AttractionsCuDto>
                    {
                        ErrorMessage = "Input id is not a Guid"
                    }
                    );
                }
                var idArg = guid;
                bool flatArg = bool.Parse(flat);

                _logger.LogInformation($"{nameof(ReadItem)}: {nameof(idArg)}: {idArg}, {nameof(flatArg)}: {flatArg}");

                var item = await _service.ReadAttractionAsync(idArg, flatArg);
                if (item == null) throw new ArgumentException($"Item with id {id} does not exist");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadItem)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //DELETE: api/attractions/deleteitem/id
        [HttpDelete("{id}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(IAttractions))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(string id)
        {
            try
            {
                var checkGuid = Guid.TryParse(id, out var guid);
                if (!checkGuid)
                {
                    return BadRequest(new ResponseItemDto<AttractionsCuDto>
                    {
                        ErrorMessage = "Input id is not a Guid"
                    }
                    );
                }
                var idArg = guid;

                _logger.LogInformation($"{nameof(DeleteItem)}: {nameof(idArg)}: {idArg}");

                var item = await _service.DeleteAttractionAsync(idArg);
                if (item == null) throw new ArgumentException($"Item with id {id} does not exist");

                _logger.LogInformation($"item {idArg} deleted");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteItem)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/attractions/readitemdto
        [HttpGet()]
        [ActionName("ReadItemDto")]
        [ProducesResponseType(200, Type = typeof(AttractionsCuDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        public async Task<IActionResult> ReadItemDto(string id = null)
        {
            try
            {
                var checkGuid = Guid.TryParse(id, out var guid);
                if (!checkGuid)
                {
                    return BadRequest(new ResponseItemDto<AttractionsCuDto>
                    {
                        ErrorMessage = "Input id is not a Guid"
                    }
                    );
                }
                var idArg = guid;

                _logger.LogInformation($"{nameof(ReadItemDto)}: {nameof(idArg)}: {idArg}");

                var item = await _service.ReadAttractionAsync(idArg, false);
                if (item == null) throw new ArgumentException($"Item with id {id} does not exist");

                return Ok(
                    new ResponseItemDto<AttractionsCuDto>()
                    {
#if DEBUG
                        ConnectionString = item.ConnectionString,
#endif
                        Item = item.Item == null ? null : new AttractionsCuDto(item.Item),
                        ErrorMessage = item.ErrorMessage
                    });

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadItemDto)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //PUT: api/attractions/updateitem/id
        //Body: AddressCUdto in Json
        [HttpPut("{id}")]
        [ActionName("UpdateItem")]
        [ProducesResponseType(200, Type = typeof(IAttractions))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] AttractionsCuDto item)
        {
            try
            {
                var checkGuid = Guid.TryParse(id, out var guid);
                if (!checkGuid)
                {
                    return BadRequest(new ResponseItemDto<AttractionsCuDto>
                    {
                        ErrorMessage = "Input id is not a Guid"
                    }
                    );
                }
                var idArg = guid;

                _logger.LogInformation($"{nameof(UpdateItem)}: {nameof(idArg)}: {idArg}");

                // if (item.AttractionId != idArg) throw new ArgumentException("Id mismatch");
                if (item.AttractionId != idArg)
                {
                    return BadRequest(new ResponseItemDto<AttractionsCuDto>
                    {
                        ErrorMessage = $"Input id {idArg} does not match item id {item.AttractionId}"
                    }
                    );
                }

                var model = await _service.UpdateAttractionAsync(item);
                _logger.LogInformation($"item {idArg} updated");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(UpdateItem)}: {ex.Message}");
                return BadRequest($"Could not update. Error {ex.Message}");
            }
        }

        //POST: api/attractions/createitem
        //Body: csAddressCUdto in Json
        [HttpPost()]
        [ActionName("CreateItem")]
        [ProducesResponseType(200, Type = typeof(IAttractions))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateItem([FromBody] AttractionCreateDto item)
        {
            try
            {

                var model = await _service.CreateAttractionAsync(item);
                _logger.LogInformation($"{nameof(CreateItem)}:");
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CreateItem)}: {ex.Message}");
                return BadRequest($"Could not create. Error {ex.Message}");
            }
        }

        // POST: api/attractions/createfullattraction
        // Body: AttractionFullCreateDto in Json
        [HttpPost()]
        [ActionName("CreateFullAttraction")]
        [ProducesResponseType(200, Type = typeof(IAttractions))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateFullAttraction([FromBody] AttractionFullCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating full attraction with address + categories");

                var model = await _service.CreateFullAttractionAsync(dto);

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateFullAttraction: {ex.Message}");
                return BadRequest($"Could not create. Error: {ex.Message}");
            }
        }



        public AttractionsController(IAttractionsService service, ILogger<AttractionsController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}

