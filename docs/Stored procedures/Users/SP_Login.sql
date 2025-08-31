ALTER PROCEDURE [dbo].[SP_Login]
    @Email NVARCHAR(255)
AS
BEGIN
    -- Select the user by email (not deleted)
    SELECT TOP 1 * --It returns all columns, including Active and Password.
    FROM UsersTable
    WHERE Email = @Email AND DeletedAt IS NULL;
END