using Models;
using Models.DTO;

namespace Services;

public interface IReviewsService
{
    public Task<ResponsePageDto<IReviews>> ReadReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
}
