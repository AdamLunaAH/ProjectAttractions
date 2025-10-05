using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class UsersDbRepos
{
    private ILogger<UsersDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public UsersDbRepos(ILogger<UsersDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IUsers>> ReadUsersAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        try
        {
            filter ??= "";
            IQueryable<UsersDbM> query;
            if (flat)
            {
                query = _dbContext.Users.AsNoTracking();
            }
            else
            {
                query = _dbContext.Users.AsNoTracking()
                    .Include(i => i.ReviewsDbM);
            }

            var ret = new ResponsePageDto<IUsers>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif
                DbItemsCount = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                    (i.FirstName.ToLower().Contains(filter) ||
                    i.LastName.ToLower().Contains(filter)))
                    .CountAsync(),

                PageItems = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                    (i.FirstName.ToLower().Contains(filter) ||
                    i.LastName.ToLower().Contains(filter)))

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .ToListAsync<IUsers>(),

                PageNr = pageNumber,
                PageSize = pageSize
            };
            return ret;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read users due to an unexpected error.");
            return new ResponsePageDto<IUsers>();
        }
    }

    public async Task<ResponsePageDto<IUsers>> ReadUsersReviewsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        try
        {
            filter ??= "";
            IQueryable<UsersDbM> query;
            if (flat)
            {
                query = _dbContext.Users.AsNoTracking();
            }
            else
            {
                query = _dbContext.Users.AsNoTracking()
                    .Include(i => i.ReviewsDbM);
            }

            var ret = new ResponsePageDto<IUsers>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif
                DbItemsCount = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                            (i.FirstName.ToLower().Contains(filter) ||
                                i.LastName.ToLower().Contains(filter))).CountAsync(),

                PageItems = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                            (i.FirstName.ToLower().Contains(filter) ||
                                i.LastName.ToLower().Contains(filter)))

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .ToListAsync<IUsers>(),

                PageNr = pageNumber,
                PageSize = pageSize
            };
            return ret;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read user reviews due to an unexpected error.");
            return new ResponsePageDto<IUsers>();
        }
    }

    public async Task<ResponseItemDto<IUsers>> ReadUserAsync(Guid id, bool flat)
    {
        try
        {
            if (!flat)
        {
            //make sure the model is fully populated, try without include.
            //remove tracking for all read operations for performance and to avoid recursion/circular access
            var query = _dbContext.Users.AsNoTracking()
                .Include(i => i.ReviewsDbM)
                .Where(i => i.UserId == id);

            return new ResponseItemDto<IUsers>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = await query.FirstOrDefaultAsync<IUsers>()
            };
        }
        else
        {
            //Not fully populated, compare the SQL Statements generated
            //remove tracking for all read operations for performance and to avoid recursion/circular access
            var query = _dbContext.Users.AsNoTracking()
                .Where(i => i.UserId == id);

            return new ResponseItemDto<IUsers>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = await query.FirstOrDefaultAsync<IUsers>()
            };
        }
    }
    catch (Exception ex)
        {
            return new ResponseItemDto<IUsers>
            {
                ErrorMessage = $"Could not read user due to an unexpected error: {ex.Message}"
            };
        }
    }


    public async Task<ResponseItemDto<IUsers>> DeleteUserAsync(Guid id)
    {
        try
        {
            var query1 = _dbContext.Users
            .Where(i => i.UserId == id);

            var item = await query1.FirstOrDefaultAsync<UsersDbM>();

            //If the item does not exists
            if (item == null) throw new ArgumentException($"Item {id} does not exist");

            //delete in the database model
            _dbContext.Users.Remove(item);

            //write to database in a UoW
            await _dbContext.SaveChangesAsync();
            return new ResponseItemDto<IUsers>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = item
            };
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IUsers>()
            {
                ErrorMessage = $"Could not delete user due to an unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<ResponseItemDto<IUsers>> UpdateUserAsync(UsersCuDto itemDto)
    {
        // var query1 = _dbContext.Users
        //     .Where(i => i.UserId == itemDto.UserId);
        // var item = await query1
        //         .FirstOrDefaultAsync<UsersDbM>();

        try
        {
            var item = await _dbContext.Users
                .FirstOrDefaultAsync(i => i.UserId == itemDto.UserId);

            //If the item does not exists
            if (item == null) throw new ArgumentException($"Item {itemDto.UserId} does not exist");

            //I cannot have duplicates in the Users table, so check that
            // var query2 = _dbContext.Users
            //     .Where(i => i.Email == itemDto.Email);
            // var existingItem = await query2.FirstOrDefaultAsync();
            // if (existingItem != null && existingItem.UserId != itemDto.UserId)
            //     throw new ArgumentException($"Item already exist with id {existingItem.UserId}");
            var existingItem = await _dbContext.Users
                .FirstOrDefaultAsync(i => i.Email == itemDto.Email);

            if (existingItem != null)
            {
                return new ResponseItemDto<IUsers>
                {
                    ErrorMessage = $"A user with the same email already exists (Id: {existingItem.UserId})"
                };
            }

            //transfer any changes from DTO to database objects
            //Update individual properties
            item.UpdateFromDTO(itemDto);

            //Update navigation properties
            await navProp_UsersCUdto_to_UsersDbM(itemDto, item);

            //transfer any changes from DTO to database objects
            //Update individual properties
            item.UpdateFromDTO(itemDto);

            //Update navigation properties
            await navProp_UsersCUdto_to_UsersDbM(itemDto, item);

            //write to database model
            _dbContext.Users.Update(item);

            //write to database in a UoW
            await _dbContext.SaveChangesAsync();

            //return the updated item in non-flat mode
            return await ReadUserAsync(item.UserId, true);
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IUsers>
            {
                ErrorMessage = $"Could not update user due to an unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<ResponseItemDto<IUsers>> CreateUserAsync(UserCreateDto itemDto)
    {
        try
        {

        //I cannot have duplicates in the Users table, so check that
        var query2 = _dbContext.Users
            .Where(i => i.Email == itemDto.Email);
        var existingItem = await query2.FirstOrDefaultAsync();
        if (existingItem != null && existingItem.Email != itemDto.Email)
            throw new ArgumentException($"User already exist with the Email: {existingItem.Email}");

        //transfer any changes from DTO to database objects
        //Update individual properties
        var item = new UsersDbM
        {
            UserId = Guid.NewGuid(),
            FirstName = itemDto.FirstName,
            LastName = itemDto.LastName,
            Email = itemDto.Email
            ,
            CreatedAt = DateTime.UtcNow
        };

        //Update navigation properties
        await navProp_UsersCUdto_to_UsersDbM(itemDto, item);

        //write to database model
        _dbContext.Users.Add(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadUserAsync(item.UserId, false);
    }
        catch (Exception ex)
        {
            return new ResponseItemDto<IUsers>
            {
                ErrorMessage = $"Could not create user due to an unexpected error: {ex.Message}"
            };
        }
    }

    private async Task navProp_UsersCUdto_to_UsersDbM(UserCreateDto itemDtoSrc, UsersDbM itemDst)
    {

        List<ReviewsDbM> reviews = null;

        await Task.Run(() => itemDst.ReviewsDbM = reviews);
    }

    private async Task navProp_UsersCUdto_to_UsersDbM(UsersCuDto itemDtoSrc, UsersDbM itemDst)
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
    }

}
