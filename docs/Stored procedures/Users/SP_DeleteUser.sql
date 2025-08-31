ALTER PROCEDURE [dbo].[SP_DeleteUser]
    @UserId INT
AS
BEGIN
    -- Mark the user as deleted
    UPDATE UsersTable
    SET DeletedAt = GETDATE()
    WHERE Id = @UserId;

    -- Mark related rows in RentedMovie as deleted
    UPDATE RentedMovie
    SET DeletedAt = GETDATE()
    WHERE UserId = @UserId;
END
