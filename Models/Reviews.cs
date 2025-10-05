using Models;
using Models.Utilities.SeedGenerator;
namespace Models;

public class Reviews : IReviews, ISeed<Reviews>
{
    public virtual Guid ReviewId { get; set; }
    public virtual IUsers Users { get; set; }
    public virtual IAttractions Attractions { get; set; }
    public virtual int ReviewScore { get; set; }
    public virtual string ReviewText { get; set; }
    public virtual DateTime CreatedAt { get; set; }


    #region constructors
    public Reviews() { }

    public Reviews(Reviews org)
    {
        this.Seeded = org.Seeded;

        this.ReviewId = org.ReviewId;
        this.ReviewScore = org.ReviewScore;
        this.ReviewText = org.ReviewText;
        this.CreatedAt = org.CreatedAt;

        //use the ternary operator to create only if the original is not null
        this.Users = (org.Users != null) ? new Users((Users)org.Users) : null;
        this.Attractions = (org.Attractions != null) ? new Attractions((Attractions)org.Attractions) : null;


    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual Reviews Seed(SeedGenerator seeder)
    {
        string ReviewTextString = "";
        Seeded = true;
        ReviewScore = seeder.Next(0, 5);
        ReviewId = Guid.NewGuid();

        switch (ReviewScore)
        {
            case <= 1:
                ReviewTextString = seeder.ReviewBad.ReviewBad;
                break;
            case >= 2 and <= 3:
                ReviewTextString = seeder.ReviewAverage.ReviewAverage;
                break;
            case >= 4:
                ReviewTextString = seeder.ReviewGood.ReviewGood;
                break;
            default:
                throw new Exception("Something went wrong when generating a review score or review text");
        }

        ReviewText = ReviewTextString;

        CreatedAt = seeder.DateAndTime(2020, 2025);

        return this;


    }
    #endregion
}