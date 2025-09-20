namespace Models;

public interface IUsers
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    // public DateTime UpdatedaAt { get; set; }

    public List<IReviews> Reviews { get; set; }

}