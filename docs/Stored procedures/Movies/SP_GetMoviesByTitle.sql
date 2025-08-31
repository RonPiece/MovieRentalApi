-- Search by Title
ALTER PROCEDURE [dbo].[SP_GetMoviesByTitle]
    @Title NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MoviesTable
    WHERE DeletedAt IS NULL AND PrimaryTitle LIKE '%' + @Title + '%'
END
