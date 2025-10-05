using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models.Utilities.SeedGenerator;
using Models;
using Models.DTO;

namespace DbModels;

[Table("AttractionsDb", Schema = "supusr")]
// change to true later
[Index(nameof(AttractionName), nameof(AttractionDescription), nameof(AddressId), IsUnique = true)]
sealed public class AttractionsDbM : Attractions, ISeed<AttractionsDbM>
{
    [Key]
    public override Guid AttractionId { get; set; }
    [Required]
    public override string AttractionName { get; set; }
    // public override AttractionCategories Category { get; set; }
    public override string AttractionDescription { get; set; }
    // public virtual string AttractionPlace { get; set; }

    // public Guid? AddressId { get; set; }
    [JsonIgnore]
    public Guid? AddressId { get; set; }

    #region correcting the Navigation properties migration error caused by using interfaces
    [NotMapped]
    public override IAttractionAddresses AttractionAddresses { get => AttractionAddressesDbM; set => new NotImplementedException(); }

    [JsonIgnore]
    // [ForeignKey("AddressId")]
    public AttractionAddressesDbM AttractionAddressesDbM { get; set; } = null;    //This is implemented in the database table
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

    #region Update from DTO
    public AttractionsDbM UpdateFromDTO(AttractionsCuDto org)
    {
        if (org == null) return null;

        AttractionName = org.AttractionName;
        AttractionDescription = org.AttractionDescription;
        AddressId = org.AddressId;

        return this;
    }
    #endregion

    #region constructors
    public AttractionsDbM() { }

    public AttractionsDbM(AttractionsCuDto org)
    {
        AttractionId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
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