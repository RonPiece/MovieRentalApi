CREATE TABLE dbo.RentedMovies
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    MovieId INT NOT NULL,
    RentStart DATE NOT NULL,
    RentEnd DATE NOT NULL,
    TotalPrice FLOAT NOT NULL,
    DeletedAt DATE NULL,
    CONSTRAINT FK_RentedMovies_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
    CONSTRAINT FK_RentedMovies_Movies FOREIGN KEY (MovieId) REFERENCES dbo.Movies(Id)
);