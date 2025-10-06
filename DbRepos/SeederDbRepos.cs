using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

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
        info.Db = await _dbContext.SUInfoDbView
    .OrderBy(v => 1)
    .FirstOrDefaultAsync();


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
        try
        {


            // Create a seeder
            var fn = Path.GetFullPath(_seedSource);
            var info = new FileInfo(fn);
            // Check if the seed file is empty (an empty file could still have some value)
            if (info.Length < 20)
            {
                IsAppSeedsEmpty = true;
            }
            // If the seed file is empty, create a new app-seed file
            if (IsAppSeedsEmpty)
            {
                try
                {
                    var appSeedPath = new SeedGenerator().WriteMasterStream(fn);
                    _logger.LogInformation("app-seeds file created at: {path}", appSeedPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create app-seeds file at {path}", fn);
                    throw;
                }
            }

            // Seeder
            var seeder = new SeedGenerator(fn);

            #region Clear Database
            await _dbContext.Database.ExecuteSqlRawAsync("EXEC supusr.sp_ClearDatabase");
            #endregion

            var rnd = new Random();

            // 1) Users
            var users = seeder.ItemsToList<UsersDbM>(80);
            var existingEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var user in users)
            {
                var originalEmail = user.Email;
                var email = originalEmail;
                int attempts = 0;
                while (existingEmails.Contains(email) || await _dbContext.Users.AnyAsync(u => u.Email == email))
                {
                    attempts++;
                    email = $"{originalEmail.Split('@')[0]}{seeder.Next(1, 10000)}@{originalEmail.Split('@')[1]}";
                    if (attempts > 10)
                        email = $"{originalEmail.Split('@')[0]}{Guid.NewGuid():N}@{originalEmail.Split('@')[1]}";
                }
                user.Email = email.ToLower();
                existingEmails.Add(user.Email);
                _dbContext.Users.Add(user);
            }
            await _dbContext.SaveChangesAsync();

            // 2) Addresses
            var addressPoolSize = 200;
            var addresses = seeder.ItemsToList<AttractionAddressesDbM>(addressPoolSize);
            var uniqueAddressKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var addr in addresses)
            {
                string key = $"{addr.StreetAddress}|{addr.ZipCode}|{addr.CityPlace}|{addr.Country}";
                int suffix = 1;
                while (uniqueAddressKeys.Contains(key) ||
                        await _dbContext.AttractionAddresses.AnyAsync(a =>
                            a.StreetAddress == addr.StreetAddress &&
                            a.ZipCode == addr.ZipCode &&
                            a.CityPlace == addr.CityPlace &&
                            a.Country == addr.Country))
                {
                    addr.StreetAddress = $"{addr.StreetAddress} #{suffix++}";
                    key = $"{addr.StreetAddress}|{addr.ZipCode}|{addr.CityPlace}|{addr.Country}";
                }
                uniqueAddressKeys.Add(key);
                _dbContext.AttractionAddresses.Add(addr);
            }
            await _dbContext.SaveChangesAsync();

            // 3) Categories
            var existingCategories = new List<CategoriesDbM>();
            var existingCategoryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //  4) Attractions
            var attractionCount = 1200;
            var attractions = seeder.ItemsToList<AttractionsDbM>(attractionCount);
            var uniqueAttractionKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var attraction in attractions)
            {
                // Assignment address
                var useAddress = seeder.Next(1, 11) != 1;
                if (useAddress && addresses.Count > 0)
                {
                    var addr = addresses[rnd.Next(addresses.Count)];
                    attraction.AttractionAddressesDbM = addr;
                    attraction.AddressId = addr.AddressId;
                    addr.AttractionsDbM ??= new List<AttractionsDbM>();
                    addr.AttractionsDbM.Add(attraction);
                }
                else
                {
                    attraction.AttractionAddressesDbM = null;
                    attraction.AddressId = null;
                }

                // Assignment categories
                var categoryCount = seeder.Next(0, 7);
                if (categoryCount > 0)
                {
                    attraction.CategoriesDbM ??= new List<CategoriesDbM>();
                    for (int c = 0; c < categoryCount; c++)
                    {
                        var candidate = seeder.ItemsToList<CategoriesDbM>(1).First();
                        if (!existingCategoryNames.Contains(candidate.CategoryName))
                        {
                            existingCategoryNames.Add(candidate.CategoryName);
                            existingCategories.Add(candidate);
                            _dbContext.Categories.Add(candidate);
                        }

                        var existing = existingCategories.First(ec =>
                            string.Equals(ec.CategoryName, candidate.CategoryName, StringComparison.OrdinalIgnoreCase));

                        if (!attraction.CategoriesDbM.Any(x => string.Equals(x.CategoryName, existing.CategoryName, StringComparison.OrdinalIgnoreCase)))
                        {
                            attraction.CategoriesDbM.Add(existing);
                            existing.AttractionsDbM ??= new List<AttractionsDbM>();
                            existing.AttractionsDbM.Add(attraction);
                        }
                    }
                }

                // Unique Attraction
                int suffix = 1;
                bool unique = false;
                string key = $"{attraction.AttractionName}|{attraction.AttractionDescription}|{attraction.AddressId}";

                while (!unique)
                {
                    if (uniqueAttractionKeys.Contains(key) || await _dbContext.Attractions.AnyAsync(a =>
                        a.AttractionName == attraction.AttractionName &&
                        a.AttractionDescription == attraction.AttractionDescription &&
                        a.AddressId == attraction.AddressId))
                    {
                        attraction.AttractionDescription = $"{attraction.AttractionDescription} (Alt {suffix++})";
                        key = $"{attraction.AttractionName}|{attraction.AttractionDescription}|{attraction.AddressId}";
                    }
                    else
                    {
                        uniqueAttractionKeys.Add(key);
                        unique = true;
                    }
                }

                _dbContext.Attractions.Add(attraction);
            }
            await _dbContext.SaveChangesAsync();

            // 5) Reviews - create 0 - 20 reviews, checks if user-attraction pairs are unique.
            var userList = await _dbContext.Users.ToListAsync();
            int userCount = userList.Count;
            var uniqueReviews = new HashSet<(Guid AttractionId, Guid UserId)>();

            foreach (var attraction in attractions)
            {
                var reviewCount = seeder.Next(0, 21);
                if (reviewCount == 0 || userCount == 0) continue;

                reviewCount = Math.Min(reviewCount, userCount);
                var userIndices = Enumerable.Range(0, userCount).OrderBy(_ => rnd.Next()).Take(reviewCount).ToList();

                foreach (var idx in userIndices)
                {
                    var user = userList[idx];

                    if (uniqueReviews.Contains((attraction.AttractionId, user.UserId))) continue;

                    var review = seeder.ItemsToList<ReviewsDbM>(1).First();
                    review.UsersDbM = user;
                    review.UserId = user.UserId;
                    review.AttractionsDbM = attraction;
                    review.AttractionId = attraction.AttractionId;

                    _dbContext.Reviews.Add(review);

                    user.ReviewsDbM ??= new List<ReviewsDbM>();
                    user.ReviewsDbM.Add(review);

                    attraction.ReviewsDbM ??= new List<ReviewsDbM>();
                    attraction.ReviewsDbM.Add(review);

                    uniqueReviews.Add((attraction.AttractionId, user.UserId));
                }
            }

            LogChangeTracker();
            await _dbContext.SaveChangesAsync();
            LogChangeTracker();

            return await DbInfo();
        }
        catch (Exception ex)
        {

            return new ResponseItemDto<SupUsrInfoAllDto>
            {
                ErrorMessage = $"Could not create attraction due to an unexpected error: {ex.Message}"
            };
        }
    }

    // Log the current state of the change tracker
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
    public async Task<ResponseItemDto<string>> RemoveSeededDataAsync()
    {
        try
        {
            int affected = await _dbContext.Database.ExecuteSqlRawAsync("EXEC supusr.sp_ClearSeededDatabase");

            return new ResponseItemDto<string>
            {
                Item = $"Removed seeded data, affected rows: {affected}"
            };
        }
        catch (Exception ex)
        {

            return new ResponseItemDto<string>
            {
                ErrorMessage = $"Could not remove seeded data due to an unexpected error: {ex.Message}"
            };
        }
    }
    public async Task<ResponseItemDto<string>> RemoveAllDataAsync()
    {
        try
        {
            int affected = await _dbContext.Database.ExecuteSqlRawAsync("EXEC supusr.sp_ClearDatabase");

            return new ResponseItemDto<string>
            {
                Item = $"Removed all data, affected rows: {affected}"
            };
        }
        catch (Exception ex)
        {

            return new ResponseItemDto<string>
            {
                ErrorMessage = $"Could not remove all data due to an unexpected error: {ex.Message}"
            };
        }
    }


}
