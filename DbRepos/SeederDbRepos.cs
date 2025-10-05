using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models.Utilities.SeedGenerator;
using Models.DTO;
using DbModels;
using DbContext;
using Configuration;
using Models;
using Microsoft.AspNetCore.Http.Features;

namespace DbRepos;

public class SeederDbRepos
{
    private const string _seedSource = "./app-seeds.json";
    private static bool IsAppSeedsEmpty = false;

    private readonly ILogger<SeederDbRepos> _logger;
    private Encryptions _encryptions;
    private readonly MainDbContext _dbContext;

    public SeederDbRepos(ILogger<SeederDbRepos> logger, Encryptions encryptions, MainDbContext context)
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
        // info.Db = new SupUsrInfoDbDto
        // {
        //     // NrUsers = await _dbContext.UsersDb.CountAsync(),
        //     // NrSeededUsers = await _dbContext.UsersDb.Where(f => f.Seeded).CountAsync(),
        //     // NrUnseededUsers = await _dbContext.UsersDb.Where(f => !f.Seeded).CountAsync(),

        //     // NrSeededAttractionAddresses = await _dbContext.AttractionAddressesDb.Where(f => f.Seeded).CountAsync(),
        //     // NrUnseededAttractionAddresses = await _dbContext.AttractionAddressesDb.Where(f => !f.Seeded).CountAsync(),
        //     // NrAttractionAddresses = await _dbContext.AttractionAddressesDb.CountAsync(),
        //     // NrAttractions = await _dbContext.AttractionsDb.CountAsync(),

        //     // NrSeededAttractions = await _dbContext.AttractionsDb.Where(f => f.Seeded).CountAsync(),
        //     // NrUnseededAttractions = await _dbContext.AttractionsDb.Where(f => !f.Seeded).CountAsync(),
        //     // NrAttractionsWithNoAddress = await _dbContext.AttractionsDb.Where(f => f.AddressId == null).CountAsync(),

        //     // NrCategories = await _dbContext.CategoriesDb.CountAsync(),
        //     // NrSeededCategories = await _dbContext.CategoriesDb.Where(f => f.Seeded).CountAsync(),
        //     // NrUnseededCategories = await _dbContext.CategoriesDb.Where(f => !f.Seeded).CountAsync(),

        //     // NrReviews = await _dbContext.ReviewsDb.CountAsync(),
        //     // NrSeededReviews = await _dbContext.ReviewsDb.Where(f => f.Seeded).CountAsync(),
        //     // NrUnseededReviews = await _dbContext.ReviewsDb.Where(f => !f.Seeded).CountAsync(),


        // };

