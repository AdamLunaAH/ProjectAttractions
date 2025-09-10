using Models;
using Seido.Utilities.SeedGenerator;
namespace Models;

public class Reviews : IReviews, ISeed<Reviews>
{
    public virtual Guid ReviewId { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual Guid AttractionId { get; set; }
    public virtual string ReviewText { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime UpdatedAt { get; set; }
    public bool Seeded { get; set; } = false;

    #region constructors
    public Reviews() { }
    public Reviews Seed(SeedGenerator seeder)
    {
        Seeded = true;
        ReviewId = Guid.NewGuid();
        UserId = Guid.NewGuid();
        AttractionId = Guid.NewGuid();
        ReviewText = seeder.LatinParagraph;

        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;

        return this;


    }
    #endregion
}