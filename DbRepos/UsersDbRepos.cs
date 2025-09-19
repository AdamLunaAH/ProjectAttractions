using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class UsersDbRepos
{
    private ILogger<UsersDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public UsersDbRepos(ILogger<UsersDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IUsers>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<UsersDbM> query;
        if (flat)
        {
            query = _dbContext.Users.AsNoTracking();
        }
        else
        {
            query = _dbContext.Users.AsNoTracking()
                .Include(i => i.ReviewsDbM);
        }

        var ret = new ResponsePageDto<IUsers>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.FirstName.ToLower().Contains(filter) ||
                            i.LastName.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.FirstName.ToLower().Contains(filter) ||
                            i.LastName.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IUsers>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }
}
