ALTER PROCEDURE [dbo].[SP_GetMovieById]
    @MovieId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        Id,
        Url,
        PrimaryTitle,
        [Description],
        PrimaryImage,
        Year,
        ReleaseDate,
        [Language],
        Budget,
        GrossWorldwide,
        Genres,
        IsAdult,
        RuntimeMinutes,
        AverageRating,
        NumVotes,
        PriceToRent,
        RentalCount
    FROM MoviesTable
    WHERE Id = @MovieId
      AND DeletedAt IS NULL;
END
