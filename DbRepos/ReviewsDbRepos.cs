using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class ReviewsDbRepos
{
    private ILogger<ReviewsDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public ReviewsDbRepos(ILogger<ReviewsDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IReviews>> ReadReviewsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        try
        {
            filter ??= "";
            IQueryable<ReviewsDbM> query;
            if (flat)
            {
                query = _dbContext.Reviews.AsNoTracking();
            }
            else
            {
                query = _dbContext.Reviews.AsNoTracking()
                    .Include(i => i.AttractionsDbM)
                    .Include(i => i.UsersDbM);
            }

            var ret = new ResponsePageDto<IReviews>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif
                DbItemsCount = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                            (i.ReviewScore.ToString().Contains(filter) ||
                                i.ReviewText.ToLower().Contains(filter))).CountAsync(),

                PageItems = await query

                //Adding filter functionality
                .Where(i => (seeded == null || i.Seeded == seeded) &&
                            (i.ReviewScore.ToString().Contains(filter) ||
                                i.ReviewText.ToLower().Contains(filter)))

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .ToListAsync<IReviews>(),

                PageNr = pageNumber,
                PageSize = pageSize
            };
            return ret;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read reviews due to an unexpected error.");
            return new ResponsePageDto<IReviews>();
        }
    }

    public async Task<ResponseItemDto<IReviews>> ReadReviewAsync(Guid id, bool flat)
    {
        try
        {
            if (!flat)
            {
                //make sure the model is fully populated, try without include.
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var query = _dbContext.Reviews.AsNoTracking()
                    .Include(i => i.UsersDbM)
                    .Include(i => i.AttractionsDbM)
                    .Where(i => i.ReviewId == id);

                return new ResponseItemDto<IReviews>()
                {
#if DEBUG
                    ConnectionString = _dbContext.dbConnection,
#endif

                    Item = await query.FirstOrDefaultAsync<IReviews>()
                };
            }
            else
            {
                //Not fully populated, compare the SQL Statements generated
                //remove tracking for all read operations for performance and to avoid recursion/circular access
                var query = _dbContext.Reviews.AsNoTracking()
                    .Include(i => i.UsersDbM)
                    .Include(i => i.AttractionsDbM);

                return new ResponseItemDto<IReviews>()
                {
#if DEBUG
                    ConnectionString = _dbContext.dbConnection,
#endif

                    Item = await query.FirstOrDefaultAsync<IReviews>()
                };
            }
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IReviews>()
            {
                ErrorMessage = $"Could not read review due to an unexpected error: {ex.Message}"
            };
        }
    }


    public async Task<ResponseItemDto<IReviews>> DeleteReviewAsync(Guid id)
    {
        try
        {
            var query1 = _dbContext.Reviews
            .Where(i => i.ReviewId == id);

            var item = await query1.FirstOrDefaultAsync<ReviewsDbM>();

            //If the item does not exists
            if (item == null) throw new ArgumentException($"Item {id} does not exist");

            //delete in the database model
            _dbContext.Reviews.Remove(item);

            //write to database in a UoW
            await _dbContext.SaveChangesAsync();
            return new ResponseItemDto<IReviews>()
            {
#if DEBUG
                ConnectionString = _dbContext.dbConnection,
#endif

                Item = item
            };
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IReviews>()
            {
                ErrorMessage = $"Could not delete review due to an unexpected error: {ex.Message}"
            };
        }

    }

    public async Task<ResponseItemDto<IReviews>> UpdateReviewAsync(ReviewsCuDto itemDto)
    {
        try
        {
            var query1 = _dbContext.Reviews
            .Where(i => i.ReviewId == itemDto.ReviewId);
            var item = await query1
                    // .Include(i => i.ReviewsDbM)
                    .FirstOrDefaultAsync<ReviewsDbM>();

            //If the item does not exists
            if (item == null) throw new ArgumentException($"Item {itemDto.ReviewId} does not exist");

            //I cannot have duplicates in the Reviews table, so check that
            var exists = await _dbContext.Reviews
            .AnyAsync(r => r.UserId == itemDto.UserId &&
                            r.AttractionId == itemDto.AttractionId &&
                            r.ReviewId != itemDto.ReviewId);

            if (exists == true)
            {
                return new ResponseItemDto<IReviews>()
                {
                    ErrorMessage = $"User {itemDto.UserId} already has a review for attraction {itemDto.AttractionId}"
                };
            }


            //transfer any changes from DTO to database objects
            //Update individual properties
            item.UpdateFromDTO(itemDto);

            //Update navigation properties
            await navProp_ReviewsCUdto_to_ReviewsDbM(itemDto, item);

            //write to database model
            _dbContext.Reviews.Update(item);

            //write to database in a UoW
            await _dbContext.SaveChangesAsync();

            //return the updated item in non-flat mode
            return await ReadReviewAsync(item.ReviewId, false);
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IReviews>()
            {
                ErrorMessage = $"Could not update review due to an unexpected error: {ex.Message}"
            };
        }
    }
    public async Task<ResponseItemDto<IReviews>> CreateReviewAsync(ReviewCreateDto itemDto)
    {
        try
        {
            if (itemDto.UserId == null || itemDto.UserId == Guid.Empty)
                throw new ArgumentException("UserId must be provided");
            if (itemDto.AttractionId == null || itemDto.AttractionId == Guid.Empty)
                throw new ArgumentException("AttractionId must be provided");

            var exists = await _dbContext.Reviews
                .AnyAsync(r => r.UserId == itemDto.UserId &&
                                r.AttractionId == itemDto.AttractionId);


            if (exists)
            {
                return new ResponseItemDto<IReviews>
                {
                    ErrorMessage = $"A review with the same user {itemDto.UserId} and attraction {itemDto.AttractionId} already exists"
                };
            }
            // throw new ArgumentException(
            //     $"User {itemDto.UserId} already has a review for attraction {itemDto.AttractionId}");

            var item = new ReviewsDbM
            {
                ReviewId = Guid.NewGuid(),
                UserId = itemDto.UserId.Value,
                AttractionId = itemDto.AttractionId.Value,
                ReviewScore = itemDto.ReviewScore,
                ReviewText = itemDto.ReviewText,
                CreatedAt = itemDto.CreatedAt ?? DateTime.UtcNow
            };

            await navProp_ReviewsCUdto_to_ReviewsDbM(itemDto, item);

            _dbContext.Reviews.Add(item);
            await _dbContext.SaveChangesAsync();

            return await ReadReviewAsync(item.ReviewId, false);
        }
        catch (Exception ex)
        {
            return new ResponseItemDto<IReviews>()
            {
                ErrorMessage = $"Could not create review due to an unexpected error: {ex.Message}"
            };
        }
    }

    private async Task navProp_ReviewsCUdto_to_ReviewsDbM(ReviewCreateDto itemDtoSrc, ReviewsDbM itemDst)
    {
        // Set the User navigation property
        if (itemDtoSrc.UserId.HasValue && itemDtoSrc.UserId != Guid.Empty)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == itemDtoSrc.UserId.Value);

            if (user == null)
                throw new ArgumentException($"User id {itemDtoSrc.UserId} does not exist");

            itemDst.UsersDbM = user;
        }
        else
        {
            itemDst.UsersDbM = null;
        }

        // Set the Attraction navigation property
        if (itemDtoSrc.AttractionId.HasValue && itemDtoSrc.AttractionId != Guid.Empty)
        {
            var attraction = await _dbContext.Attractions
                .FirstOrDefaultAsync(a => a.AttractionId == itemDtoSrc.AttractionId.Value);

            if (attraction == null)
                throw new ArgumentException($"Attraction id {itemDtoSrc.AttractionId} does not exist");

            itemDst.AttractionsDbM = attraction;
        }
        else
        {
            itemDst.AttractionsDbM = null;
        }
    }

    private async Task navProp_ReviewsCUdto_to_ReviewsDbM(ReviewsCuDto itemDtoSrc, ReviewsDbM itemDst)
    {
        //update FriendsDbM from itemDto.FriendId
        if (itemDtoSrc.UserId.HasValue)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == itemDtoSrc.UserId.Value);

            if (user == null)
                throw new ArgumentException(
                    $"User id {itemDtoSrc.UserId} does not exist");

            itemDst.UsersDbM = user;
        }
        else
        {
            itemDst.UsersDbM = null;
        }

        if (itemDtoSrc.AttractionId.HasValue)
        {
            var attraction = await _dbContext.Attractions
                .FirstOrDefaultAsync(a => a.AttractionId == itemDtoSrc.AttractionId.Value);

            if (attraction == null)
                throw new ArgumentException(
                    $"Attraction id {itemDtoSrc.AttractionId} does not exist");

            itemDst.AttractionsDbM = attraction;
        }
        else
        {
            itemDst.AttractionsDbM = null;
        }
    }
}
