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

    // private const string _seedSource = "./app-seeds.json";
    private ILogger<AttractionsDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AttractionsDbRepos(ILogger<AttractionsDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

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
                            i.AttractionDescription.ToLower().Contains(filter)
                            ||
                            i.AttractionAddressesDbM.StreetAddress.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.CityPlace.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.Country.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.ZipCode.ToLower().Contains(filter) ||
                            i.CategoriesDbM.Any(c => c.CategoryName.ToLower().Contains(filter)) ||
                            i.ReviewsDbM.Any(r => r.ReviewText.ToLower().Contains(filter)) ||
                            i.ReviewsDbM.Any(r => r.ReviewScore.ToString().ToLower().Contains(filter))
                            )).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.StreetAddress.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.CityPlace.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.Country.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.ZipCode.ToLower().Contains(filter) ||
                            i.CategoriesDbM.Any(c => c.CategoryName.ToLower().Contains(filter)) ||
                            i.ReviewsDbM.Any(r => r.ReviewText.ToLower().Contains(filter)) ||
                            i.ReviewsDbM.Any(r => r.ReviewScore.ToString().ToLower().Contains(filter))
                        ))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IAttractions>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponsePageDto<IAttractions>> ReadSearchByAttractionAddressCategoryAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize)
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
                .Include(i => i.CategoriesDbM);
            // .Include(i => i.ReviewsDbM);
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
                            i.AttractionDescription.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.StreetAddress.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.CityPlace.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.Country.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.ZipCode.ToLower().Contains(filter) ||
                            i.CategoriesDbM.Any(c => c.CategoryName.ToLower().Contains(filter))
            )).CountAsync(),


            PageItems = await query

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.StreetAddress.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.CityPlace.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.Country.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.ZipCode.ToLower().Contains(filter) ||
                            i.CategoriesDbM.Any(c => c.CategoryName.ToLower().Contains(filter))
                            ))

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
                .Include(i => i.CategoriesDbM);
            // .Include(i => i.ReviewsDbM);
        }

        var addressQuery = query
        .Where(i => (!seeded.HasValue || i.Seeded == seeded.Value) &&
                    (i.AttractionName.ToLower().Contains(filter) ||
                    i.AttractionDescription.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.StreetAddress.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.CityPlace.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.Country.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.ZipCode.ToLower().Contains(filter) ||
                            i.CategoriesDbM.Any(c => c.CategoryName.ToLower().Contains(filter))));

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
                            i.AttractionDescription.ToLower().Contains(filter)
                            ||
                            i.AttractionAddressesDbM.StreetAddress.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.CityPlace.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.Country.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.ZipCode.ToLower().Contains(filter) ||
                            i.CategoriesDbM.Any(c => c.CategoryName.ToLower().Contains(filter))
                            )).CountAsync(),

            PageItems = await addressQuery

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter)
                            ||
                            i.AttractionAddressesDbM.StreetAddress.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.CityPlace.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.Country.ToLower().Contains(filter) ||
                            i.AttractionAddressesDbM.ZipCode.ToLower().Contains(filter) ||
                            i.CategoriesDbM.Any(c => c.CategoryName.ToLower().Contains(filter))
                            ))

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
                .Include(i => i.ReviewsDbM);
        }

        var reviewQuery = query
        .Where(i => (!seeded.HasValue || i.Seeded == seeded.Value) &&
                    (i.AttractionName.ToLower().Contains(filter) ||
                    i.AttractionDescription.ToLower().Contains(filter)
                    ));

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
                            i.AttractionDescription.ToLower().Contains(filter)
                            )).CountAsync(),

            PageItems = await reviewQuery

            //Adding filter functionality
            .Where(i => (seeded == null || i.Seeded == seeded) &&
                        (i.AttractionName.ToLower().Contains(filter) ||
                            i.AttractionDescription.ToLower().Contains(filter)
                            ))

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
            var query = _dbContext.Attractions.AsNoTracking()
                .Include(i => i.AttractionAddressesDbM)
                .Include(i => i.ReviewsDbM)
                .Include(i => i.CategoriesDbM)
                .Where(i => i.AttractionId == id);

            var item = await query.FirstOrDefaultAsync<AttractionsDbM>();
            if (item == null)
            {
                return new ResponseItemDto<IAttractions>
                {
                    ErrorMessage = $"Could not find item with id {id}"

                };
            }

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
        var query = _dbContext.Attractions
            .Where(i => i.AttractionId == id);

        // var item = await query.FirstOrDefaultAsync<AttractionsDbM>();

        //If the item does not exists
        var item = await query.FirstOrDefaultAsync<AttractionsDbM>();
        if (item == null)
        {
            return new ResponseItemDto<IAttractions>
            {
                ErrorMessage = $"Could not find item with id {id}"
            };
        }

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

    public async Task<ResponseItemDto<IAttractions>> UpdateAttractionAsync(AttractionUpdateDto itemDto)
    {
        try
        {
            var item = await _dbContext.Attractions
                .FirstOrDefaultAsync(i => i.AttractionId == itemDto.AttractionId);

            if (item == null)
                return new ResponseItemDto<IAttractions> { ErrorMessage = $"Item {itemDto.AttractionId} does not exist" };

            // Check for duplicates using unique index
            var duplicate = await _dbContext.Attractions
                .FirstOrDefaultAsync(a =>
                    a.AttractionName == itemDto.AttractionName &&
                    a.AttractionDescription == itemDto.AttractionDescription &&
                    a.AddressId == itemDto.AddressId &&
                    a.AttractionId != itemDto.AttractionId);

            if (duplicate != null)
            {
                return new ResponseItemDto<IAttractions>
                {
                    ErrorMessage = $"An attraction with the same name, description, and address already exists (Id: {duplicate.AttractionId})"
                };
            }

            // Transfer changes
            item.UpdateFromDTO(new AttractionsCuDto
            {
                AttractionId = itemDto.AttractionId,
                AttractionName = itemDto.AttractionName,
                AttractionDescription = itemDto.AttractionDescription,
                AddressId = itemDto.AddressId,
                CategoryId = itemDto.CategoryId,
            });
            await navProp_AttractionUpdateDto_to_AttractionsDbM(itemDto, item);

            _dbContext.Attractions.Update(item);
            await _dbContext.SaveChangesAsync();

            return await ReadAttractionAsync(item.AttractionId, false);
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IAttractions>
            {
                ErrorMessage = $"Could not update attraction due to an unexpected error: {ex.Message}"
            };
        }
    }
    public async Task<ResponseItemDto<IAttractions>> CreateAttractionAsync(AttractionCreateDto itemDto)
    {
        try
        {

            // Check for duplicates using unique index
            var duplicate = await _dbContext.Attractions
                .FirstOrDefaultAsync(a =>
                    a.AttractionName == itemDto.AttractionName &&
                    a.AttractionDescription == itemDto.AttractionDescription &&
                    a.AddressId == itemDto.AddressId);

            if (duplicate != null)
            {
                return new ResponseItemDto<IAttractions>
                {
                    ErrorMessage = $"An attraction with the same name, description, and address already exists (Id: {duplicate.AttractionId})"
                };
            }

            // Create new attraction
            var item = new AttractionsDbM
            {
                AttractionId = Guid.NewGuid(),
                AttractionName = itemDto.AttractionName,
                AttractionDescription = itemDto.AttractionDescription,
                AddressId = itemDto.AddressId
            };

            await navProp_AttractionsCUdto_to_AttractionsDbM(itemDto, item);

            _dbContext.Attractions.Add(item);
            await _dbContext.SaveChangesAsync();

            return await ReadAttractionAsync(item.AttractionId, true);
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IAttractions>
            {
                ErrorMessage = $"Could not create attraction due to an unexpected error: {ex.Message}"
            };
        }
    }



    public async Task<ResponseItemDto<IAttractions>> CreateFullAttractionAsync(AttractionFullCreateDto dto)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Address: reuse if exists
                var address = await _dbContext.AttractionAddresses
                    .FirstOrDefaultAsync(a =>
                        a.StreetAddress == dto.Address.StreetAddress &&
                        a.ZipCode == dto.Address.ZipCode &&
                        a.CityPlace == dto.Address.CityPlace &&
                        a.Country == dto.Address.Country);

                if (address == null)
                {
                    address = new AttractionAddressesDbM
                    {
                        AddressId = Guid.NewGuid(),
                        StreetAddress = dto.Address.StreetAddress,
                        ZipCode = dto.Address.ZipCode,
                        CityPlace = dto.Address.CityPlace,
                        Country = dto.Address.Country
                    };
                    _dbContext.AttractionAddresses.Add(address);
                    await _dbContext.SaveChangesAsync();
                }

                // 2. Categories: reuse if exists
                var categories = new List<CategoriesDbM>();
                if (dto.CategoryNames?.Any() == true)
                {
                    foreach (var categoryName in dto.CategoryNames)
                    {
                        var trimmedName = categoryName.Trim();

                        // Try to find existing category by name
                        var category = await _dbContext.Categories
                            .FirstOrDefaultAsync(c => c.CategoryName == trimmedName);

                        // If not found, create it
                        if (category == null)
                        {
                            category = new CategoriesDbM
                            {
                                CategoryId = Guid.NewGuid(),
                                CategoryName = trimmedName
                            };
                            _dbContext.Categories.Add(category);
                            await _dbContext.SaveChangesAsync();
                        }

                        categories.Add(category);
                    }
                }

                // Include explicitly existing category IDs
                // if (dto.ExistingCategoryIds?.Any() == true)
                // {
                //     var existing = await _dbContext.Categories
                //         .Where(c => dto.ExistingCategoryIds.Contains(c.CategoryId))
                //         .ToListAsync();
                //     categories.AddRange(existing);
                // }

                // 3. Check unique index gracefully
                var duplicateAttraction = await _dbContext.Attractions
                    .FirstOrDefaultAsync(a =>
                        a.AttractionName == dto.AttractionName &&
                        a.AttractionDescription == dto.AttractionDescription &&
                        a.AddressId == address.AddressId);

                if (duplicateAttraction != null)
                {
                    return new ResponseItemDto<IAttractions>
                    {
                        ErrorMessage = $"An attraction with the same name, description, and address already exists (Id: {duplicateAttraction.AttractionId})"
                    };
                }

                // 4. Create new attraction
                var attraction = new AttractionsDbM
                {
                    AttractionId = Guid.NewGuid(),
                    AttractionName = dto.AttractionName,
                    AttractionDescription = dto.AttractionDescription,
                    AddressId = address.AddressId,
                    CategoriesDbM = categories
                };

                _dbContext.Attractions.Add(attraction);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return await ReadAttractionAsync(attraction.AttractionId, false);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResponseItemDto<IAttractions>
                {
                    ErrorMessage = $"Could not create attraction due to an unexpected error: {ex.Message}"
                };
            }
        });
    }




    private async Task navProp_AttractionUpdateDto_to_AttractionsDbM(AttractionUpdateDto itemDtoSrc, AttractionsDbM itemDst)
    {
        List<ReviewsDbM> reviews = null;
        // if (itemDtoSrc.ReviewId != null)
        // {
        //     reviews = new List<ReviewsDbM>();
        //     foreach (var id in itemDtoSrc.ReviewId)
        //     {
        //         var f = await _dbContext.Reviews.FirstOrDefaultAsync(i => i.ReviewId == id);
        //         if (f == null)
        //             throw new ArgumentException($"Item id {id} not existing");

        //         reviews.Add(f);
        //     }
        // }
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

            itemDst.AttractionAddressesDbM = attractionAddresses;
        }
        else
        {
            itemDst.AttractionAddressesDbM = null;
        }

    }

    private async Task navProp_AttractionsCUdto_to_AttractionsDbM(AttractionCreateDto itemDtoSrc, AttractionsDbM itemDst)
    {
        List<ReviewsDbM> reviews = null;
        // if (itemDtoSrc.ReviewId != null)
        // {
        //     reviews = new List<ReviewsDbM>();
        //     foreach (var id in itemDtoSrc.ReviewId)
        //     {
        //         var f = await _dbContext.Reviews.FirstOrDefaultAsync(i => i.ReviewId == id);
        //         if (f == null)
        //             throw new ArgumentException($"Item id {id} not existing");

        //         reviews.Add(f);
        //     }
        // }
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

            itemDst.AttractionAddressesDbM = attractionAddresses;
        }
        else
        {
            itemDst.AttractionAddressesDbM = null;
        }

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
