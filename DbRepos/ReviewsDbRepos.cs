using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class ReviewsDbRepos
{
    private ILogger<ReviewsDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public ReviewsDbRepos(ILogger<ReviewsDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IReviews>> ReadReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<ReviewsDbM> query;
        if (flat)
        {
            query = _dbContext.Reviews.AsNoTracking();
        }
        else
        {
            query = _dbContext.Reviews.AsNoTracking()
                .Include(i => i.AttractionsDbM)
                .Include(i => i.UsersDbM);
        }

        var ret = new ResponsePageDto<IReviews>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.ReviewScore.ToString().Contains(filter) ||
                            i.ReviewText.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.ReviewScore.ToString().Contains(filter) ||
                            i.ReviewText.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IReviews>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }
}
