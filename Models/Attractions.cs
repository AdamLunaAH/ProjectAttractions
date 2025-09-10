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

    public bool Seeded { get; set; } = false;

    #region constructors
    public Attractions() { }
    public Attractions Seed(SeedGenerator seeder)
    {
        Seeded = true;
        AttractionId = Guid.NewGuid();

        // currently sets a random fist name from seeder
        AttractionName = seeder.FirstName;

        // Category = seeder.FromEnum<AttractionCategories>();

        // currently set a random full name from seeder
        AttractionDescription = seeder.FullName;

        this.AttractionAddresses = new AttractionAddresses();

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