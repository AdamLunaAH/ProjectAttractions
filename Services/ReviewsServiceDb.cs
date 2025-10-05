using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
using DbRepos;

namespace Services;

public class ReviewsServiceDb : IReviewsService
{
    private readonly ReviewsDbRepos _repo = null;
    private readonly ILogger<ReviewsServiceDb> _logger = null;


    public ReviewsServiceDb(ReviewsDbRepos repo)
    {
        _repo = repo;
    }
    public ReviewsServiceDb(ReviewsDbRepos repo, ILogger<ReviewsServiceDb> logger) : this(repo)
    {
        _logger = logger;
    }

    public Task<ResponsePageDto<IReviews>> ReadReviewsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadReviewsAsync(seeded, flat, filter, pageNumber, pageSize);

    public Task<ResponseItemDto<IReviews>> ReadReviewAsync(Guid id, bool flat) => _repo.ReadReviewAsync(id, flat);

    public Task<ResponseItemDto<IReviews>> DeleteReviewAsync(Guid id) => _repo.DeleteReviewAsync(id);
    public Task<ResponseItemDto<IReviews>> UpdateReviewAsync(ReviewsCuDto item) => _repo.UpdateReviewAsync(item);
    public Task<ResponseItemDto<IReviews>> CreateReviewAsync(ReviewCreateDto item)
    => _repo.CreateReviewAsync(item);

}

