using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;
using Models.DTO;

namespace DbModels;

[Table("AttractionAddressesDb", Schema = "supusr")]
[Index(nameof(StreetAddress), nameof(ZipCode), nameof(CityPlace), nameof(Country), IsUnique = false)]


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
    //     // One-to-one relationship with AttractionDbM
    // [JsonIgnore] // Do not include in any JSON response from the WebApi
    // public AttractionDbM AttractionDbM { get; set; }
    // [ForeignKey(nameof(AddressId))]
    // public Guid AttractionDbMId { get; set; }

    //removed from EFC
    [NotMapped]
    public override List<IAttractions> Attractions { get => AttractionsDbM?.Cast<IAttractions>().ToList(); set => throw new NotImplementedException(); }
    // public override IAttractions Attractions { get => AttractionsDbM; set => new NotImplementedException(); }
    // public override List<IAttractions> Attractions { get => AttractionsDbM?.ToList<IAttractions>(); set => new NotImplementedException(); }
    //do not include in any json response from the WebApi
    [JsonIgnore]
    [NotMapped]
    public List<AttractionsDbM> AttractionsDbM { get; set; } = new();
    // public AttractionsDbM AttractionsDbM { get; set; } = null;
    // public List<AttractionsDbM> AttractionsDbM { get; set; } = null;
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



// public class AttractionAddressesDbM : AttractionAddresses, ISeed<AttractionAddressesDbM>, IEquatable<AttractionAddressesDbM>
// {

//     [Key]
//     public override Guid AddressId { get; set; }
//     // { get => base.AddressId; set => base.AddressId = value; }

//     public override string StreetAddress { get; set; }
//     public override string ZipCode { get; set; }
//     public override string CityPlace { get; set; }
//     public override string Country { get; set; }

//     #region implementing IEquatable
//     public bool Equals(AttractionAddressesDbM other) => (other != null) && ((StreetAddress, ZipCode, CityPlace, Country) ==
//         (other.StreetAddress, other.ZipCode, other.CityPlace, other.Country));

//     public override bool Equals(object obj) => Equals(obj as AttractionAddressesDbM);
//     public override int GetHashCode() => (StreetAddress, ZipCode, CityPlace, Country).GetHashCode();
//     #endregion

//     #region correcting the Navigation properties migration error caused by using interfaces
//     [NotMapped] //removed from EFC

//     [JsonIgnore] //do not include in any json response from the WebApi
//     public List<AttractionsDbM> AttractionsDbM { get; set; } = null;
//     #endregion

//     #region constructors
//     public AttractionAddressesDbM() { }
//     #endregion

//     public new AttractionAddressesDbM Seed(SeedGenerator seeder)
//     {
//         base.Seed(seeder);
//         return this;
//     }
// }

// // public class AttractionAddressesDbM : AttractionAddresses, ISeed<AttractionAddressesDbM>
// // {

// //     [Key]
// //     public override Guid AddressId { get; set; }
// //     // { get => base.AddressId; set => base.AddressId = value; }

// //     public new AttractionAddressesDbM Seed(SeedGenerator seeder)
// //     {
// //         base.Seed(seeder);
// //         return this;
// //     }
// // }