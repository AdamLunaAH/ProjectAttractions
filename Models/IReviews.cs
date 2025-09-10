namespace Models;

public interface IReviews
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public Guid AttractionId { get; set; }
    public string ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}