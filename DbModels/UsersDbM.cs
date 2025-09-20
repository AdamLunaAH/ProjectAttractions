using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;
using Models.DTO;

namespace DbModels;

[Table("UsersDb", Schema = "supusr")]
[Index(nameof(FirstName), nameof(LastName), IsUnique = false)]
[Index(nameof(LastName), nameof(FirstName), IsUnique = false)]
[Index(nameof(Email), IsUnique = true)]
sealed public class UsersDbM : Users, ISeed<UsersDbM>, IEquatable<UsersDbM>
{
    // public override Guid UserId { get => base.UserId; set => base.UserId = value; }
    [Key]
    public override Guid UserId { get; set; }
    public override string FirstName { get; set; }
    public override string LastName { get; set; }
    public override string Email { get; set; }
    public override DateTime CreatedAt { get; set; }
    // public override DateTime UpdatedaAt { get; set; }

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override List<IReviews> Reviews { get => ReviewsDbM?.ToList<IReviews>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<ReviewsDbM> ReviewsDbM { get; set; } = null;
    #endregion

    #region IEquatable

    public bool Equals(UsersDbM other) => (other != null) && ((FirstName, LastName, Email) == (other.FirstName, other.LastName, other.Email));
    public override bool Equals(object obj) => Equals(obj as UsersDbM);
    public override int GetHashCode() => (FirstName, LastName, Email).GetHashCode();
    #endregion


    #region seeder
    public override UsersDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
    #endregion

    #region Update from DTO
    public UsersDbM UpdateFromDTO(UsersCuDto org)
    {
        if (org == null) return null;

        FirstName = org.FirstName;
        LastName = org.LastName;
        Email = org.Email;

        return this;
    }
    #endregion

    #region constructors
    public UsersDbM() { }

    public UsersDbM(UsersCuDto org)
    {
        UserId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
    #endregion





}