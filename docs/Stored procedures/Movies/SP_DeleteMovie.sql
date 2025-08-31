ALTER PROCEDURE [dbo].[SP_DeleteMovie]
    @MovieId INT
AS
BEGIN

    -- Mark the movie as deleted
    UPDATE MoviesTable
    SET DeletedAt = GETDATE()
    WHERE Id = @MovieId;

	    -- Mark related rows in RentedMovie as deleted
    UPDATE RentedMovie
    SET DeletedAt = GETDATE()
    WHERE MovieId = @MovieId;
END
