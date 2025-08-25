using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using movielogger.dal;
using movielogger.api.tests.helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using movielogger.services.interfaces;
using movielogger.services.services;
using movielogger.api.mappings;
using movielogger.services.mapping;
using FluentValidation;
using movielogger.dal.entities;

namespace movielogger.api.tests.fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Remove existing DbContext configuration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MovieLoggerDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(MovieLoggerDbContext));
            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }

            // Configure in-memory database
            services.AddDbContext<MovieLoggerDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            services.AddScoped<IAssessmentDbContext>(provider =>
                provider.GetRequiredService<MovieLoggerDbContext>());

            // Remove existing authentication configuration
            var authenticationDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(JwtBearerHandler));
            if (authenticationDescriptor != null)
            {
                services.Remove(authenticationDescriptor);
            }

            // Configure JWT authentication for testing
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "MovieLoggerAPI",
                    ValidAudience = "MovieLoggerUsers",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("8a6b89da22b854f58f57f15bb675d2692255801f989b24098d6266fc1f4857f13fdace450ac0503fd68db511a8520ff106a16ce31f1402a6ae4ffc3e1849b531"))
                };
            });

            // Configure services
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IAccountsService, AccountsService>();
            services.AddScoped<IGenresService, GenresService>();
            services.AddScoped<ILibraryService, LibraryService>();
            services.AddScoped<IMoviesService, MoviesService>();
            services.AddScoped<IReviewsService, ReviewsService>();
            services.AddScoped<IViewingsService, ViewingsService>();

            // Configure AutoMapper
            services.AddAutoMapper(
                typeof(ApiMappingProfile).Assembly,
                typeof(ServicesMappingProfile).Assembly);

            // Configure Validation
            services.AddValidatorsFromAssemblyContaining<Program>();

            // Configure Controllers
            services.AddControllers()
                .AddApplicationPart(typeof(Program).Assembly);

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MovieLoggerDbContext>();
            db.Database.EnsureCreated();

            SeedTestData(db);
        });
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MovieLoggerDbContext>();

        db.Reviews.RemoveRange(db.Reviews);
        db.UserMovieViewings.RemoveRange(db.UserMovieViewings);
        db.UserMovies.RemoveRange(db.UserMovies);
        db.Movies.RemoveRange(db.Movies);
        db.Users.RemoveRange(db.Users);
        db.Genres.RemoveRange(db.Genres);
        db.SaveChanges();

        // Reseed data
        SeedTestData(db);
    }

    private static void SeedTestData(MovieLoggerDbContext db)
    {
        try
        {
            // Add users
            var user1 = TestDataBuilder.CreateTestUser(1, "John Doe", "johndoe@example.com");
            var user2 = TestDataBuilder.CreateTestUser(2, "Jane Doe", "janedoe@example.com");
            db.Users.AddRange(user1, user2);
            db.SaveChanges();

            // Add genres (including Horror that tests expect)
            var genre1 = TestDataBuilder.CreateTestGenre(1, "Horror");
            var genre2 = TestDataBuilder.CreateTestGenre(2, "Action");
            var genre3 = TestDataBuilder.CreateTestGenre(3, "Comedy");
            db.Genres.AddRange(genre1, genre2, genre3);
            db.SaveChanges();

            // Add movies
            var movie1 = TestDataBuilder.CreateTestMovie(1, genre1.Id, "The Thing");
            var movie2 = TestDataBuilder.CreateTestMovie(2, genre2.Id, "Dumb and Dumber");
            var movie3 = TestDataBuilder.CreateTestMovie(3, genre3.Id, "Test Movie 3");
            var movie4 = TestDataBuilder.CreateTestMovie(4, genre1.Id, "Test Movie 4");
            db.Movies.AddRange(movie1, movie2, movie3, movie4);
            db.SaveChanges();

            // Add user movies
            var userMovie1 = TestDataBuilder.CreateTestUserMovie(1, user1.Id, movie1.Id);
            var userMovie2 = TestDataBuilder.CreateTestUserMovie(2, user1.Id, movie2.Id);
            var userMovie3 = TestDataBuilder.CreateTestUserMovie(3, user2.Id, movie1.Id);
            var userMovie4 = TestDataBuilder.CreateTestUserMovie(4, user2.Id, movie2.Id);
            db.UserMovies.AddRange(userMovie1, userMovie2, userMovie3, userMovie4);
            db.SaveChanges();

            // Add viewings
            var viewing1 = TestDataBuilder.CreateTestViewing(1, user1.Id, movie1.Id);
            var viewing2 = TestDataBuilder.CreateTestViewing(2, user1.Id, movie2.Id);
            var viewing3 = TestDataBuilder.CreateTestViewing(3, user2.Id, movie3.Id);
            var viewing4 = TestDataBuilder.CreateTestViewing(4, user2.Id, movie4.Id);
            db.UserMovieViewings.AddRange(viewing1, viewing2, viewing3, viewing4);
            db.SaveChanges();

            // Add reviews
            var review1 = TestDataBuilder.CreateTestReview(1, viewing1.Id);
            var review2 = TestDataBuilder.CreateTestReview(2, viewing2.Id);
            db.Reviews.AddRange(review1, review2);
            db.SaveChanges();

            // Add EventTypeReferences (for audit tests)
            var eventTypes = new[]
            {
                new EventTypeReference { EventType = (EventType)1, Name = "Movie Added", Description = "A new movie was added to the system", IsActive = true },
                new EventTypeReference { EventType = (EventType)2, Name = "Movie Updated", Description = "An existing movie was updated", IsActive = true },
                new EventTypeReference { EventType = (EventType)3, Name = "Movie Deleted", Description = "A movie was deleted from the system", IsActive = true },
                new EventTypeReference { EventType = (EventType)4, Name = "Movie Favorited", Description = "A movie was marked as favorite or unfavorited", IsActive = true },
                new EventTypeReference { EventType = (EventType)5, Name = "Movie Added to Library", Description = "A movie was added to or removed from user library", IsActive = true },
                new EventTypeReference { EventType = (EventType)6, Name = "Review Added", Description = "A new review was added", IsActive = true },
                new EventTypeReference { EventType = (EventType)7, Name = "Review Updated", Description = "An existing review was updated", IsActive = true },
                new EventTypeReference { EventType = (EventType)8, Name = "Review Deleted", Description = "A review was deleted", IsActive = true },
                new EventTypeReference { EventType = (EventType)9, Name = "User Registered", Description = "A new user registered", IsActive = true },
                new EventTypeReference { EventType = (EventType)10, Name = "User Logged In", Description = "A user logged into the system", IsActive = true },
                new EventTypeReference { EventType = (EventType)11, Name = "User Profile Updated", Description = "A user updated their profile", IsActive = true }
            };
            db.EventTypeReferences.AddRange(eventTypes);
            db.SaveChanges();

            // Add EntityTypeReferences (for audit tests)
            var entityTypes = new[]
            {
                new EntityTypeReference { EntityType = (EntityType)1, Name = "Movie", Description = "Movie entities in the system", IsActive = true },
                new EntityTypeReference { EntityType = (EntityType)2, Name = "Review", Description = "Review entities in the system", IsActive = true },
                new EntityTypeReference { EntityType = (EntityType)3, Name = "User", Description = "User entities in the system", IsActive = true },
                new EntityTypeReference { EntityType = (EntityType)4, Name = "Genre", Description = "Genre entities in the system", IsActive = true },
                new EntityTypeReference { EntityType = (EntityType)5, Name = "Viewing", Description = "Viewing entities in the system", IsActive = true },
                new EntityTypeReference { EntityType = (EntityType)6, Name = "LibraryItem", Description = "Library item entities in the system", IsActive = true }
            };
            db.EntityTypeReferences.AddRange(entityTypes);
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("Error seeding test data", ex);
        }
    }
}
