namespace Models;

// public enum AttractionCategories { Restaurant, Cafe, Architecture }

public interface ICategories
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }

    public List<IAttractions> Attractions { get; set; }
}