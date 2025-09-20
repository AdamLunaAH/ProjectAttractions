using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
using DbRepos;

namespace Services;

public class AttractionAddressesServiceDb : IAttractionAddressesService
{
    private readonly AttractionAddressesDbRepos _repo = null;
    private readonly ILogger<AttractionAddressesServiceDb> _logger = null;


    public AttractionAddressesServiceDb(AttractionAddressesDbRepos repo)
    {
        _repo = repo;
    }
    public AttractionAddressesServiceDb(AttractionAddressesDbRepos repo, ILogger<AttractionAddressesServiceDb> logger) : this(repo)
    {
        _logger = logger;
    }

    //Simple 1:1 calls in this case, but as Services expands, this will no longer need to be the case
    public Task<ResponsePageDto<IAttractionAddresses>> ReadAttractionAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAttractionAddressesAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IAttractionAddresses>> ReadAttractionAddressAsync(Guid id, bool flat) => _repo.ReadAttractionAddressAsync(id, flat);

    public Task<ResponseItemDto<IAttractionAddresses>> DeleteAttractionAddressAsync(Guid id) => _repo.DeleteAttractionAddressAsync(id);
    public Task<ResponseItemDto<IAttractionAddresses>> UpdateAttractionAddressAsync(AttractionAddressesCuDto item) => _repo.UpdateAttractionAddressAsync(item);
    public Task<ResponseItemDto<IAttractionAddresses>> CreateAttractionAddressAsync(AttractionAddressesCuDto item) => _repo.CreateAttractionAddressAsync(item);
}

