using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
using DbRepos;

namespace Services;

public class CategoriesServiceDb : ICategoriesService
{
    private readonly CategoriesDbRepos _repo = null;
    private readonly ILogger<CategoriesServiceDb> _logger = null;

    public CategoriesServiceDb(CategoriesDbRepos repo)
    {
        _repo = repo;
    }
    public CategoriesServiceDb(CategoriesDbRepos repo, ILogger<CategoriesServiceDb> logger) : this(repo)
    {
        _logger = logger;
    }

    //Simple 1:1 calls in this case, but as Services expands, this will no longer need to be the case
    public Task<ResponsePageDto<ICategories>> ReadCategoriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadCategoriesAsync(seeded, flat, filter, pageNumber, pageSize);

    public Task<ResponseItemDto<ICategories>> ReadCategoryAsync(Guid id, bool flat) => _repo.ReadCategoryAsync(id, flat);

    public Task<ResponseItemDto<ICategories>> DeleteCategoryAsync(Guid id) => _repo.DeleteCategoryAsync(id);
    public Task<ResponseItemDto<ICategories>> UpdateCategoryAsync(CategoriesCuDto item) => _repo.UpdateCategoryAsync(item);
    public Task<ResponseItemDto<ICategories>> CreateCategoryAsync(CategoriesCuDto item) => _repo.CreateCategoryAsync(item);
}

