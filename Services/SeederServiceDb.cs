using Microsoft.Extensions.Logging;

using DbRepos;

namespace Services;

public class SeederServiceDb : ISeederService
{
    private readonly SeederDbRepos _repo = null;
    private readonly ILogger<SeederServiceDb> _logger = null;

    public Task SeedAllAsync() => _repo.SeedAllAsync();

    #region constructors
    public SeederServiceDb(SeederDbRepos repo)
    {
        _repo = repo;
    }
    public SeederServiceDb(SeederDbRepos repo, ILogger<SeederServiceDb> logger):this(repo)
    {
        _logger = logger;
    }
    #endregion
}

