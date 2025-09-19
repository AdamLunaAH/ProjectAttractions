using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class AttractionAddressesDbRepos
{
    private ILogger<AttractionAddressesDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AttractionAddressesDbRepos(ILogger<AttractionAddressesDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IAttractionAddresses>> ReadAttractionAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<AttractionAddressDbM> query;
        if (flat)
        {
            query = _dbContext.AttractionAddresses.AsNoTracking();
        }
        else
        {
            query = _dbContext.AttractionAddresses.AsNoTracking()
                .Include(i => i.AttractionsDbM);
        }

        var ret = new ResponsePageDto<IAttractionAddresses>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.Country.ToLower().Contains(filter) ||
                            i.CityPlace.ToLower().Contains(filter) ||
                            i.ZipCode.ToLower().Contains(filter) ||
                            i.StreetAddress.ToLower().Contains(filter)))
                            .CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        i.Country.ToLower().Contains(filter) ||
                            i.CityPlace.ToLower().Contains(filter) ||
                            i.ZipCode.ToLower().Contains(filter) ||
                            i.StreetAddress.ToLower().Contains(filter))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IAttractionAddresses>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }
}
