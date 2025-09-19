namespace Models;

// public enum AttractionCategories { Restaurant, Cafe, Architecture }

public interface IAttractionCategories
{
    public Guid AttractionCategoryId { get; set; }
    public IAttractions Attractions { get; set; }
    public ICategories Categories { get; set; }
}