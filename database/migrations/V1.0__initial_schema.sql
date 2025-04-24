CREATE TABLE Genres (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(100) NOT NULL
);

CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    IsAdmin BOOLEAN DEFAULT FALSE,
    IsDeleted BOOLEAN DEFAULT FALSE
);

CREATE TABLE Movies (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(500) NOT NULL,
    Description TEXT,
    ReleaseDate TIMESTAMP,
    GenreId INT NOT NULL,
    IsDeleted BOOLEAN DEFAULT FALSE,
    CONSTRAINT FK_Movies_GenreId_Genres_Id FOREIGN KEY (GenreId) REFERENCES Genres(Id)
);

CREATE TABLE User_Movies (
    Id SERIAL PRIMARY KEY,
    UserId INT NOT NULL,
    MovieId INT NOT NULL,
    Favourite BOOLEAN DEFAULT FALSE,
    OwnsMovie BOOLEAN DEFAULT FALSE,
    UpcomingViewDate TIMESTAMP,
    CONSTRAINT FK_UserMovies_UserId_Users_Id FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserMovies_MovieId_Movies_Id FOREIGN KEY (MovieId) REFERENCES Movies(Id),
    UNIQUE(UserId, MovieId)
);

CREATE TABLE Viewings (
    Id SERIAL PRIMARY KEY,
    UserMovieId INT NOT NULL,
    DateViewed TIMESTAMP NOT NULL,
    CONSTRAINT FK_Viewings_UserMovieId_UserMovies_Id FOREIGN KEY (UserMovieId) REFERENCES User_Movies(Id)
);

CREATE TABLE Reviews (
    Id SERIAL PRIMARY KEY,
    ViewingId INT NOT NULL,
    ReviewText TEXT,
    Score INT,
    CONSTRAINT FK_Reviews_ViewingId_Viewings_Id FOREIGN KEY (ViewingId) REFERENCES Viewings(Id)
);
