ALTER PROCEDURE [dbo].[SP_GetRentedMovies]

@UserId INT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- SET NOCOUNT ON;

    SELECT 
        m.Id, m.Url, m.PrimaryTitle, m.Description, m.PrimaryImage, m.Year, m.ReleaseDate, m.Language,
        m.Budget, m.GrossWorldwide, m.Genres, m.IsAdult, m.RuntimeMinutes, m.AverageRating, m.NumVotes,
        m.PriceToRent, m.RentalCount, r.RentStart, r.RentEnd, r.TotalPrice,  r.Id AS RentalId
    FROM MoviesTable m
    JOIN RentedMovie r ON m.Id = r.MovieId
    WHERE r.UserId = @UserId
    --AND r.RentStart <= GETDATE()
    --AND r.RentEnd >= GETDATE()
    AND r.DeletedAt IS NULL
    AND m.DeletedAt IS NULL
END
