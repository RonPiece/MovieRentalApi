ALTER PROCEDURE [dbo].[SP_UpdateUser]
    @UserId INT,
    @Name NVARCHAR(50),
    @Password NVARCHAR(100),
    @Email NVARCHAR(100)
AS
BEGIN
    -- Update the user in the Users table
    UPDATE UsersTable
    SET Name = @Name,
        Password = @Password,
        Email = @Email
    WHERE Id = @UserId;
END
