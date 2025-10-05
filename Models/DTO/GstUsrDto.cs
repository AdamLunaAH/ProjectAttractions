namespace Models.DTO;

public class GstUsrInfoDbDto
{

    public int NrSeededUsers { get; set; } = 0;
    public int NrUnseededUsers { get; set; } = 0;
    public int NrAttractionsWithNoAddress { get; set; } = 0;

    public int NrSeededAttractionAddresses { get; set; } = 0;
    public int NrUnseededAttractionAddresses { get; set; } = 0;

    public int NrSeededAttractions { get; set; } = 0;
    public int NrUnseededAttractions { get; set; } = 0;

    public int NrSeededCategories { get; set; } = 0;
    public int NrUnseededCategories { get; set; } = 0;

    public int NrSeededReviews { get; set; } = 0;
    public int NrUnseededReviews { get; set; } = 0;

    public int AttractionsWithoutReviews { get; set; } = 0;
}


public class GstUsrInfoAttractionsDto
{
    public string Country { get; set; } = null;
    public string CityPlace { get; set; } = null;
    public int NrAddresses { get; set; } = 0;
}

public class GstUsrInfoCategoriesDto
{
    public string CategoryName { get; set; } = null;
    public int NrCategories { get; set; } = 0;
}
public class GstUsrInfoAttractionAddressesDto
{
    public string StreetAddress { get; set; } = null;
    public string ZipCode { get; set; } = null;
    public string CityPlace { get; set; } = null;
    public string Country { get; set; } = null;
    public int NrAddresses { get; set; } = 0;
}

public class GstUsrInfoUsersDto
{
    public string FirstName { get; set; } = null;
    public string LastName { get; set; } = null;

    public string Email { get; set; } = null;

    public int NrUsers { get; set; } = 0;
}
public class GstUsrInfoReviewsDto
{
    public string ReviewScore { get; set; } = null;
    public string ReviewText { get; set; } = null;
    public int NrReviews { get; set; } = 0;
}

public class GstUsrInfoAttractionsWithoutReviewsDto
{
    public string AttractionName { get; set; } = null;
    public string Country { get; set; } = null;
    public string CityPlace { get; set; } = null;
    public int NrAttractions { get; set; } = 0;
}


public class GstUsrInfoAllDto
{
    public GstUsrInfoDbDto Db { get; set; } = null;
    public List<GstUsrInfoAttractionsDto> Attractions { get; set; } = null;
    public List<GstUsrInfoCategoriesDto> Categories { get; set; } = null;
    public List<GstUsrInfoAttractionAddressesDto> AttractionAddresses { get; set; } = null;
    public List<GstUsrInfoUsersDto> Users { get; set; } = null;
    public List<GstUsrInfoReviewsDto> Reviews { get; set; } = null;
    public List<GstUsrInfoAttractionsWithoutReviewsDto> AttractionsWithoutReviews { get; set; } = null;

}