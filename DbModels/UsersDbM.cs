using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

[Table("UsersDb", Schema = "supusr")]
[Index(nameof(FirstName), nameof(LastName), IsUnique = false)]
[Index(nameof(LastName), nameof(FirstName), IsUnique = false)]
sealed public class UsersDbM : Users, ISeed<UsersDbM>, IEquatable<UsersDbM>
{
    // public override Guid UserId { get => base.UserId; set => base.UserId = value; }
    [Key]
    public override Guid UserId { get; set; }
    public override string FirstName { get; set; }
    public override string LastName { get; set; }
    public override DateTime CreatedAt { get; set; }
    public override DateTime UpdatedAt { get; set; }

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override List<IReviews> Reviews { get => ReviewsDbM?.ToList<IReviews>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<ReviewsDbM> ReviewsDbM { get; set; } = null;
    #endregion

    #region IEquatable

    public bool Equals(UsersDbM other) => (other != null) && ((FirstName, LastName) == (other.FirstName, other.LastName));
    public override bool Equals(object obj) => Equals(obj as UsersDbM);
    public override int GetHashCode() => (FirstName, LastName).GetHashCode();
    #endregion


    public override UsersDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }

    #region constructors
    public UsersDbM() { }
    #endregion





}