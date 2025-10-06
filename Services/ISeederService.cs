using Models.DTO;

namespace Services;

public interface ISeederService
{
    public Task<ResponseItemDto<SupUsrInfoAllDto>> SeedAllAsync();
    public Task<ResponseItemDto<string>> RemoveSeededDataAsync();
    public Task<ResponseItemDto<string>> RemoveAllDataAsync();
}


