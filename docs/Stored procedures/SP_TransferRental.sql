ALTER PROCEDURE [dbo].[SP_TransferRental]
    @RentalId INT,
    @ToUserEmail NVARCHAR(255)
AS
BEGIN
    --SET NOCOUNT ON;

    DECLARE @ToUserId INT;
    DECLARE @MovieId INT, @RentStart DATE, @RentEnd DATE, @TotalPrice FLOAT;

    -- Find recipient user ID
    SELECT @ToUserId = Id FROM UsersTable WHERE Email = @ToUserEmail AND DeletedAt IS NULL;

    IF @ToUserId IS NULL
    BEGIN
        RAISERROR('Recipient user not found.', 16, 1);
        RETURN;
    END

    -- Get rental details from the specific rental row
    SELECT TOP 1 @MovieId = MovieId, @RentStart = RentStart, @RentEnd = RentEnd, @TotalPrice = TotalPrice
    FROM RentedMovie
    WHERE Id = @RentalId AND DeletedAt IS NULL;

    IF @MovieId IS NULL
    BEGIN
        RAISERROR('Rental not found for transfer.', 16, 1);
        RETURN;
    END

	-- Prevent transfer if rental period has ended
    IF (@RentEnd IS NULL OR @RentEnd < CAST(GETDATE() AS DATE))
    BEGIN
        RAISERROR('Cannot transfer: rental period has ended.', 16, 1);
        RETURN;
    END

    -- Prevent duplicate rental for recipient for the same movie with dates overlapping date ranges
    IF EXISTS (
    SELECT 1 FROM RentedMovie
    WHERE UserId = @ToUserId AND MovieId = @MovieId 
    AND (@RentStart <= RentEnd AND @RentEnd >= RentStart)
    AND DeletedAt IS NULL
	)
	BEGIN
		RAISERROR('Recipient already has this movie rented for overlapping dates.', 16, 1);
		RETURN;
	END

    -- Remove rental from current user (delete the specific row)
    DELETE FROM RentedMovie WHERE Id = @RentalId AND DeletedAt IS NULL;

    -- Add rental to recipient
    INSERT INTO RentedMovie (UserId, MovieId, RentStart, RentEnd, TotalPrice)
    VALUES (@ToUserId, @MovieId, @RentStart, @RentEnd, @TotalPrice);
END