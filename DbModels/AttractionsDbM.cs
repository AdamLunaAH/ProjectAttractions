using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

[Table("AttractionsDb", Schema = "supusr")]
// change to true later
[Index(nameof(AttractionName), nameof(AttractionDescription), nameof(AddressId), IsUnique = true)]
sealed public class AttractionsDbM : Attractions, ISeed<AttractionsDbM>
{
    [Key]
    public override Guid AttractionId { get; set; }
    public override string AttractionName { get; set; }
    // public override AttractionCategories Category { get; set; }
    public override string AttractionDescription { get; set; }
    // public virtual string AttractionPlace { get; set; }

    // public Guid? AddressId { get; set; }
    [JsonIgnore]
    public Guid? AddressId { get; set; }

    #region correcting the Navigation properties migration error caused by using interfaces
    [NotMapped]
    public override IAttractionAddresses AttractionAddresses { get => AttractionAddressDbM; set => new NotImplementedException(); }

    [JsonIgnore]
    // [ForeignKey("AddressId")]
    public AttractionAddressDbM AttractionAddressDbM { get; set; } = null;    //This is implemented in the database table
    #endregion

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override List<ICategories> Categories { get => CategoriesDbM?.ToList<ICategories>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<CategoriesDbM> CategoriesDbM { get; set; } = null;
    #endregion

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override List<IReviews> Reviews { get => ReviewsDbM?.ToList<IReviews>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<ReviewsDbM> ReviewsDbM { get; set; } = null;
    #endregion

    #region implementing IEquatable
    public bool Equals(AttractionsDbM other) => (other != null) && ((AttractionName, AttractionAddresses) ==
        (other.AttractionName, other.AttractionAddresses));

    public override bool Equals(object obj) => Equals(obj as AttractionsDbM);
    public override int GetHashCode() => (AttractionName, AttractionAddresses).GetHashCode();
    #endregion

    #region randomly seed this instance
    public override AttractionsDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
    #endregion

    #region constructors
    public AttractionsDbM() { }
    #endregion

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