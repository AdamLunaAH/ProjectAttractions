using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;
using Models.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DbModels;

[Table("ReviewsDb", Schema = "supusr")]
[Index(nameof(ReviewScore), nameof(ReviewText), IsUnique = false)]
[Index(nameof(AttractionId), nameof(UserId), IsUnique = true)]

sealed public class ReviewsDbM : Reviews, ISeed<ReviewsDbM>, IEquatable<ReviewsDbM>
{
    [Key]
    public override Guid ReviewId { get; set; }

    [JsonIgnore]
    public Guid? UserId { get; set; }

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override IUsers Users { get => UsersDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    // [ForeignKey("AddressId")]
    public UsersDbM UsersDbM { get; set; } = null;    //This is implemented in the database table
    #endregion

    public Guid? AttractionId { get; set; }

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override IAttractions Attractions { get => AttractionsDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    // [ForeignKey("AddressId")]
    public AttractionsDbM AttractionsDbM { get; set; } = null;    //This is implemented in the database table
    #endregion



    public override int ReviewScore { get; set; }
    [Column(TypeName = "VARCHAR")]
    [StringLength(250)]
    public override string ReviewText { get; set; }

    public override DateTime CreatedAt { get; set; }
    // public override DateTime UpdatedaAt { get; set; }



    #region IEquatable
    public bool Equals(ReviewsDbM other) => (other != null) && ((Users, Attractions, ReviewScore, CreatedAt) == (other.Users, other.Attractions, other.ReviewScore, other.CreatedAt));

    public override bool Equals(object obj) => Equals(obj as ReviewsDbM);
    public override int GetHashCode() => (Users, Attractions, ReviewScore, CreatedAt).GetHashCode();
    #endregion



    public override ReviewsDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }

    #region Update from DTO
    public ReviewsDbM UpdateFromDTO(ReviewsCuDto org)
    {
        ReviewScore = org.ReviewScore;
        ReviewText = org.ReviewText;

        return this;
    }
    #endregion

    #region constructors
    public ReviewsDbM() { }
    public ReviewsDbM(ReviewsCuDto org)
    {
        ReviewId = Guid.NewGuid();
        UpdateFromDTO(org);
    }

    #endregion

}