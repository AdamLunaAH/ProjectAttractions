using Models;
using Models.DTO;

namespace Services;

public interface IUsersService
{
    public Task<ResponsePageDto<IUsers>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);

    public Task<ResponsePageDto<IUsers>> ReadUsersReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);

    public Task<ResponseItemDto<IUsers>> ReadUserAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IUsers>> DeleteUserAsync(Guid id);
    public Task<ResponseItemDto<IUsers>> UpdateUserAsync(UsersCuDto item);
    public Task<ResponseItemDto<IUsers>> CreateUserAsync(UsersCuDto item);
}
