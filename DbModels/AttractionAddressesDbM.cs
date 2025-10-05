using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Models.Utilities.SeedGenerator;
using Models;
using Models.DTO;

namespace DbModels;

[Table("AttractionAddressesDb", Schema = "supusr")]
[Index(nameof(StreetAddress), nameof(ZipCode), nameof(CityPlace), nameof(Country), IsUnique = true)]


sealed public class AttractionAddressesDbM : AttractionAddresses, ISeed<AttractionAddressesDbM>, IEquatable<AttractionAddressesDbM>
{
    [Key]
    public override Guid AddressId { get; set; }

    [Required]
    public override string StreetAddress { get; set; }
    [Required]
    public override string ZipCode { get; set; }
    [Required]
    public override string CityPlace { get; set; }
    [Required]
    public override string Country { get; set; }

    #region implementing IEquatable
    public bool Equals(AttractionAddressesDbM other) => (other != null) && ((StreetAddress, ZipCode, CityPlace, Country) ==
        (other.StreetAddress, other.ZipCode, other.CityPlace, other.Country));

    public override bool Equals(object obj) => Equals(obj as AttractionAddressesDbM);
    public override int GetHashCode() => (StreetAddress, ZipCode, CityPlace, Country).GetHashCode();
    #endregion

    #region correcting the Navigation properties migration error caused by using interfaces
    [NotMapped]
    public override List<IAttractions> Attractions { get => AttractionsDbM?.Cast<IAttractions>().ToList(); set => throw new NotImplementedException(); }
    [NotMapped]
    public List<AttractionsDbM> AttractionsDbM { get; set; } = new();
    #endregion

    #region randomly seed this instance
    public override AttractionAddressesDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
    #endregion

    #region Update from DTO
    public AttractionAddressesDbM UpdateFromDTO(AttractionAddressesCuDto org)
    {
        if (org == null) return null;
        StreetAddress = org.StreetAddress;
        ZipCode = org.ZipCode;
        CityPlace = org.CityPlace;
        Country = org.Country;

        return this;
    }
    #endregion

    #region constructors
    public AttractionAddressesDbM() { }
    public AttractionAddressesDbM(AttractionAddressesCuDto org)
    {
        AddressId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
    #endregion
}