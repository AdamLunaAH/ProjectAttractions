using Seido.Utilities.SeedGenerator;
namespace Models;

public class Users : IUsers, ISeed<Users>
{
    public virtual Guid UserId { get; set; }
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime UpdatedAt { get; set; }

    public virtual List<IReviews> Reviews { get; set; } = null;


    #region constructors
    public Users() { }

    public Users(Users org)
    {
        this.Seeded = org.Seeded;

        this.UserId = org.UserId;
        this.FirstName = org.FirstName;
        this.LastName = org.LastName;
        this.CreatedAt = org.CreatedAt;
        this.UpdatedAt = org.UpdatedAt;

        //using Linq Select and copy contructor to create a list copy
        this.Reviews = (org.Reviews != null) ? org.Reviews.Select(p => new Reviews((Reviews)p)).ToList<IReviews>() : null;
    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual Users Seed(SeedGenerator seeder)
    {
        Seeded = true;
        UserId = Guid.NewGuid();
        FirstName = seeder.FirstName;
        LastName = seeder.LastName;

        CreatedAt = seeder.DateAndTime(2010, 2024);
        UpdatedAt = DateTime.Now;

        return this;
    }
    #endregion
}