CREATE OR ALTER VIEW supusr.vw_SupUsrInfoDb AS
SELECT
    -- Users
    (SELECT COUNT(*) FROM supusr.UsersDb) AS NrUsers,
    (SELECT COUNT(*) FROM supusr.UsersDb WHERE Seeded = 1) AS NrSeededUsers,
    (SELECT COUNT(*) FROM supusr.UsersDb WHERE Seeded = 0) AS NrUnseededUsers,

    -- Attraction Addresses
    (SELECT COUNT(*) FROM supusr.AttractionAddressesDb) AS NrAttractionAddresses,
    (SELECT COUNT(*) FROM supusr.AttractionAddressesDb WHERE Seeded = 1) AS NrSeededAttractionAddresses,
    (SELECT COUNT(*) FROM supusr.AttractionAddressesDb WHERE Seeded = 0) AS NrUnseededAttractionAddresses,

    -- Attractions
    (SELECT COUNT(*) FROM supusr.AttractionsDb) AS NrAttractions,
    (SELECT COUNT(*) FROM supusr.AttractionsDb WHERE Seeded = 1) AS NrSeededAttractions,
    (SELECT COUNT(*) FROM supusr.AttractionsDb WHERE Seeded = 0) AS NrUnseededAttractions,
    (SELECT COUNT(*) FROM supusr.AttractionsDb WHERE AddressId IS NULL) AS NrAttractionsWithNoAddress,

    -- Categories
    (SELECT COUNT(*) FROM supusr.CategoriesDb) AS NrCategories,
    (SELECT COUNT(*) FROM supusr.CategoriesDb WHERE Seeded = 1) AS NrSeededCategories,
    (SELECT COUNT(*) FROM supusr.CategoriesDb WHERE Seeded = 0) AS NrUnseededCategories,

    -- Reviews
    (SELECT COUNT(*) FROM supusr.ReviewsDb) AS NrReviews,
    (SELECT COUNT(*) FROM supusr.ReviewsDb WHERE Seeded = 1) AS NrSeededReviews,
    (SELECT COUNT(*) FROM supusr.ReviewsDb WHERE Seeded = 0) AS NrUnseededReviews;
