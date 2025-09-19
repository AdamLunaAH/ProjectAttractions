using Models;
using Models.DTO;

namespace Services;

public interface IAttractionsService
{
    public Task<ResponsePageDto<IAttractions>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);

    public Task<ResponsePageDto<IAttractions>> ReadAttractionsNoReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
}


