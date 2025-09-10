using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

public class AttractionAddressesDbM : AttractionAddresses, ISeed<AttractionAddressesDbM>, IEquatable<AttractionAddressesDbM>
{

    [Key]
    public override Guid AddressId { get; set; }
    // { get => base.AddressId; set => base.AddressId = value; }

    public override string StreetAddress { get; set; }
    public override string ZipCode { get; set; }
    public override string CityPlace { get; set; }
    public override string Country { get; set; }

    #region implementing IEquatable
    public bool Equals(AttractionAddressesDbM other) => (other != null) && ((StreetAddress, ZipCode, CityPlace, Country) ==
        (other.StreetAddress, other.ZipCode, other.CityPlace, other.Country));

    public override bool Equals(object obj) => Equals(obj as AttractionAddressesDbM);
    public override int GetHashCode() => (StreetAddress, ZipCode, CityPlace, Country).GetHashCode();
    #endregion

    #region correcting the Navigation properties migration error caused by using interfaces
    [NotMapped] //removed from EFC

    [JsonIgnore] //do not include in any json response from the WebApi
    public List<AttractionsDbM> AttractionsDbM { get; set; } = null;
    #endregion

    #region constructors
    public AttractionAddressesDbM() { }
    #endregion

    public new AttractionAddressesDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
}

// public class AttractionAddressesDbM : AttractionAddresses, ISeed<AttractionAddressesDbM>
// {

//     [Key]
//     public override Guid AddressId { get; set; }
//     // { get => base.AddressId; set => base.AddressId = value; }

//     public new AttractionAddressesDbM Seed(SeedGenerator seeder)
//     {
//         base.Seed(seeder);
//         return this;
//     }
// }