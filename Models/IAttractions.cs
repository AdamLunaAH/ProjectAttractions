namespace Models;

public interface IAttractions
{
    public Guid AttractionId { get; set; }
    public string AttractionName { get; set; }
    public string AttractionDescription { get; set; }
    public IAttractionAddresses AttractionAddresses { get; set; }
    public List<ICategories> Categories { get; set; }
    public List<IReviews> Reviews { get; set; }


}