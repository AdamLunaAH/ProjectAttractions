using Seido.Utilities.SeedGenerator;
namespace Models;

public class Attractions : IAttractions, ISeed<Attractions>
{
    public virtual Guid AttractionId { get; set; }
    public virtual string AttractionName { get; set; }
    // public virtual AttractionCategories Category { get; set; }
    public virtual string AttractionDescription { get; set; }
    // public virtual string AttractionPlace { get; set; }
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

        // currently sets a random fist name from seeder
        AttractionName = seeder.TouristAttractionName;

        // Category = seeder.FromEnum<AttractionCategories>();

        // currently set a random full name from seeder
        AttractionDescription = seeder.TouristAttractionDescription.TouristAttractionDescription;

        AttractionAddresses = new AttractionAddresses();

        // Categories = new List<ICategories>();

        // this.AttractionAddresses = (seeder.Address != null) ? new AttractionAddresses().Seed(seeder) : null;

        // // // this.AttractionAddresses = (seeder.Address != null) ? new Address((Address)seeder.Address) : null;

        // currently set a random country
        // AttractionPlace = seeder.City();

        return this;
    }
    #endregion



    #region Seeder

    // public bool Seeded { get; set; } = false;
    // public Attractions Seed(SeedGenerator seeder)
    // {
    //     Seeded = true;
    //     AttractionId = Guid.NewGuid();

    //     // currently sets a random fist name from seeder
    //     AttractionName = seeder.FirstName;

    //     Category = seeder.FromEnum<AttractionCategories>();

    //     // currently set a random full name from seeder
    //     AttractionDescription = seeder.FullName;

    //     // currently set a random country
    //     // AttractionPlace = seeder.City();

    //     return this;

    // }

    #endregion



}

// using Seido.Utilities.SeedGenerator;
// namespace Models;

// public class Attractions : IAttractions, ISeed<Attractions>
// {
//     public virtual Guid AttractionId { get; set; }
//     public virtual string AttractionName { get; set; }
//     public virtual AttractionCategories Category { get; set; }
//     public virtual string AttractionDescription { get; set; }
//     // public virtual string AttractionPlace { get; set; }
//     public virtual IAttractionAddresses AttractionAddresses { get; set; }


//     #region Seeder

//     public bool Seeded { get; set; } = false;
//     public Attractions Seed(SeedGenerator seeder)
//     {
//         Seeded = true;
//         AttractionId = Guid.NewGuid();

//         // currently sets a random fist name from seeder
//         AttractionName = seeder.FirstName;

//         Category = seeder.FromEnum<AttractionCategories>();

//         // currently set a random full name from seeder
//         AttractionDescription = seeder.FullName;

//         // currently set a random country
//         AttractionPlace = seeder.City();

//         return this;

//     }

//     #endregion



// }