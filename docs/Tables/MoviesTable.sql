CREATE TABLE dbo.Movies
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Url] NVARCHAR(MAX) NULL,
    PrimaryTitle NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    PrimaryImage NVARCHAR(MAX) NULL,
    [Year] INT NULL,
    ReleaseDate DATE NULL,
    [Language] NVARCHAR(50) NULL,
    Budget FLOAT NULL,
    GrossWorldwide FLOAT NULL,
    Genres NVARCHAR(255) NULL,
    IsAdult BIT NULL,
    RuntimeMinutes INT NULL,
    AverageRating FLOAT NULL,
    NumVotes INT NULL,
    PriceToRent FLOAT NOT NULL DEFAULT 0,
    RentalCount INT NOT NULL DEFAULT 0,
    DeletedAt DATE NULL
);