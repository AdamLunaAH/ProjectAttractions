using Models;
using Models.DTO;

namespace Services;

public interface IAttractionsService
{
    public Task<ResponsePageDto<IAttractions>> ReadAttractionsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize);

    public Task<ResponsePageDto<IAttractions>> ReadAttractionsNoAddressAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize, bool noAddress = false);

    public Task<ResponsePageDto<IAttractions>> ReadAttractionsNoReviewsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize, bool noReview = false);

    public Task<ResponseItemDto<IAttractions>> ReadAttractionAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IAttractions>> DeleteAttractionAsync(Guid id);
    public Task<ResponseItemDto<IAttractions>> UpdateAttractionAsync(AttractionsCuDto item);
    public Task<ResponseItemDto<IAttractions>> CreateAttractionAsync(AttractionsCuDto item);
}


