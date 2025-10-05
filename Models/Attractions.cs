using Models.Utilities.SeedGenerator;
namespace Models;

public class Attractions : IAttractions, ISeed<Attractions>
{
    public virtual Guid AttractionId { get; set; }
    public virtual string AttractionName { get; set; }
    public virtual string AttractionDescription { get; set; }
    // public virtual strin AttractionPlace { get; set; }
    public virtual IAttractionAddresses AttractionAddresses { get; set; } = null;

    public virtual List<ICategories> Categories { get; set; }
    public virtual List<IReviews> Reviews { get; set; }



    #region constructors
    public Attractions() { }

    public Attractions(Attractions org)
    {
        this.Seeded = org.Seeded;

        this.AttractionId = org.AttractionId;
        this.AttractionName = org.AttractionName;
        this.AttractionDescription = org.AttractionDescription;
        this.AttractionAddresses = (org.AttractionAddresses != null) ? new AttractionAddresses((AttractionAddresses)org.AttractionAddresses) : null;
        this.Categories = (org.Categories != null) ? org.Categories.Select(p => new Categories((Categories)p)).ToList<ICategories>() : null;
        this.Reviews = (org.Reviews != null) ? org.Reviews.Select(p => new Reviews((Reviews)p)).ToList<IReviews>() : null;

    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;
    public virtual Attractions Seed(SeedGenerator seeder)
    {
        Seeded = true;
        AttractionId = Guid.NewGuid();
        AttractionName = seeder.TouristAttractionName;
        AttractionDescription = seeder.TouristAttractionDescription.TouristAttractionDescription;
        AttractionAddresses = new AttractionAddresses();

        return this;
    }
    #endregion
}
