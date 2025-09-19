using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Seido.Utilities.SeedGenerator;
using Models.DTO;
using DbModels;
using DbContext;
using Configuration;
using Models;
using Microsoft.AspNetCore.Http.Features;

namespace DbRepos;

public class AdminDbRepos
{
    private const string _seedSource = "./app-seeds.json";
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
        info.Db = new SupUsrInfoDbDto
        {
            NrSeededUsers = await _dbContext.Users.Where(f => f.Seeded).CountAsync(),
            NrUnseededUsers = await _dbContext.Users.Where(f => !f.Seeded).CountAsync(),
            NrAttractionsWithAddress = await _dbContext.Attractions.Where(f => f.AddressId != null).CountAsync(),

            NrSeededAttractionAddresses = await _dbContext.AttractionAddresses.Where(f => f.Seeded).CountAsync(),
            NrUnseededAttractionAddresses = await _dbContext.AttractionAddresses.Where(f => !f.Seeded).CountAsync(),

            NrSeededAttractions = await _dbContext.Attractions.Where(f => f.Seeded).CountAsync(),
            NrUnseededAttractions = await _dbContext.Attractions.Where(f => !f.Seeded).CountAsync(),

            NrSeededCategories = await _dbContext.Categories.Where(f => f.Seeded).CountAsync(),
            NrUnseededCategories = await _dbContext.Categories.Where(f => !f.Seeded).CountAsync(),

            NrSeededReviews = await _dbContext.Reviews.Where(f => f.Seeded).CountAsync(),
            NrUnseededReviews = await _dbContext.Reviews.Where(f => !f.Seeded).CountAsync(),

            AttractionsWithoutReviews = await _dbContext.Attractions.Where(f => f.Seeded).CountAsync(),

        };

        return new ResponseItemDto<SupUsrInfoAllDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = info
        };
    }



    public async Task SeedAsync()
    {
        //Create a seeder
        var fn = Path.GetFullPath(_seedSource); var seeder = new SeedGenerator(fn);

        #region old code
        //remove existing creditcards in the database
        // _dbContext.CreditCards.RemoveRange(_dbContext.CreditCards);

        //Seeding new creditcards into the database
        // var creditcards = seeder.ItemsToList<CreditCardDbM>(10);
        // _dbContext.CreditCards.AddRange(creditcards);



        // var attractionAddresses = seeder.ItemsToList<AttractionAddressesDbM>(10);
        // _dbContext.AttractionAddresses.AddRange(attractionAddresses);
        #endregion


        #region clear database

        _dbContext.Reviews.RemoveRange(_dbContext.Reviews);
        _dbContext.Categories.RemoveRange(_dbContext.Categories);
        _dbContext.AttractionAddresses.RemoveRange(_dbContext.AttractionAddresses);
        _dbContext.Attractions.RemoveRange(_dbContext.Attractions);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        await _dbContext.SaveChangesAsync();



        #endregion

        // Create attractions


        #region new create attraction
        // List<Attractions> tempAttractions = new List<Attractions>();

        // do
        // {

        // }
        // while (tempAttractions.Count < 100);
        #endregion
        var attractions = seeder.ItemsToList<AttractionsDbM>(10);
        _dbContext.Attractions.AddRange(attractions);

        // Create users
        var users = seeder.ItemsToList<UsersDbM>(10);
        _dbContext.Users.AddRange(users);

        // Addresses
        var addresses = seeder.ItemsToList<AttractionAddressDbM>(10);
        _dbContext.AttractionAddresses.AddRange(addresses);

        // Categories
        var existingCategories = new List<CategoriesDbM>();

        foreach (var attraction in attractions)
        {
            // Address
            attraction.AttractionAddressDbM = seeder.FromList(addresses);

            //         attraction.AttractionAddressDbM =
            // seeder.UniqueItemsPickedFromList(1, addresses);


            // ---- Categories (0–3 per attraction) ----
            var count = seeder.Next(1, 5);
            for (int i = 0; i < count; i++)
            {
                // var candidate = seeder.Item<CategoriesDbM>();
                // change it to not re-fetch the data from the database but instead create a copy of the categories in a list locally
                var candidate = seeder.ItemsToList<CategoriesDbM>(1).First(); ;
                var existing = existingCategories
                    .FirstOrDefault(c =>
                        string.Equals(c.CategoryName, candidate.CategoryName, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    existing = candidate;
                    existingCategories.Add(existing);
                    _dbContext.Categories.Add(existing);
                }

                attraction.CategoriesDbM ??= new List<CategoriesDbM>();
                attraction.CategoriesDbM.Add(existing);
            }
        }

        // ---------- Reviews ----------
        var rnd = new Random();
        foreach (var user in users)
        {
            // Let each user review 0–4 random attractions, but only once per attraction
            var reviewCount = seeder.Next(5, 15);
            var reviewedAttractions = attractions
                .OrderBy(_ => rnd.Next())
                .Take(reviewCount);

            foreach (var attraction in reviewedAttractions)
            {
                // var review = seeder.Item<ReviewsDbM>();
                var review = seeder.ItemsToList<ReviewsDbM>(1).First();
                review.UsersDbM = user;
                review.AttractionsDbM = attraction;
                review.UserId = user.UserId;
                review.AttractionId = attraction.AttractionId;

                _dbContext.Reviews.Add(review);
            }
        }

        // foreach (var attraction in attractions)
        // {
        //     // attraction.AttractionAddressDbM = seeder.AttractionAddressDbM;
        //     attraction.CategoriesDbM = seeder.ItemsToList<CategoriesDbM>(seeder.Next(0, 3));

        // }

        // foreach (var user in users)
        // {
        //     user.ReviewsDbM = seeder.ItemsToList<ReviewsDbM>(seeder.Next(0, 4));

        //     // foreach (var review in user.ReviewsDbM)
        //     // {
        //     //     // review.AttractionsDbM = seeder.UniqueIndexPickedFromList(seeder.Next(0, 1), attractions);
        //     // }
        // }



        // var reviews = seeder.ItemsToList<ReviewsDbM>(10);
        // _dbContext.Reviews.AddRange(reviews);

        // var categories = seeder.ItemsToList<CategoriesDbM>(10);
        // _dbContext.Categories.AddRange(categories);





        //Save changes to the database
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
                AttractionAddressDbM attractionAddresses => attractionAddresses.AddressId,
                CategoriesDbM categoriesDbM => categoriesDbM.CategoryId,
                UsersDbM usersDbM => usersDbM.UserId,
                ReviewsDbM reviewsDbM => reviewsDbM.ReviewId,

                _ => Guid.Empty
            };

            _logger.LogInformation($"{nameof(LogChangeTracker)}: {e.Entity.GetType().Name}: {id} - {e.State}");
        }
    }


}
