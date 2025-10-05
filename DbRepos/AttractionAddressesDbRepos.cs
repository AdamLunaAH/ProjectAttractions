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

    public async Task<ResponsePageDto<IAttractionAddresses>> ReadAttractionAddressesAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        try
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
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                            (
                                i.Country.ToLower().Contains(filter) ||
                                i.CityPlace.ToLower().Contains(filter) ||
                                i.ZipCode.ToLower().Contains(filter) ||
                                i.StreetAddress.ToLower().Contains(filter)
                            ))
                .CountAsync(),

                PageItems = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                            (
                                i.Country.ToLower().Contains(filter) ||
                                i.CityPlace.ToLower().Contains(filter) ||
                                i.ZipCode.ToLower().Contains(filter) ||
                                i.StreetAddress.ToLower().Contains(filter)
                            ))

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .ToListAsync<IAttractionAddresses>(),

                PageNr = pageNumber,
                PageSize = pageSize
            };
            return ret;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read attraction addresses due to an unexpected error.");
            return new ResponsePageDto<IAttractionAddresses>();
        }
    }

    public async Task<ResponseItemDto<IAttractionAddresses>> ReadAttractionAddressAsync(Guid id, bool flat)
    {
        try
        {
            if (!flat)
            {
                //make sure the model is fully populated, try without include.
                //remove tracking for all read operations for performance and to avoid recursion/circular access

                var query = _dbContext.AttractionAddresses.AsNoTracking()
                    .Include(i => i.AttractionsDbM)
                    .Where(i => i.AddressId == id);

                var item = await query.FirstOrDefaultAsync<AttractionAddressesDbM>();
                if (item == null)
                {
                    return new ResponseItemDto<IAttractionAddresses>
                    {
                        ErrorMessage = $"Could not find item with id {id}"

                    };
                }

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
        catch (Exception ex)
        {
            return new ResponseItemDto<IAttractionAddresses>
            {
                ErrorMessage = $"Could not read attraction address due to an unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<ResponseItemDto<IAttractionAddresses>> DeleteAttractionAddressAsync(Guid id)
    {
        try
        {
            var query = _dbContext.AttractionAddresses
            .Where(i => i.AddressId == id);


            //If the item does not exists
            var item = await query.FirstOrDefaultAsync<AttractionAddressesDbM>();
            if (item == null)
            {
                return new ResponseItemDto<IAttractionAddresses>
                {
                    ErrorMessage = $"Could not find item with id {id}"
                };
            }

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
        catch (Exception ex)
        {
            return new ResponseItemDto<IAttractionAddresses>()
            {
                ErrorMessage = $"Could not delete attraction address due to an unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<ResponseItemDto<IAttractionAddresses>> UpdateAttractionAddressAsync(AttractionAddressesCuDto itemDto)
    {
        try
        {
            var query = _dbContext.AttractionAddresses
            .Where(i => i.AddressId == itemDto.AddressId);
            //If the item does not exists
            var item = await query.FirstOrDefaultAsync<AttractionAddressesDbM>();
            if (item == null)
            {
                return new ResponseItemDto<IAttractionAddresses>
                {
                    ErrorMessage = $"Could not find item with id {itemDto.AddressId}"
                };
            }

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
        catch (Exception ex)
        {
            return new ResponseItemDto<IAttractionAddresses>
            {
                ErrorMessage = $"Could not update attraction address due to an unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<ResponseItemDto<IAttractionAddresses>> CreateAttractionAddressAsync(AttractionAddressCreateDto itemDto)
    {
        try
        {
            //transfer any changes from DTO to database objects
            //Update individual properties
            // var item = new AttractionAddressesDbM(itemDto);
            var duplicate = await _dbContext.AttractionAddresses
                .FirstOrDefaultAsync(a =>
                    a.StreetAddress == itemDto.StreetAddress &&
                    a.ZipCode == itemDto.ZipCode &&
                    a.CityPlace == itemDto.CityPlace &&
                    a.Country == itemDto.Country);

            if (duplicate != null)
            {
                return new ResponseItemDto<IAttractionAddresses>
                {
                    ErrorMessage = $"An attraction with the same name, description, and address already exists (Id: {duplicate.AddressId})"
                };
            }

            var item = new AttractionAddressesDbM
            {
                AddressId = Guid.NewGuid(),
                StreetAddress = itemDto.StreetAddress,
                ZipCode = itemDto.ZipCode,
                CityPlace = itemDto.CityPlace,
                Country = itemDto.Country

            };

            //write to database model
            _dbContext.AttractionAddresses.Add(item);

            //write to database in a UoW
            await _dbContext.SaveChangesAsync();

            //return the updated item in non-flat mode
            return await ReadAttractionAddressAsync(item.AddressId, false);
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IAttractionAddresses>
            {
                ErrorMessage = $"Could not create attraction address due to an unexpected error: {ex.Message}"
            };
        }
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
