namespace Models;

public class AttractionCategories : IAttractionCategories
{
    public virtual Guid AttractionCategoryId { get; set; }
    public virtual Guid AttractionId { get; set; }
    public virtual Guid CategoryId { get; set; }
}