using Models;
using Models.DTO;

namespace Services;

public interface IReviewsService
{
    public Task<ResponsePageDto<IReviews>> ReadReviewsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize);

    public Task<ResponseItemDto<IReviews>> ReadReviewAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IReviews>> DeleteReviewAsync(Guid id);
    public Task<ResponseItemDto<IReviews>> UpdateReviewAsync(ReviewsCuDto item);
    // public Task<ResponseItemDto<IReviews>> CreateReviewAsync(ReviewsCuDto item);
    Task<ResponseItemDto<IReviews>> CreateReviewAsync(ReviewCreateDto item);

}
