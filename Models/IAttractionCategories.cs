namespace Models;

// public enum AttractionCategories { Restaurant, Cafe, Architecture }

public interface IAttractionCategories
{
    public Guid AttractionCategoryId { get; set; }
    public Guid AttractionId { get; set; }
    public Guid CategoryId { get; set; }
}