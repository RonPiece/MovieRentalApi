-- Search by Release Date Range
ALTER PROCEDURE [dbo].[SP_GetMoviesByReleaseDate]
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SELECT * FROM MoviesTable
    WHERE DeletedAt IS NULL
      AND ReleaseDate >= @StartDate
      AND ReleaseDate <= @EndDate
END
