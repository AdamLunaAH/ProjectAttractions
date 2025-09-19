using Models;
using Models.DTO;

namespace Services;

public interface IUsersService
{
    public Task<ResponsePageDto<IUsers>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
}
