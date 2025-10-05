namespace Models.DTO;

using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;
//DTO is a DataTransferObject, can be instanstiated by the controller logic
//and represents a, fully instantiable, subset of the Database models
//for a specific purpose.

//These DTO are simplistic and used to Update and Create objects

public class UserCreateDto
{
    // public Guid? UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
public class UsersCuDto
{
    public virtual Guid? UserId { get; set; }

    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }

    public virtual string Email { get; set; }

    public DateTime? CreatedAt { get; set; } = null;

    public virtual List<Guid> ReviewId { get; set; } = null;


    public UsersCuDto() { }
    public UsersCuDto(IUsers org)
    {
        if (org == null)
            return;
        UserId = org.UserId;
        FirstName = org.FirstName;
        LastName = org.LastName;
        Email = org.Email;
        CreatedAt = org.CreatedAt;

    }
}


public class AttractionAddressCreateDto
{

    public virtual string StreetAddress { get; set; }
    public virtual string ZipCode { get; set; }
    public virtual string CityPlace { get; set; }
    public virtual string Country { get; set; }
}

public class AttractionAddressesCuDto
{
    public virtual Guid? AddressId { get; set; }

    public virtual string StreetAddress { get; set; }
    public virtual string ZipCode { get; set; }
    public virtual string CityPlace { get; set; }
    public virtual string Country { get; set; }
    public virtual List<Guid> AttractionId { get; set; } = null;

    public AttractionAddressesCuDto() { }
    public AttractionAddressesCuDto(IAttractionAddresses org)
    {
        if (org == null)
            return;
        AddressId = org.AddressId;
        StreetAddress = org.StreetAddress;
        ZipCode = org.ZipCode;
        CityPlace = org.CityPlace;
        Country = org.Country;
        AttractionId = org.Attractions?.Select(i => i.AttractionId).ToList();
    }
}

public class AttractionCreateDto
{
    public virtual string AttractionName { get; set; }

    public virtual string AttractionDescription { get; set; }
    public virtual Guid? AddressId { get; set; }

    public virtual List<Guid> CategoryId { get; set; } = null;

    public virtual List<Guid> ReviewId { get; set; } = null;

}

public class AttractionsCuDto
{
    public virtual Guid AttractionId { get; set; }

    public virtual string AttractionName { get; set; }

    public virtual string AttractionDescription { get; set; }
    public virtual Guid? AddressId { get; set; }

    public virtual List<Guid> CategoryId { get; set; } = null;

    public virtual List<Guid> ReviewId { get; set; } = null;

    public AttractionsCuDto() { }
    public AttractionsCuDto(IAttractions org)
    {
        if (org == null)
            return;
        AttractionId = org.AttractionId;
        AttractionName = org.AttractionName;
        AttractionDescription = org.AttractionDescription;
        AddressId = org?.AttractionAddresses?.AddressId;
        ReviewId = org.Reviews?.Select(i => i.ReviewId).ToList();
        CategoryId = org.Categories?.Select(i => i.CategoryId).ToList();

    }
}

public class CategoryCreateDto
{
    public virtual string CategoryName { get; set; }
}

public class CategoriesCuDto
{
    public virtual Guid CategoryId { get; set; }
    public virtual string CategoryName { get; set; }

    public virtual List<Guid> AttractionId { get; set; } = null;


    public CategoriesCuDto() { }
    public CategoriesCuDto(ICategories org)
    {
        if (org == null)
            return;
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
    public virtual Guid ReviewId { get; set; }

    public virtual Guid? UserId { get; set; }
    public virtual Guid? AttractionId { get; set; }
    public virtual int ReviewScore { get; set; }

    public virtual string ReviewText { get; set; }

    public DateTime? CreatedAt { get; set; } = null;
    public ReviewsCuDto() { }
    public ReviewsCuDto(IReviews org)
    {
        if (org == null)
            return;
        ReviewId = org.ReviewId;

        UserId = org.Users.UserId;
        AttractionId = org.Attractions.AttractionId;
        ReviewScore = org.ReviewScore;
        ReviewText = org.ReviewText;
        CreatedAt = org.CreatedAt;
    }
}

// public class CategoryCreateListDto
// {
//     public virtual string CategoryName { get; set; }
// }

public class AttractionFullCreateDto
{
    public string AttractionName { get; set; }
    public string AttractionDescription { get; set; }

    // Address info
    public AttractionAddressCreateDto Address { get; set; }

    // List of new categories
    // public List<CategoryCreateDto> Categories { get; set; } = new();
    public List<string> CategoryNames { get; set; } = new();

    // Existing category ID
    // public List<Guid> ExistingCategoryIds { get; set; } = new();
}

