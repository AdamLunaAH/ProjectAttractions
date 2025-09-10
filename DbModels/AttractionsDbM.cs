using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

public class AttractionsDbM : Attractions, ISeed<AttractionsDbM>
{
    [Key]
    public override Guid AttractionId { get; set; }
    public override string AttractionName { get; set; }
    // public override AttractionCategories Category { get; set; }
    public override string AttractionDescription { get; set; }
    // public virtual string AttractionPlace { get; set; }

    // public Guid? AddressId { get; set; }
    [NotMapped]
        public override IAttractionAddresses AttractionAddresses { get; set; }

    #region correcting the Navigation properties migration error caused by using interfaces
    // [NotMapped]
    // public override IAttractionAddresses AttractionAddresses { get => AttractionAddressesDbM; set => new NotImplementedException(); }

    [JsonIgnore]
    //[ForeignKey("AddressId")]
    [NotMapped]
    public AttractionAddressesDbM AttractionAddressesDbM { get; set; } = null;    //This is implemented in the database table

    #endregion

    #region constructors
    public AttractionsDbM() { }
    #endregion



    public new AttractionsDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
}

// using Seido.Utilities.SeedGenerator;
// using Models;
// using System.ComponentModel.DataAnnotations;

// namespace DbModels;

// public class AttractionsDbM : Attractions, ISeed<AttractionsDbM>
// {
//     [Key]
//     public override Guid AttractionId { get; set; }

//     public new AttractionsDbM Seed(SeedGenerator seeder)
//     {
//         base.Seed(seeder);
//         return this;
//     }
// }