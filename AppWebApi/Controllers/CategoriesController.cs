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
    public class CategoriesController : Controller
    {
        readonly ICategoriesService _service = null;
        readonly ILogger<CategoriesController> _logger = null;

        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttractions>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(string seeded = "both", string flat = "true",
            string filter = null, string pageNr = "0", string pageSize = "10")
        {
            try
            {
                bool? seededArg = seeded.ToLower() switch
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

                _logger.LogInformation($"{nameof(Read)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                    $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");

                var resp = await _service.ReadCategoriesAsync(seededArg, flatArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Read)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/categories/readitem
        [HttpGet()]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(ICategories))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(string id = null, string flat = "false")
        {
            try
            {
                var checkGuid = Guid.TryParse(id, out var guid);
                if (!checkGuid)
                {
                    return BadRequest(new ResponseItemDto<CategoriesCuDto>
                    {
                        ErrorMessage = "Input id is not a Guid"
                    }
                    );
                }
                var idArg = guid;
                bool flatArg = bool.Parse(flat);

                _logger.LogInformation($"{nameof(ReadItem)}: {nameof(idArg)}: {idArg}, {nameof(flatArg)}: {flatArg}");

                var item = await _service.ReadCategoryAsync(idArg, flatArg);
                if (item == null) throw new ArgumentException($"Item with id {id} does not exist");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadItem)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //DELETE: api/categories/deleteitem/id
        [HttpDelete("{id}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(ICategories))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(string id)
        {
            try
            {
                var checkGuid = Guid.TryParse(id, out var guid);
                if (!checkGuid)
                {
                    return BadRequest(new ResponseItemDto<CategoriesCuDto>
                    {
                        ErrorMessage = "Input id is not a Guid"
                    }
                    );
                }
                var idArg = guid;

                _logger.LogInformation($"{nameof(DeleteItem)}: {nameof(idArg)}: {idArg}");

                var item = await _service.DeleteCategoryAsync(idArg);
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

        //GET: api/categories/readitemdto
        [HttpGet()]
        [ActionName("ReadItemDto")]
        [ProducesResponseType(200, Type = typeof(CategoriesCuDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(404, Type = typeof(string))]
        public async Task<IActionResult> ReadItemDto(string id = null)
        {
            try
            {
                var checkGuid = Guid.TryParse(id, out var guid);
                if (!checkGuid)
                {
                    return BadRequest(new ResponseItemDto<CategoriesCuDto>
                    {
                        ErrorMessage = "Input id is not a Guid"
                    }
                    );
                }
                var idArg = guid;

                _logger.LogInformation($"{nameof(ReadItemDto)}: {nameof(idArg)}: {idArg}");

                var item = await _service.ReadCategoryAsync(idArg, false);
                if (item == null) throw new ArgumentException($"Item with id {id} does not exist");

                return Ok(
                    new ResponseItemDto<CategoriesCuDto>()
                    {
#if DEBUG
                        ConnectionString = item.ConnectionString,
#endif
                        Item = new CategoriesCuDto(item.Item)
                    });

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadItemDto)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //PUT: api/categories/updateitem/id
        //Body: AddressCUdto in Json
        [HttpPut("{id}")]
        [ActionName("UpdateItem")]
        [ProducesResponseType(200, Type = typeof(ICategories))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] CategoriesCuDto item)
        {
            try
            {
                var checkGuid = Guid.TryParse(id, out var guid);
                if (!checkGuid)
                {
                    return BadRequest(new ResponseItemDto<CategoriesCuDto>
                    {
                        ErrorMessage = "Input id is not a Guid"
                    }
                    );
                }
                var idArg = guid;

                _logger.LogInformation($"{nameof(UpdateItem)}: {nameof(idArg)}: {idArg}");

                // if (item.CategoryId != idArg) throw new ArgumentException("Id mismatch");
                    if (item.CategoryId != idArg)
                    {
                        return BadRequest(new ResponseItemDto<CategoriesCuDto>
                        {
                            ErrorMessage = $"Input id {idArg} does not match item id {item.CategoryId}"
                        }
                        );
                    }

                var model = await _service.UpdateCategoryAsync(item);
                _logger.LogInformation($"item {idArg} updated");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(UpdateItem)}: {ex.Message}");
                return BadRequest($"Could not update. Error {ex.Message}");
            }
        }

        //POST: api/categories/createitem
        //Body: csAddressCUdto in Json
        [HttpPost()]
        [ActionName("CreateItem")]
        [ProducesResponseType(200, Type = typeof(ICategories))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateItem([FromBody] CategoryCreateDto item)
        {
            try
            {


                var model = await _service.CreateCategoryAsync(item);
                _logger.LogInformation($"{nameof(CreateItem)}:");


                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CreateItem)}: {ex.Message}");
                return BadRequest($"Could not create. Error {ex.Message}");
            }
        }

        public CategoriesController(ICategoriesService service, ILogger<CategoriesController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}

