using Seido.Utilities.SeedGenerator;
namespace Models;

public class Categories : ICategories, ISeed<Categories>
{
    public virtual Guid CategoryId { get; set; }
    public virtual string CategoryName { get; set; }
    public virtual List<IAttractions> Attractions { get; set; }


    #region constructors

    public Categories() { }

    public Categories(Categories org)
    {
        this.Seeded = org.Seeded;
        this.CategoryId = org.CategoryId;
        this.CategoryName = org.CategoryName;
    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual Categories Seed(SeedGenerator seeder)
    {
        Seeded = true;
        CategoryId = Guid.NewGuid();
        CategoryName = seeder.PetName;

        return this;
    }

    #endregion
}