CREATE TABLE Genres (
    Id INT PRIMARY KEY,
    Title VARCHAR(100) NOT NULL
);

CREATE TABLE Users (
    Id INT PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    IsAdmin BOOLEAN DEFAULT FALSE
);

CREATE TABLE Movies (
    Id INT PRIMARY KEY,
    Title VARCHAR(500) NOT NULL,
    Description VARCHAR(500),
    ReleaseDate DATETIME,
    GenreId INT NOT NULL,
    IsDeleted BOOLEAN DEFAULT FALSE,
    CONSTRAINT FL_Movies_GenreId_Genres_Id FOREIGN KEY (GenreId) REFERENCES Genres(Id)
);

CREATE TABLE User_Movies (
    Id INT PRIMARY KEY,
    UserId INT NOT NULL,
    MovieId INT NOT NULL,
    Favourite BOOLEAN DEFAULT FALSE,
    OwnsMovie BOOLEAN DEFAULT FALSE,
    UpcomingViewDate DATETIME,
    CONSTRAINT FL_UserMovies_UserId_Users_Id FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FL_UserMovies_MovieId_Movies_Id FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE,
    UNIQUE(UserId, MovieId)
);

CREATE TABLE Viewings (
    Id SERIAL PRIMARY KEY,
    UserMovieId INT NOT NULL,
    DateViewed DATETIME NOT NULL,
    CONSTRAINT FL_Viewings_UserMovieId_UserMovies_Id FOREIGN KEY (UserMovieId) REFERENCES User_Movies(Id)
);

CREATE TABLE Reviews (
    Id SERIAL PRIMARY KEY,
    ViewingId INT NOT NULL,
    ReviewText VARCHAR(500),
    Score INT CHECK (Score BETWEEN 1 AND 10),
    CONSTRAINT FL_Reviews_ViewingId_Viewings_Id FOREIGN KEY (ViewingId) REFERENCES Viewings(Id)
);
