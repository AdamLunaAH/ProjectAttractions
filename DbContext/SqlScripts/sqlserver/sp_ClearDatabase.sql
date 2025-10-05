CREATE OR ALTER PROCEDURE supusr.sp_ClearDatabase
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Delete data in the correct dependency order
        DELETE FROM supusr.ReviewsDb;
        DELETE FROM supusr.CategoriesDb;
        DELETE FROM supusr.AttractionsDb;
        DELETE FROM supusr.AttractionAddressesDb;
        DELETE FROM supusr.UsersDb;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Roll back if any error occurs
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Re-throw the error with details
        DECLARE
            @ErrorMessage NVARCHAR(4000),
            @ErrorSeverity INT,
            @ErrorState INT;

        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
