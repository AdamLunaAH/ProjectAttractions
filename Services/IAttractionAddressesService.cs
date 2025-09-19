using Models;
using Models.DTO;

namespace Services;

public interface IAttractionAddressesService
{
    public Task<ResponsePageDto<IAttractionAddresses>> ReadAttractionAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
}
