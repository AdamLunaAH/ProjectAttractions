using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

// using Models.Utilities.SeedGenerator;
using Models.DTO;
using DbModels;
using DbContext;
using Configuration;
using Models;
using Microsoft.AspNetCore.Http.Features;

namespace DbRepos;

public class AdminDbRepos
{
    private readonly ILogger<AdminDbRepos> _logger;
    private Encryptions _encryptions;
    private readonly MainDbContext _dbContext;
    public AdminDbRepos(ILogger<AdminDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }

    public async Task<ResponseItemDto<SupUsrInfoAllDto>> InfoAsync() => await DbInfo();

    private async Task<ResponseItemDto<SupUsrInfoAllDto>> DbInfo()
    {
        var info = new SupUsrInfoAllDto();
        info.Db = await _dbContext.SUInfoDbView.FirstAsync();

        return new ResponseItemDto<SupUsrInfoAllDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = info
        };
    }
    // This method is for debugging purposes only and to demonstrate the ChangeTracker
    private void LogChangeTracker()
    {
        foreach (var e in _dbContext.ChangeTracker.Entries())
        {
            var id = e.Entity switch
            {
                AttractionsDbM attractionsDbM => attractionsDbM.AttractionId,
                AttractionAddressesDbM attractionAddresses => attractionAddresses.AddressId,
                CategoriesDbM categoriesDbM => categoriesDbM.CategoryId,
                UsersDbM usersDbM => usersDbM.UserId,
                ReviewsDbM reviewsDbM => reviewsDbM.ReviewId,

                _ => Guid.Empty
            };

            _logger.LogInformation($"{nameof(LogChangeTracker)}: {e.Entity.GetType().Name}: {id} - {e.State}");
        }
    }
}
