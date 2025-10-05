using Models.Utilities.SeedGenerator;

namespace Models;

public class AttractionCategories : IAttractionCategories
{
    public virtual Guid AttractionCategoryId { get; set; }
    public virtual IAttractions Attractions { get; set; }
    public virtual ICategories Categories { get; set; }

    public virtual bool Seeded { get; set; } = false;

    #region constructor
    #endregion
}