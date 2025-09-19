namespace Models;

public interface IAttractionAddresses
{
    public Guid AddressId { get; set; }
    public string StreetAddress { get; set; }
    public string ZipCode { get; set; }
    public string CityPlace { get; set; }
    public string Country { get; set; }

    public IAttractions Attractions { get; set; }
}