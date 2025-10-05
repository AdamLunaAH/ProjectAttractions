namespace Models.DTO;

public class SupUsrInfoDbDto
{

    public int NrUsers { get; set; } = 0;
    public int NrSeededUsers { get; set; } = 0;
    public int NrUnseededUsers { get; set; } = 0;
    public int NrAttractionAddresses{ get; set; } = 0;
    public int NrSeededAttractionAddresses { get; set; } = 0;
    public int NrUnseededAttractionAddresses { get; set; } = 0;
    public int NrAttractions { get; set; } = 0;
    public int NrSeededAttractions { get; set; } = 0;
    public int NrUnseededAttractions { get; set; } = 0;
    public int NrAttractionsWithNoAddress { get; set; } = 0;
    public int NrCategories { get; set; } = 0;
    public int NrSeededCategories { get; set; } = 0;
    public int NrUnseededCategories { get; set; } = 0;
    public int NrReviews { get; set; } = 0;
    public int NrSeededReviews { get; set; } = 0;
    public int NrUnseededReviews { get; set; } = 0;
    // public int AttractionsWithoutReviews { get; set; } = 0;
}

public class SupUsrInfoAttractionsWithoutReviewsDto
{
    public string AttractionName { get; set; } = null;
    public string Country { get; set; } = null;
    public string CityPlace { get; set; } = null;
    public int NrAttractions { get; set; } = 0;
}

public class SupUsrInfoAttractionsDto
{
    public string Country { get; set; } = null;
    public string CityPlace { get; set; } = null;
    public int NrAddresses { get; set; } = 0;
}

public class SupUsrInfoCategoriesDto
{
    public string CategoryName { get; set; } = null;
    public int NrCategories { get; set; } = 0;
}
public class SupUsrInfoAttractionAddressesDto
{
    public string StreetAddress { get; set; } = null;
    public string ZipCode { get; set; } = null;
    public string CityPlace { get; set; } = null;
    public string Country { get; set; } = null;
    public int NrAddresses { get; set; } = 0;
}

public class SupUsrInfoUsersDto
{
    public string FirstName { get; set; } = null;
    public string LastName { get; set; } = null;

    public string Email { get; set; } = null;

    public int NrUsers { get; set; } = 0;
}
public class SupUsrInfoReviewsDto
{
    public string ReviewScore { get; set; } = null;
    public string ReviewText { get; set; } = null;
    public int NrReviews { get; set; } = 0;
}


public class SupUsrInfoAllDto
{
    public SupUsrInfoDbDto Db { get; set; } = null;
    public List<SupUsrInfoAttractionsDto> Attractions { get; set; } = null;
    public List<SupUsrInfoCategoriesDto> Categories { get; set; } = null;
    public List<SupUsrInfoAttractionAddressesDto> AttractionAddresses { get; set; } = null;
    public List<SupUsrInfoUsersDto> Users { get; set; } = null;
    public List<SupUsrInfoReviewsDto> Reviews { get; set; } = null;

    public List<SupUsrInfoAttractionsWithoutReviewsDto> AttractionsWithoutReviews { get; set; } = null;
}