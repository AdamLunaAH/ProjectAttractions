namespace Models;

// public enum AttractionCategories { Restaurant, Cafe, Architecture }

public interface IAttractions
{
    public Guid AttractionId { get; set; }
    public string AttractionName { get; set; }
    // public AttractionCategories Category { get; set; }
    public string AttractionDescription { get; set; }
    // public string AttractionPlace { get; set; }
    public IAttractionAddresses AttractionAddresses { get; set; }
    public List<ICategories> Categories { get; set; }
    public List<IReviews> Reviews { get; set; }
}