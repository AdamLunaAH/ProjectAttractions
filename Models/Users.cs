using Seido.Utilities.SeedGenerator;
namespace Models;

public class Users : IUsers, ISeed<Users>
{
    public virtual Guid UserId { get; set; }
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime UpdatedAt { get; set; }

    public bool Seeded { get; set; } = false;

    #region constructors
    public Users() { }

    public Users Seed(SeedGenerator seeder)
    {
        Seeded = true;
        UserId = Guid.NewGuid();
        FirstName = seeder.FirstName;
        LastName = seeder.LastName;

        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;

        return this;
    }
    #endregion
}