        return new ResponseItemDto<SupUsrInfoAllDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = info
        };
    }



    public async Task<ResponseItemDto<SupUsrInfoAllDto>> SeedAllAsync()
{
    //Create a seeder
    var fn = Path.GetFullPath(_seedSource);
    var info = new FileInfo(fn);
    if (info.Length < 20)
    {
        IsAppSeedsEmpty = true;
    }

    if (IsAppSeedsEmpty == true)
    {
        try
        {
            // Create a master seed file using SeedGenerator and write it to the app-seeds.json location
            var appSeedPath = new SeedGenerator().WriteMasterStream(fn);
            _logger.LogInformation("app-seeds file created at: {path}", appSeedPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create app-seeds file at {path}", fn);
            throw;
        }
    }
    //Create a seeder
    var seeder = new SeedGenerator(fn);

        #region clear database
        // _dbContext.Reviews.RemoveRange(_dbContext.Reviews);
        // _dbContext.Categories.RemoveRange(_dbContext.Categories);
        // _dbContext.AttractionAddresses.RemoveRange(_dbContext.AttractionAddresses);
        // _dbContext.Attractions.RemoveRange(_dbContext.Attractions);
        // _dbContext.Users.RemoveRange(_dbContext.Users);
        // await _dbContext.SaveChangesAsync();

    #endregion

        var rnd = new Random();

    // --- 1) Create Users (80) and ensure unique emails ---
    var users = seeder.ItemsToList<UsersDbM>(80);
    var existingEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var user in users)
    {
        // ensure unique email in set and DB
        var originalEmail = user.Email;
        var email = originalEmail;
        int attempts = 0;
        while (existingEmails.Contains(email) || await _dbContext.Users.AnyAsync(u => u.Email == email))
        {
            attempts++;
            // append a random number suffix if conflict
            email = $"{originalEmail.Split('@')[0]}{seeder.Next(1, 10000)}@{originalEmail.Split('@')[1]}";
            if (attempts > 10) // fallback safety
                email = $"{originalEmail.Split('@')[0]}{Guid.NewGuid():N}@{originalEmail.Split('@')[1]}";
        }
        user.Email = email.ToLower();
        existingEmails.Add(user.Email);
        _dbContext.Users.Add(user);
    }

    await _dbContext.SaveChangesAsync();

    // --- 2) Create a pool of addresses (reused by many attractions) ---
    // We'll create fewer addresses than attractions so they can be reused.
    // e.g. create 200 addresses to be shared.
    var addressPoolSize = 200;
    var addresses = seeder.ItemsToList<AttractionAddressesDbM>(addressPoolSize);
    foreach (var addr in addresses)
    {
        _dbContext.AttractionAddresses.Add(addr);
    }
    await _dbContext.SaveChangesAsync();

    // --- 3) Prepare category dedupe store ---
    var existingCategories = new List<CategoriesDbM>();

    // --- 4) Create 1200 attractions ---
    var attractionCount = 1200;
    var attractions = seeder.ItemsToList<AttractionsDbM>(attractionCount);

    foreach (var attraction in attractions)
    {
        // 1-in-10 chance of NOT having an address
        var useAddress = seeder.Next(1, 11) != 1; // 1..10 -> treat 1 as "no address"
        if (useAddress && addresses.Count > 0)
        {
            // pick random address from the pool
            var addr = addresses[rnd.Next(addresses.Count)];
            attraction.AttractionAddressesDbM = addr;
            attraction.AddressId = addr.AddressId;
            // also add the attraction to address navigation if you want
            addr.AttractionsDbM ??= new List<AttractionsDbM>();
            addr.AttractionsDbM.Add(attraction);
        }
        else
        {
            attraction.AttractionAddressesDbM = null;
            attraction.AddressId = null;
        }

        // Categories: 0 - 6 categories
        var categoryCount = seeder.Next(0, 7); // 0..6
        if (categoryCount > 0)
        {
            attraction.CategoriesDbM ??= new List<CategoriesDbM>();
            for (int c = 0; c < categoryCount; c++)
            {
                // create candidate category
                var candidate = seeder.ItemsToList<CategoriesDbM>(1).First();
                var existing = existingCategories
                    .FirstOrDefault(ec => string.Equals(ec.CategoryName, candidate.CategoryName, StringComparison.OrdinalIgnoreCase));
                if (existing == null)
                {
                    existing = candidate;
                    existingCategories.Add(existing);
                    _dbContext.Categories.Add(existing);
                }

                // avoid duplicate category on same attraction
                if (!attraction.CategoriesDbM.Any(x => string.Equals(x.CategoryName, existing.CategoryName, StringComparison.OrdinalIgnoreCase)))
                {
                    attraction.CategoriesDbM.Add(existing);
                    existing.AttractionsDbM ??= new List<AttractionsDbM>();
                    existing.AttractionsDbM.Add(attraction);
                }
            }
        }

        _dbContext.Attractions.Add(attraction);
    }

    await _dbContext.SaveChangesAsync();

    // Persist categories added during attraction loop
    await _dbContext.SaveChangesAsync();

    // --- 5) Reviews: For each attraction create 0 - 20 reviews, but at most one per user/attraction.
    // We already have 'users' list loaded from DB earlier; make sure we fetch fresh user list from DB with keys
    var userList = await _dbContext.Users.ToListAsync(); // UsersDbM list
    var userCount = userList.Count;
    foreach (var attraction in attractions)
    {
        // number of reviews for this attraction: 0..20 (cap at number of users)
        var reviewCount = seeder.Next(0, 21); // 0..20
        if (reviewCount == 0 || userCount == 0) continue;

        // cap at number of users (one review per user constraint)
        reviewCount = Math.Min(reviewCount, userCount);

        // random unique users for this attraction: shuffle indices and take reviewCount
        var userIndices = Enumerable.Range(0, userCount).OrderBy(_ => rnd.Next()).Take(reviewCount).ToList();

        foreach (var idx in userIndices)
        {
            var user = userList[idx];

            var review = seeder.ItemsToList<ReviewsDbM>(1).First();
            review.UsersDbM = user;
            review.UserId = user.UserId;
            review.AttractionsDbM = attraction;
            review.AttractionId = attraction.AttractionId;

            // push to DB
            _dbContext.Reviews.Add(review);

            // set navigation collections locally if needed
            user.ReviewsDbM ??= new List<ReviewsDbM>();
            user.ReviewsDbM.Add(review);

            attraction.ReviewsDbM ??= new List<ReviewsDbM>();
            attraction.ReviewsDbM.Add(review);
        }
    }

    LogChangeTracker();
    await _dbContext.SaveChangesAsync();
    LogChangeTracker();
    return await DbInfo();
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
