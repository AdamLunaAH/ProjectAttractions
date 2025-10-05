using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
using DbRepos;

namespace Services;

public class AttractionsServiceDb : IAttractionsService
{
    private readonly AttractionsDbRepos _repo = null;
    private readonly ILogger<AttractionsServiceDb> _logger = null;

    public AttractionsServiceDb(AttractionsDbRepos repo)
    {
        _repo = repo;
    }
    public AttractionsServiceDb(AttractionsDbRepos repo, ILogger<AttractionsServiceDb> logger) : this(repo)
    {
        _logger = logger;
    }

    public Task<ResponsePageDto<IAttractions>> ReadAttractionsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAttractionsAsync(seeded, flat, filter, pageNumber, pageSize);

    public Task<ResponsePageDto<IAttractions>> ReadAttractionsNoAddressAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize, bool noAddress) => _repo.ReadAttractionsNoAddressAsync(seeded, flat, filter, pageNumber, pageSize, noAddress);

    public Task<ResponsePageDto<IAttractions>> ReadAttractionsNoReviewsAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize, bool noReview) => _repo.ReadAttractionsNoReviewsAsync(seeded, flat, filter, pageNumber, pageSize, noReview);

    public Task<ResponseItemDto<IAttractions>> ReadAttractionAsync(Guid id, bool flat) => _repo.ReadAttractionAsync(id, flat);

    public Task<ResponseItemDto<IAttractions>> DeleteAttractionAsync(Guid id) => _repo.DeleteAttractionAsync(id);
    public Task<ResponseItemDto<IAttractions>> UpdateAttractionAsync(AttractionsCuDto item) => _repo.UpdateAttractionAsync(item);
    public Task<ResponseItemDto<IAttractions>> CreateAttractionAsync(AttractionCreateDto item) => _repo.CreateAttractionAsync(item);
    public async Task<ResponseItemDto<IAttractions>> CreateFullAttractionAsync(AttractionFullCreateDto dto)
    {
        return await _repo.CreateFullAttractionAsync(dto);
    }


}

