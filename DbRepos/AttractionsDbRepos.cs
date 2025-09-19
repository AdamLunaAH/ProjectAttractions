using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class AttractionsDbRepos
{

    private const string _seedSource = "./app-seeds.json";
    private ILogger<AttractionsDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AttractionsDbRepos(ILogger<AttractionsDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

//     public async Task<ResponseItemDto<SupUsrInfoAttractionsDtoInfo>> InfoAsync() => await DbInfo();

//     private async Task<ResponseItemDto<SupUsrInfoAttractionsDtoInfo>> DbInfo()
//     {
//         var info = new SupUsrInfoAttractionsDtoInfo();
//         info.Db = new SupUsrInfoAttractionsDtoInfo
//         {
//             NrSeededFriends = await _dbContext.Friends.Where(f => f.Seeded).CountAsync(),
//             NrUnseededFriends = await _dbContext.Friends.Where(f => !f.Seeded).CountAsync(),
//             NrFriendsWithAddress = await _dbContext.Friends.Where(f => f.AddressId != null).CountAsync(),

//             NrSeededAddresses = await _dbContext.Addresses.Where(f => f.Seeded).CountAsync(),
//             NrUnseededAddresses = await _dbContext.Addresses.Where(f => !f.Seeded).CountAsync(),

//             NrSeededPets = await _dbContext.Pets.Where(f => f.Seeded).CountAsync(),
//             NrUnseededPets = await _dbContext.Pets.Where(f => !f.Seeded).CountAsync(),

//             NrSeededQuotes = await _dbContext.Quotes.Where(f => f.Seeded).CountAsync(),
//             NrUnseededQuotes = await _dbContext.Quotes.Where(f => !f.Seeded).CountAsync(),
//         };

//         return new ResponseItemDto<SupUsrInfoAttractionsDtoInfo>
//         {
// #if DEBUG
//             ConnectionString = _dbContext.dbConnection,
// #endif

//             Item = info
//         };
//     }




    public async Task<ResponsePageDto<IAttractions>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<AttractionsDbM> query;
        if (flat)
        {
            query = _dbContext.Attractions.AsNoTracking();
        }
        else
        {
            query = _dbContext.Attractions.AsNoTracking()
                .Include(i => i.AttractionAddressDbM)
                .Include(i => i.CategoriesDbM)
                .Include(i => i.ReviewsDbM);
        }

        var ret = new ResponsePageDto<IAttractions>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IAttractions>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponsePageDto<IAttractions>> ReadAttractionsNoReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<AttractionsDbM> query;
        if (flat)
        {
            query = _dbContext.Attractions.AsNoTracking();
        }
        else
        {
            query = _dbContext.Attractions.AsNoTracking()
                .Include(i => i.AttractionAddressDbM)
                .Include(i => i.CategoriesDbM)
                .Include(i => i.ReviewsDbM);
        }

        var ret = new ResponsePageDto<IAttractions>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IAttractions>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }
}
