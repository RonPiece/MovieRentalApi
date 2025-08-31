ALTER PROCEDURE [dbo].[SP_RentMovie]
    @UserId INT,
    @MovieId INT,
    @RentStart DATE,
    @RentEnd DATE
AS
BEGIN
	-- Prevent renting with a deleted user
	IF EXISTS (SELECT 1 FROM UsersTable WHERE Id = @UserId AND DeletedAt IS NOT NULL)
	BEGIN
		RAISERROR('User is deleted and cannot rent movies.', 16, 1)
		RETURN
	END

	-- Prevent renting a deleted movie
	IF EXISTS (SELECT 1 FROM MoviesTable WHERE Id = @MovieId AND DeletedAt IS NOT NULL) 
	BEGIN
		RAISERROR('Movie is deleted and cannot be rented.', 16, 1)
		RETURN
	END

	-- Prevent overlapping rentals for the same movie
	IF EXISTS (
    SELECT 1 FROM RentedMovie
    WHERE UserId = @UserId  -- Add this condition to limit check to same user
    AND MovieId = @MovieId
    AND DeletedAt IS NULL
    AND ((RentStart <= @RentEnd AND RentEnd >= @RentStart)))
	BEGIN
		RAISERROR('You already have this movie rented for overlapping dates.', 16, 1)
		RETURN
	END

    -- Declare variables
    DECLARE @PriceToRent INT;
    DECLARE @RentalDays INT;
    DECLARE @TotalPrice FLOAT;

    -- Fetch the PriceToRent for the movie
    SELECT @PriceToRent = PriceToRent
    FROM MoviesTable
    WHERE Id = @MovieId;

    -- Calculate the rental duration (including both start and end dates)
    SET @RentalDays = DATEDIFF(DAY, @RentStart, @RentEnd) + 1;

    -- Calculate the TotalPrice
    SET @TotalPrice = @RentalDays * @PriceToRent;

    -- Insert the rental record into RentedMovie
    INSERT INTO RentedMovie (UserId, MovieId, RentStart, RentEnd, TotalPrice)
    VALUES (@UserId, @MovieId, @RentStart, @RentEnd, @TotalPrice);

    -- Update the movie's rental count and gross worldwide earnings
    UPDATE MoviesTable
    SET RentalCount = RentalCount + 1,
        GrossWorldwide = GrossWorldwide + @TotalPrice
    WHERE Id = @MovieId;
END
