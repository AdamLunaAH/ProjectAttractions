namespace Models.DTO;

//DTO is a DataTransferObject, can be instanstiated by the controller logic
//and represents a, fully instantiable, subset of the Database models
//for a specific purpose.

//These DTO are simplistic and used to Update and Create objects
public class UsersCuDto
{
    public virtual Guid? UserId { get; set; }

    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }

    public virtual string Email { get; set; }

    public DateTime? CreatedAt { get; set; } = null;
    // public DateTime? UpdatedaAt { get; set; } = null;



    public virtual List<Guid> ReviewId { get; set; } = null;

    // public virtual List<Guid> QuotesId { get; set; } = null;

    public UsersCuDto() { }
    public UsersCuDto(IUsers org)
    {
        UserId = org.UserId;
        FirstName = org.FirstName;
        LastName = org.LastName;
        Email = org.Email;
        CreatedAt = org.CreatedAt;

        // UpdatedaAt = org.UpdatedaAt;
        // ReviewId = org.Reviews?.Select(i => i.ReviewId).ToList();
        // QuotesId = org.Quotes?.Select(i => i.QuoteId).ToList();
    }
}

public class AttractionAddressesCuDto
{
    public virtual Guid? AddressId { get; set; }

    public virtual string StreetAddress { get; set; }
    public virtual string ZipCode { get; set; }
    public virtual string CityPlace { get; set; }
    public virtual string Country { get; set; }
    public virtual List<Guid> AttractionId { get; set; } = null;

    // public virtual List<Guid> AttractionId { get; set; } = null;

    public AttractionAddressesCuDto() { }
    public AttractionAddressesCuDto(IAttractionAddresses org)
    {
        AddressId = org.AddressId;
        StreetAddress = org.StreetAddress;
        ZipCode = org.ZipCode;
        CityPlace = org.CityPlace;
        Country = org.Country;
        AttractionId = org.Attractions?.Select(i => i.AttractionId).ToList();
    }
}

public class AttractionsCuDto
{
    //cannot be nullable as a Pets has to have an owner even when created
    public virtual Guid AttractionId { get; set; }

    public virtual string AttractionName { get; set; }

    public virtual string AttractionDescription { get; set; }
    public virtual Guid? AddressId { get; set; }

    public virtual List<Guid> CategoryId { get; set; } = null;

    public virtual List<Guid> ReviewId { get; set; } = null;

    public AttractionsCuDto() { }
    public AttractionsCuDto(IAttractions org)
    {
        AttractionId = org.AttractionId;
        AttractionName = org.AttractionName;
        AttractionDescription = org.AttractionDescription;
        AddressId = org?.AttractionAddresses?.AddressId;
        ReviewId = org.Reviews?.Select(i => i.ReviewId).ToList();
        CategoryId = org.Categories?.Select(i => i.CategoryId).ToList();

    }
}

public class CategoriesCuDto
{
    public virtual Guid CategoryId { get; set; }
    public virtual string CategoryName { get; set; }
    public virtual string Author { get; set; }

    public virtual List<Guid> AttractionId { get; set; } = null;


    public CategoriesCuDto() { }
    public CategoriesCuDto(ICategories org)
    {
        CategoryId = org.CategoryId;
        CategoryName = org.CategoryName;
        AttractionId = org.Attractions?.Select(i => i.AttractionId).ToList();
    }
}


public class ReviewCreateDto
{
    public Guid? UserId { get; set; }
    public Guid? AttractionId { get; set; }
    public int ReviewScore { get; set; }
    public string ReviewText { get; set; }
    public DateTime? CreatedAt { get; set; }
}


public class ReviewsCuDto
{
    //cannot be nullable as a Pets has to have an owner even when created
    public virtual Guid ReviewId { get; set; }

    public virtual Guid? UserId { get; set; }
    public virtual Guid? AttractionId { get; set; }
    public virtual int ReviewScore { get; set; }

    public virtual string ReviewText { get; set; }

    public DateTime? CreatedAt { get; set; } = null;
    // public DateTime? UpdatedaAt { get; set; } = null;



    // public virtual List<Guid> CategoryId { get; set; } = null;

    // public virtual List<Guid> ReviewId { get; set; } = null;

    public ReviewsCuDto() { }
    public ReviewsCuDto(IReviews org)
    {
        ReviewId = org.ReviewId;

        UserId = org.Users.UserId;
        AttractionId = org.Attractions.AttractionId;
        ReviewScore = org.ReviewScore;
        ReviewText = org.ReviewText;
        CreatedAt = org.CreatedAt;
        // UpdatedaAt = org.UpdatedaAt;

    }
}