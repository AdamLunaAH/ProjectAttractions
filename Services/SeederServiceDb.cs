using Microsoft.Extensions.Logging;

using DbRepos;
using Models.DTO;

namespace Services;

public class SeederServiceDb : ISeederService
{
    private readonly SeederDbRepos _repo = null;
    private readonly ILogger<SeederServiceDb> _logger = null;

    public Task<ResponseItemDto<SupUsrInfoAllDto>> SeedAllAsync() => _repo.SeedAllAsync();
    public Task<ResponseItemDto<string>> RemoveSeededDataAsync() => _repo.RemoveSeededDataAsync();
    public Task<ResponseItemDto<string>> RemoveAllDataAsync() => _repo.RemoveAllDataAsync();

    #region constructors
    public SeederServiceDb(SeederDbRepos repo)
    {
        _repo = repo;
    }
    public SeederServiceDb(SeederDbRepos repo, ILogger<SeederServiceDb> logger) : this(repo)
    {
        _logger = logger;
    }
    #endregion
}


