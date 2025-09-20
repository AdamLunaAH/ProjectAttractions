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
        IQueryable<AttractionAddressesDbM> query;
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

    public async Task<ResponseItemDto<IAttractionAddresses>> ReadAttractionAddressAsync(Guid id, bool flat)
    {
        if (!flat)
        {
            //make sure the model is fully populated, try without include.
            //remove tracking for all read operations for performance and to avoid recursion/circular access
            var query = _dbContext.AttractionAddresses.AsNoTracking()
                .Include(i => i.AttractionsDbM)
                .Where(i => i.AddressId == id);

            return new ResponseItemDto<IAttractionAddresses>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = await query.FirstOrDefaultAsync<IAttractionAddresses>()
            };
        }
        else
        {
            //Not fully populated, compare the SQL Statements generated
            //remove tracking for all read operations for performance and to avoid recursion/circular access
            var query = _dbContext.AttractionAddresses.AsNoTracking()
                .Where(i => i.AddressId == id);

            return new ResponseItemDto<IAttractionAddresses>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = await query.FirstOrDefaultAsync<IAttractionAddresses>()
            };
        }
    }


    public async Task<ResponseItemDto<IAttractionAddresses>> DeleteAttractionAddressAsync(Guid id)
    {
        var query1 = _dbContext.AttractionAddresses
            .Where(i => i.AddressId == id);

        var item = await query1.FirstOrDefaultAsync<AttractionAddressesDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.AttractionAddresses.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return new ResponseItemDto<IAttractionAddresses>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = item
        };
    }

    public async Task<ResponseItemDto<IAttractionAddresses>> UpdateAttractionAddressAsync(AttractionAddressesCuDto itemDto)
    {
        var query1 = _dbContext.AttractionAddresses
            .Where(i => i.AddressId == itemDto.AddressId);
        var item = await query1
                // .Include(i => i.ReviewsDbM)
                .FirstOrDefaultAsync<AttractionAddressesDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {itemDto.AddressId} is not existing");

        //I cannot have duplicates in the AttractionAddresses table, so check that
        // var query2 = _dbContext.AttractionAddresses
        //     .Where(i => i.Email == itemDto.Email);
        // var existingItem = await query2.FirstOrDefaultAsync();
        // if (existingItem != null && existingItem.AddressId != itemDto.AddressId)
        //     throw new ArgumentException($"Item already exist with id {existingItem.AddressId}");

        //transfer any changes from DTO to database objects
        //Update individual properties
        item.UpdateFromDTO(itemDto);

        //Update navigation properties
        await navProp_AttractionAddressesCUdto_to_AttractionAddressesDbM(itemDto, item);

        //write to database model
        _dbContext.AttractionAddresses.Update(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadAttractionAddressAsync(item.AddressId, false);
    }

    public async Task<ResponseItemDto<IAttractionAddresses>> CreateAttractionAddressAsync(AttractionAddressesCuDto itemDto)
    {
        if (itemDto.AddressId != null)
            throw new ArgumentException($"{nameof(itemDto.AddressId)} must be null when creating a new object");

        //I cannot have duplicates in the AttractionAddresses table, so check that
        // var query2 = _dbContext.AttractionAddresses
        //     .Where(i => i.Email == itemDto.Email);
        // var existingItem = await query2.FirstOrDefaultAsync();
        // if (existingItem != null && existingItem.AddressId != itemDto.AddressId)
        //     throw new ArgumentException($"Item already exist with id {existingItem.AddressId}");

        //transfer any changes from DTO to database objects
        //Update individual properties
        var item = new AttractionAddressesDbM(itemDto);

        //Update navigation properties
        await navProp_AttractionAddressesCUdto_to_AttractionAddressesDbM(itemDto, item);

        //write to database model
        _dbContext.AttractionAddresses.Add(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadAttractionAddressAsync(item.AddressId, false);
    }

    private async Task navProp_AttractionAddressesCUdto_to_AttractionAddressesDbM(AttractionAddressesCuDto itemDtoSrc, AttractionAddressesDbM itemDst)
    {
        //update FriendsDbM from itemDto.FriendId
        List<AttractionsDbM> attractions = null;
        if (itemDtoSrc.AttractionId != null)
        {
            attractions = new List<AttractionsDbM>();
            foreach (var id in itemDtoSrc.AttractionId)
            {
                var f = await _dbContext.Attractions.FirstOrDefaultAsync(i => i.AttractionId == id);
                if (f == null)
                    throw new ArgumentException($"Item id {id} not existing");

                attractions.Add(f);
            }
        }
        itemDst.AttractionsDbM = attractions;
    }
}
