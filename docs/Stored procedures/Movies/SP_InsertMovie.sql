ALTER PROCEDURE [dbo].[SP_InsertMovie]
    @Url NVARCHAR(MAX),
    @PrimaryTitle NVARCHAR(255),
    @Description NVARCHAR(MAX),
    @PrimaryImage NVARCHAR(MAX),
    @Year INT,
    @ReleaseDate DATE,
    @Language NVARCHAR(50),
    @Budget FLOAT,
    @GrossWorldwide FLOAT,
    @Genres NVARCHAR(255),
    @IsAdult BIT,
    @RuntimeMinutes INT,
    @AverageRating FLOAT,
    @NumVotes INT,
	@PriceToRent INT

AS
BEGIN

    -- Insert the movie into the Movies table
    INSERT INTO MoviesTable (Url, PrimaryTitle, Description, PrimaryImage, Year, ReleaseDate, Language, Budget, GrossWorldwide, Genres, IsAdult, RuntimeMinutes, AverageRating, NumVotes, PriceToRent)
    VALUES (@Url, @PrimaryTitle, @Description, @PrimaryImage, @Year, @ReleaseDate, @Language, @Budget, @GrossWorldwide, @Genres, @IsAdult, @RuntimeMinutes, @AverageRating, @NumVotes, @PriceToRent);
END
