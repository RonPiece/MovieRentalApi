ALTER PROCEDURE [dbo].[SP_GetAllUsers]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- SET NOCOUNT ON;

	SELECT Id,[Name],Email,[Password],Active,DeletedAt
	FROM UsersTable
	-- I want to swagger to have the option to see all users, even the delted ones.
	-- WHERE DeletedAt IS NULL
END
