using Models;
using Models.DTO;

namespace Services;

public interface ICategoriesService
{
    public Task<ResponsePageDto<ICategories>> ReadCategoriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
}
