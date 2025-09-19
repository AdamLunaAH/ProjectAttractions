namespace Models;

public interface IReviews
{
    public Guid ReviewId { get; set; }
    public IUsers Users { get; set; }
    public IAttractions Attractions { get; set; }
    public int ReviewScore { get; set; }
    public string ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}