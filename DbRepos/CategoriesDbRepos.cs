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

    public async Task<ResponsePageDto<ICategories>> ReadCategoriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
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
            .Where(i => (i.Seeded == seeded) &&
                        (i.CategoryName.ToLower().Contains(filter)))
                        .CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.CategoryName.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<ICategories>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }
}
