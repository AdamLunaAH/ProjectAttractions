using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
using DbRepos;

namespace Services;

public class UsersServiceDb : IUsersService
{
    private readonly UsersDbRepos _repo = null;
    private readonly ILogger<UsersServiceDb> _logger = null;


    public UsersServiceDb(UsersDbRepos repo)
    {
        _repo = repo;
    }
    public UsersServiceDb(UsersDbRepos repo, ILogger<UsersServiceDb> logger) : this(repo)
    {
        _logger = logger;
    }

    //Simple 1:1 calls in this case, but as Services expands, this will no longer need to be the case
    public Task<ResponsePageDto<IUsers>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadUsersAsync(seeded, flat, filter, pageNumber, pageSize);
}

