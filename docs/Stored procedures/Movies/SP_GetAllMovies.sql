ALTER PROCEDURE [dbo].[SP_GetAllMovies]
    @Page INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM MoviesTable
    WHERE DeletedAt IS NULL
    ORDER BY Id
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Return total count for pagination
    SELECT COUNT(*) AS TotalCount FROM MoviesTable WHERE DeletedAt IS NULL;
END
