using Seido.Utilities.SeedGenerator;

namespace Models;

public class AttractionAddresses : IAttractionAddresses, ISeed<AttractionAddresses>, IEquatable<AttractionAddresses>
{
    public virtual Guid AddressId { get; set; }
    public virtual string StreetAddress { get; set; }
    public virtual string ZipCode { get; set; }
    public virtual string CityPlace { get; set; }
    public virtual string Country { get; set; }

    public override string ToString() => $"{StreetAddress}, {ZipCode} {CityPlace}, {Country}";

    #region Seeder
    public bool Seeded { get; set; } = false;

    #endregion
    public AttractionAddresses() { }

    public AttractionAddresses Seed(SeedGenerator seeder)
    {
        Seeded = true;
        AddressId = Guid.NewGuid();
        StreetAddress = seeder.StreetAddress();
        ZipCode = seeder.ZipCode.ToString();
        CityPlace = seeder.City();
        Country = seeder.Country;

        return this;
    }

    #region implementing IEquatable
    public bool Equals(AttractionAddresses other) => (other != null) && ((this.StreetAddress, this.ZipCode, this.CityPlace, this.Country) ==
        (other.StreetAddress, other.ZipCode, other.CityPlace, other.Country));

    public override bool Equals(object obj) => Equals(obj as AttractionAddresses);
    public override int GetHashCode() => (StreetAddress, ZipCode, CityPlace, Country).GetHashCode();
    #endregion
}


// using Seido.Utilities.SeedGenerator;

// namespace Models;

// public class AttractionAddresses : IAttractionAddresses, ISeed<AttractionAddresses>, IEquatable<AttractionAddresses>
// {
//     public virtual Guid AddressId { get; set; }
//     public virtual string StreetAddress { get; set; }
//     public virtual string ZipCode { get; set; }
//     public virtual string Place { get; set; }
//     public virtual string Country { get; set; }

//     #region Seeder
//     public bool Seeded { get; set; } = false;
//     public AttractionAddresses Seed(SeedGenerator seeder)
//     {
//         Seeded = true;
//         AddressId = Guid.NewGuid();
//         StreetAddress = seeder.StreetAddress();
//         ZipCode = seeder.ZipCode.ToString();
//         Country = seeder.Country;

//         return this;

//     }

//     #endregion
//     public AttractionAddresses() { }

//     public AttractionAddresses Seed(SeedGenerator seeder)
//     {
//         Seeded = true;
//         AddressId = Guid.NewGuid();
//         StreetAddress = seeder.StreetAddress();
//         ZipCode = seeder.ZipCode.ToString();
//         Country = seeder.Country;

//         return this;

//     }

//     #region IEquatable
//     public bool Equals(Addresses other) => (other != null) && ((this.StreetAddress, this.ZipCode, this.City, this.Country) ==
//         (other.StreetAddress, other.ZipCode, other.City, other.Country));
//     public override bool Equals(object obj) => Equals(obj as Addresses);
//     public override int GetHashCode() => (StreetAddress, ZipCode, City, Country).GetHashCode();
//     #endregion
// }