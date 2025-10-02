using Models;
using Models.DTO;

namespace Services;

public interface IAttractionAddressesService
{
    public Task<ResponsePageDto<IAttractionAddresses>> ReadAttractionAddressesAsync(bool? seeded, bool flat, string filter, int pageNumber, int pageSize);

    public Task<ResponseItemDto<IAttractionAddresses>> ReadAttractionAddressAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IAttractionAddresses>> DeleteAttractionAddressAsync(Guid id);
    public Task<ResponseItemDto<IAttractionAddresses>> UpdateAttractionAddressAsync(AttractionAddressesCuDto item);
    public Task<ResponseItemDto<IAttractionAddresses>> CreateAttractionAddressAsync(AttractionAddressCreateDto item);
}
