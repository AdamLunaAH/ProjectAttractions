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




    public async Task<ResponsePageDto<IAttractions>> ReadAttractionsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize)
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
                .Include(i => i.AttractionAddressesDbM)
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
            .Where(i => (seeded == null || i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
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

    public async Task<ResponsePageDto<IAttractions>> ReadAttractionsNoAddressAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize, bool noAddress)
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
                .Include(i => i.AttractionAddressesDbM)
                .Include(i => i.CategoriesDbM)
                .Include(i => i.ReviewsDbM);
        }

        var addressQuery = query
        .Where(i => (!seeded.HasValue || i.Seeded == seeded.Value) &&
                    (i.AttractionName.ToLower().Contains(filter) ||
                    i.AttractionDescription.ToLower().Contains(filter)));

        if (noAddress)
        {
            // Only attractions with no address
            addressQuery = addressQuery.Where(i => i.AttractionAddressesDbM == null);
        }

        var ret = new ResponsePageDto<IAttractions>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await addressQuery

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter))).CountAsync(),

            PageItems = await addressQuery

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
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


    public async Task<ResponsePageDto<IAttractions>> ReadAttractionsNoReviewsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize, bool noReview)
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
                .Include(i => i.AttractionAddressesDbM)
                .Include(i => i.CategoriesDbM)
                .Include(i => i.ReviewsDbM);
        }

        var reviewQuery = query
        .Where(i => (!seeded.HasValue || i.Seeded == seeded.Value) &&
                    (i.AttractionName.ToLower().Contains(filter) ||
                    i.AttractionDescription.ToLower().Contains(filter)));

        if (noReview)
        {
            reviewQuery = reviewQuery.Where(i => !i.ReviewsDbM.Any());
        }

        var ret = new ResponsePageDto<IAttractions>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await reviewQuery

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter))).CountAsync(),

            PageItems = await reviewQuery

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
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

    public async Task<ResponseItemDto<IAttractions>> ReadAttractionAsync(Guid id, bool flat)
    {
        if (!flat)
        {
            //make sure the model is fully populated, try without include.
            //remove tracking for all read operations for performance and to avoid recursion/circular access
            var query = _dbContext.Attractions.AsNoTracking()
                .Include(i => i.ReviewsDbM)
                .Include(i => i.CategoriesDbM)
                .Where(i => i.AttractionId == id);

            return new ResponseItemDto<IAttractions>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = await query.FirstOrDefaultAsync<IAttractions>()
            };
        }
        else
        {
            //Not fully populated, compare the SQL Statements generated
            //remove tracking for all read operations for performance and to avoid recursion/circular access
            var query = _dbContext.Attractions.AsNoTracking()
                .Where(i => i.AttractionId == id);

            return new ResponseItemDto<IAttractions>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = await query.FirstOrDefaultAsync<IAttractions>()
            };
        }
    }


    public async Task<ResponseItemDto<IAttractions>> DeleteAttractionAsync(Guid id)
    {
        var query1 = _dbContext.Attractions
            .Where(i => i.AttractionId == id);

        var item = await query1.FirstOrDefaultAsync<AttractionsDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Attractions.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return new ResponseItemDto<IAttractions>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = item
        };
    }

    public async Task<ResponseItemDto<IAttractions>> UpdateAttractionAsync(AttractionsCuDto itemDto)
    {
        var query1 = _dbContext.Attractions
            .Where(i => i.AttractionId == itemDto.AttractionId);
        var item = await query1
                // .Include(i => i.ReviewsDbM)
                .FirstOrDefaultAsync<AttractionsDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {itemDto.AttractionId} is not existing");

        //I cannot have duplicates in the Attractions table, so check that
        var query2 = _dbContext.Attractions
            .Where(i => i.AttractionName == itemDto.AttractionName);
        var existingItem = await query2.FirstOrDefaultAsync();
        if (existingItem != null && existingItem.AttractionId != itemDto.AttractionId)
            throw new ArgumentException($"Item already exist with id {existingItem.AttractionId}");

        //transfer any changes from DTO to database objects
        //Update individual properties
        item.UpdateFromDTO(itemDto);

        //Update navigation properties
        await navProp_AttractionsCUdto_to_AttractionsDbM(itemDto, item);

        //write to database model
        _dbContext.Attractions.Update(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadAttractionAsync(item.AttractionId, false);
    }

    public async Task<ResponseItemDto<IAttractions>> CreateAttractionAsync(AttractionsCuDto itemDto)
    {
        // if (itemDto.AttractionId != null)
        //     throw new ArgumentException($"{nameof(itemDto.AttractionId)} must be null when creating a new object");

        if (itemDto.AttractionId != Guid.Empty)
            throw new ArgumentException("AttractionId must be empty when creating a new attraction.");


        //I cannot have duplicates in the Attractions table, so check that
        var query2 = _dbContext.Attractions
            .Where(i => i.AttractionName == itemDto.AttractionName);
        var existingItem = await query2.FirstOrDefaultAsync();
        if (existingItem != null && existingItem.AttractionId != itemDto.AttractionId)
            throw new ArgumentException($"Item already exist with id {existingItem.AttractionId}");

        //transfer any changes from DTO to database objects
        //Update individual properties
        var item = new AttractionsDbM(itemDto);

        //Update navigation properties
        await navProp_AttractionsCUdto_to_AttractionsDbM(itemDto, item);

        //write to database model
        _dbContext.Attractions.Add(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadAttractionAsync(item.AttractionId, true);
    }

    private async Task navProp_AttractionsCUdto_to_AttractionsDbM(AttractionsCuDto itemDtoSrc, AttractionsDbM itemDst)
    {
        //update FriendsDbM from itemDto.FriendId
        List<ReviewsDbM> reviews = null;
        if (itemDtoSrc.ReviewId != null)
        {
            reviews = new List<ReviewsDbM>();
            foreach (var id in itemDtoSrc.ReviewId)
            {
                var f = await _dbContext.Reviews.FirstOrDefaultAsync(i => i.ReviewId == id);
                if (f == null)
                    throw new ArgumentException($"Item id {id} not existing");

                reviews.Add(f);
            }
        }
        itemDst.ReviewsDbM = reviews;


        List<CategoriesDbM> categories = null;
        if (itemDtoSrc.CategoryId != null)
        {
            categories = new List<CategoriesDbM>();
            foreach (var id in itemDtoSrc.CategoryId)
            {
                var f = await _dbContext.Categories.FirstOrDefaultAsync(i => i.CategoryId == id);
                if (f == null)
                    throw new ArgumentException($"Item id {id} not existing");

                categories.Add(f);
            }
        }
        itemDst.CategoriesDbM = categories;


        if (itemDtoSrc.AddressId.HasValue)
        {
            var attractionAddresses = await _dbContext.AttractionAddresses
                .FirstOrDefaultAsync(a => a.AddressId == itemDtoSrc.AddressId.Value);

            if (attractionAddresses == null)
                throw new ArgumentException(
                    $"Attraction id {itemDtoSrc.AddressId} does not exist");

            itemDst.AttractionAddresses = attractionAddresses;
        }
        else
        {
            itemDst.AttractionAddresses = null;
        }

    }
}
