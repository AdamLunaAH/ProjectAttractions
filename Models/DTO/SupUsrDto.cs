namespace Models.DTO;

public class SupUsrInfoDbDto
{

    public int NrSeededUsers { get; set; } = 0;
    public int NrUnseededUsers { get; set; } = 0;
    public int NrAttractionsWithAddress { get; set; } = 0;

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

public class SupUsrInfoAllDto
{
    public SupUsrInfoDbDto Db { get; set; } = null;
    public List<GstUsrInfoFriendsDto> Friends { get; set; } = null;
    public List<GstUsrInfoPetsDto> Pets { get; set; } = null;
    public List<GstUsrInfoQuotesDto> Quotes { get; set; } = null;
}