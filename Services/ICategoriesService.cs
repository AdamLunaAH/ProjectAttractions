using Models;
using Models.DTO;

namespace Services;

public interface ICategoriesService
{
    public Task<ResponsePageDto<ICategories>> ReadCategoriesAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<ICategories>> ReadCategoryAsync(Guid id, bool flat);
    public Task<ResponseItemDto<ICategories>> DeleteCategoryAsync(Guid id);
    public Task<ResponseItemDto<ICategories>> UpdateCategoryAsync(CategoriesCuDto item);
    public Task<ResponseItemDto<ICategories>> CreateCategoryAsync(CategoryCreateDto item);
}
