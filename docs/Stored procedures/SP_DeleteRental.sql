ALTER PROCEDURE [dbo].[SP_DeleteRental]
    @UserId INT,
    @MovieId INT
AS
BEGIN
    DELETE FROM RentedMovie
    WHERE UserId = @UserId AND MovieId = @MovieId;
END