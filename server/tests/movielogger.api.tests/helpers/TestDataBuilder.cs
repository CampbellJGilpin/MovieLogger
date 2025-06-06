using movielogger.dal.entities;

namespace movielogger.api.tests.helpers;

public static class TestDataBuilder
{
    public static User CreateTestUser(int id = 1, string? userName = null, string? email = null)
    {
        return new User
        {
            Id = id,
            UserName = userName ?? $"TestUser{id}",
            Email = email ?? $"testuser{id}@example.com",
            Password = "password123",
            IsAdmin = false,
            IsDeleted = false
        };
    }

    public static Genre CreateTestGenre(int id = 1, string? title = null)
    {
        return new Genre
        {
            Id = id,
            Title = title ?? $"TestGenre{id}"
        };
    }

    public static Movie CreateTestMovie(int id = 1, int genreId = 1, string? title = null)
    {
        return new Movie
        {
            Id = id,
            Title = title ?? $"TestMovie{id}",
            Description = $"Description for movie {id}",
            GenreId = genreId,
            ReleaseDate = DateTime.Now.AddYears(-id),
            IsDeleted = false
        };
    }

    public static UserMovie CreateTestUserMovie(int id = 1, int userId = 1, int movieId = 1)
    {
        return new UserMovie
        {
            Id = id,
            UserId = userId,
            MovieId = movieId,
            Favourite = false,
            OwnsMovie = true
        };
    }

    public static Viewing CreateTestViewing(int id = 1, int userMovieId = 1)
    {
        return new Viewing
        {
            Id = id,
            UserMovieId = userMovieId,
            DateViewed = DateTime.Now.AddDays(-id)
        };
    }

    public static Review CreateTestReview(int id = 1, int viewingId = 1)
    {
        return new Review
        {
            Id = id,
            ViewingId = viewingId,
            ReviewText = $"Test review {id}",
            Score = 4
        };
    }
} 