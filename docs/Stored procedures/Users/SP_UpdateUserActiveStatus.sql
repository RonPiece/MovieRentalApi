
ALTER PROCEDURE [dbo].[SP_UpdateUserActiveStatus]
    @UserId INT,
    @IsActive BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE UsersTable  -- Make sure 'Users' is your actual table name
    SET Active = @IsActive
    WHERE Id = @UserId;

END
