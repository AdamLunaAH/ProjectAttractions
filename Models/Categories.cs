using Seido.Utilities.SeedGenerator;
namespace Models;

public class Categories : ICategories, ISeed<Categories>
{
    public virtual Guid CategoryId { get; set; }
    public virtual string CategoryName { get; set; }

    public bool Seeded { get; set; } = false;

    #region constructors

    public Categories() { }

    public Categories Seed(SeedGenerator seeder)
    {
        Seeded = true;
        CategoryId = Guid.NewGuid();

        CategoryName = seeder.PetName;

        return this;
    }

    #endregion
}