using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class CategoriesDbRepos
{
    private ILogger<CategoriesDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CategoriesDbRepos(ILogger<CategoriesDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<ICategories>> ReadCategoriesAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        try
        {
            filter ??= "";
            IQueryable<CategoriesDbM> query;
            if (flat)
            {
                query = _dbContext.Categories.AsNoTracking();
            }
            else
            {
                query = _dbContext.Categories.AsNoTracking()
                    .Include(i => i.AttractionsDbM);
            }

            var ret = new ResponsePageDto<ICategories>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif
                DbItemsCount = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                            i.CategoryName.ToLower().Contains(filter))
                            .CountAsync(),

                PageItems = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                            i.CategoryName.ToLower().Contains(filter))

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .ToListAsync<ICategories>(),

                PageNr = pageNumber,
                PageSize = pageSize
            };
            return ret;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read categories due to an unexpected error.");
            return new ResponsePageDto<ICategories>();
        }
    }

    public async Task<ResponseItemDto<ICategories>> ReadCategoryAsync(Guid id, bool flat)
    {
        try
        {
            if (!flat)
            {
                //make sure the model is fully populated, try without include.
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var query = _dbContext.Categories.AsNoTracking()
                    .Include(i => i.AttractionsDbM)
                    .Where(i => i.CategoryId == id);

                return new ResponseItemDto<ICategories>()
                {
#if DEBUG
                    ConnectionString = _dbContext.dbConnection,
#endif

                    Item = await query.FirstOrDefaultAsync<ICategories>()
                };
            }
            else
            {
                //Not fully populated, compare the SQL Statements generated
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var query = _dbContext.Categories.AsNoTracking()
                    .Where(i => i.CategoryId == id);

                return new ResponseItemDto<ICategories>()
                {
#if DEBUG
                    ConnectionString = _dbContext.dbConnection,
#endif

                    Item = await query.FirstOrDefaultAsync<ICategories>()
                };
            }
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<ICategories>()
            {
                ErrorMessage = $"Could not read category due to an unexpected error: {ex.Message}"
            };
        }
    }


    public async Task<ResponseItemDto<ICategories>> DeleteCategoryAsync(Guid id)
    {
        try
        {
            var query1 = _dbContext.Categories
            .Where(i => i.CategoryId == id);

            var item = await query1.FirstOrDefaultAsync<CategoriesDbM>();

            //If the item does not exists
            if (item == null) throw new ArgumentException($"Item {id} is not existing");

            //delete in the database model
            _dbContext.Categories.Remove(item);

            //write to database in a UoW
            await _dbContext.SaveChangesAsync();
            return new ResponseItemDto<ICategories>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = item
            };
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<ICategories>()
            {
                ErrorMessage = $"Could not delete category due to an unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<ResponseItemDto<ICategories>> UpdateCategoryAsync(CategoriesCuDto itemDto)
    {
        try
        {
            var query1 = _dbContext.Categories
            .Where(i => i.CategoryId == itemDto.CategoryId);
            var item = await query1
                    .FirstOrDefaultAsync<CategoriesDbM>();

            //If the item does not exists
            if (item == null) throw new ArgumentException($"Item {itemDto.CategoryId} is not existing");

            //I cannot have duplicates in the Categories table, so check that
            var query2 = _dbContext.Categories
                .Where(i => i.CategoryName == itemDto.CategoryName);
            var existingItem = await query2.FirstOrDefaultAsync();
            if (existingItem != null && existingItem.CategoryId != itemDto.CategoryId)
                throw new ArgumentException($"Item already exist with id {existingItem.CategoryId}");

            //transfer any changes from DTO to database objects
            //Update individual properties
            item.UpdateFromDTO(itemDto);

            //Update navigation properties
            await navProp_CategoriesCUdto_to_CategoriesDbM(itemDto, item);

            //write to database model
            _dbContext.Categories.Update(item);

            //write to database in a UoW
            await _dbContext.SaveChangesAsync();

            //return the updated item in non-flat mode
            return await ReadCategoryAsync(item.CategoryId, false);
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<ICategories>()
            {
                ErrorMessage = $"Could not update category due to an unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<ResponseItemDto<ICategories>> CreateCategoryAsync(CategoryCreateDto itemDto)
    {
        try
        {
            //I cannot have duplicates in the Categories table, so check that
            var query2 = _dbContext.Categories
            .Where(i => i.CategoryName == itemDto.CategoryName);
            var existingItem = await query2.FirstOrDefaultAsync();
            if (existingItem != null && existingItem.CategoryName != itemDto.CategoryName)
                throw new ArgumentException($"Category already exist with name: {existingItem.CategoryName}");

            var item = new CategoriesDbM
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = itemDto.CategoryName
            };


            //write to database model
            _dbContext.Categories.Add(item);

            //write to database in a UoW
            await _dbContext.SaveChangesAsync();

            //return the updated item in non-flat mode
            return await ReadCategoryAsync(item.CategoryId, false);
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<ICategories>
            {
                ErrorMessage = $"Could not create category due to an unexpected error: {ex.Message}"
            };
        }
    }


    private async Task navProp_CategoriesCUdto_to_CategoriesDbM(CategoriesCuDto itemDtoSrc, CategoriesDbM itemDst)
    {
        //update FriendsDbM from itemDto.FriendId
        List<AttractionsDbM> attractions = null;
        if (itemDtoSrc.AttractionId != null)
        {
            attractions = new List<AttractionsDbM>();
            foreach (var id in itemDtoSrc.AttractionId)
            {
                var f = await _dbContext.Attractions.FirstOrDefaultAsync(i => i.AttractionId == id);
                if (f == null)
                    throw new ArgumentException($"Item id {id} not existing");

                attractions.Add(f);
            }
        }
        itemDst.AttractionsDbM = attractions;
    }
}
