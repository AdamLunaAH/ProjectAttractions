using Models;
using Seido.Utilities.SeedGenerator;
namespace Models;

public class Reviews : IReviews, ISeed<Reviews>
{
    public virtual Guid ReviewId { get; set; }
    public virtual IUsers Users { get; set; }
    public virtual IAttractions Attractions { get; set; }
    public virtual int ReviewScore { get; set; }
    public virtual string ReviewText { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime UpdatedAt { get; set; }


    #region constructors
    public Reviews() { }

    public Reviews(Reviews org)
    {
        this.Seeded = org.Seeded;

        this.ReviewId = org.ReviewId;
        this.ReviewScore = org.ReviewScore;
        this.ReviewText = org.ReviewText;
        this.CreatedAt = org.CreatedAt;
        this.UpdatedAt = org.UpdatedAt;

        //use the ternary operator to create only if the original is not null
        this.Users = (org.Users != null) ? new Users((Users)org.Users) : null;
        this.Attractions = (org.Attractions != null) ? new Attractions((Attractions)org.Attractions) : null;


    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual Reviews Seed(SeedGenerator seeder)
    {
        Seeded = true;
        ReviewScore = seeder.Next(0, 5);
        ReviewId = Guid.NewGuid();
        // this.Users = new Users();
        // UserId = Guid.NewGuid();
        // this.Attractions = new Attractions();
        // AttractionId = Guid.NewGuid();
        ReviewText = seeder.LatinSentences(2).FirstOrDefault();

        CreatedAt = seeder.DateAndTime(2020, 2025);
        UpdatedAt = DateTime.Now;

        return this;


    }
    #endregion
}