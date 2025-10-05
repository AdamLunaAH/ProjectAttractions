using Models.DTO;

namespace Services;

public interface ISeederService
{
    public Task<ResponseItemDto<SupUsrInfoAllDto>> SeedAllAsync();
}


