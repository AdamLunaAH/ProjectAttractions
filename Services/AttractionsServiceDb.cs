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

    //Simple 1:1 calls in this case, but as Services expands, this will no longer need to be the case
    public Task<ResponsePageDto<IAttractions>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAttractionsAsync(seeded, flat, filter, pageNumber, pageSize);

    public Task<ResponsePageDto<IAttractions>> ReadAttractionsNoReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAttractionsNoReviewsAsync(seeded, flat, filter, pageNumber, pageSize);
}

