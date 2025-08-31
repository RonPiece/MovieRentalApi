ALTER PROCEDURE [dbo].[SP_InsertUser]
    @Name NVARCHAR(50),
    @Password NVARCHAR(100),
    @Email NVARCHAR(100),
	@Active BIT
AS
BEGIN

    -- Insert the user into the UsersTable
INSERT INTO UsersTable ([Name], [Password], Email, Active, DeletedAt)
VALUES (@Name, @Password, @Email, @Active, NULL);
END
