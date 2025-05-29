using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using movielogger.dal;

namespace movielogger.api.tests.fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AssessmentDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AssessmentDbContext));
            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }
            
            services.AddDbContext<AssessmentDbContext>(options =>
            {
                // TODO - Temp fix to stop running into ID conflicts. Check for better fix.
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
            });

            services.AddScoped<IAssessmentDbContext>(provider =>
                provider.GetRequiredService<AssessmentDbContext>());
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AssessmentDbContext>();
            db.Database.EnsureCreated();
            
            SeedTestData(db);
        });
    }

    private static void SeedTestData(AssessmentDbContext db)
    {
        db.Reviews.RemoveRange(db.Reviews);
        db.Viewings.RemoveRange(db.Viewings);
        db.UserMovies.RemoveRange(db.UserMovies);
        db.Movies.RemoveRange(db.Movies);
        db.Users.RemoveRange(db.Users);
        db.Genres.RemoveRange(db.Genres);
        db.SaveChanges();
        
        db.Users.AddRange(
            new dal.entities.User
            {
                Id = 1, 
                UserName = "John Doe", 
                Email = "johndoe@gmail.com", 
                Password = "password123"
            },    
            new dal.entities.User
            {
                Id = 2, 
                UserName = "Jane Doe", 
                Email = "janedoe@gmail.com", 
                Password = "password456"
            }
        );
        
        db.Genres.AddRange(
            new dal.entities.Genre { Id = 1, Title = "Horror" },
            new dal.entities.Genre { Id = 2, Title = "Comedy" }
        );

        db.Movies.AddRange(
            new dal.entities.Movie
            {
                Id = 1, 
                Title = "The Thing", 
                GenreId = 1, 
                Description = "Scary movie", 
                IsDeleted = false, 
                ReleaseDate = DateTime.Now.AddYears(-1)
            },
            new dal.entities.Movie
            {
                Id = 2, 
                Title = "Dumb and Dumber", 
                GenreId = 2, 
                Description = "Funny movie", 
                IsDeleted = false, 
                ReleaseDate = DateTime.Now.AddYears(-2)
            }
        );
        
        db.UserMovies.AddRange(
            new dal.entities.UserMovie
            {
                Id = 1,
                UserId = 1,
                MovieId = 1
            },
            new dal.entities.UserMovie
            {
                Id = 2,
                UserId = 1,
                MovieId = 2
            },
            new dal.entities.UserMovie
            {
                Id = 3,
                UserId = 2,
                MovieId = 1
            },
            new dal.entities.UserMovie
            {
                Id = 4,
                UserId = 2,
                MovieId = 2
            }
        );
        
        db.Viewings.AddRange(
            new dal.entities.Viewing
            {
                Id = 1,
                DateViewed = DateTime.Now.AddDays(-1),
                UserMovieId = 1
            },   
            new dal.entities.Viewing
            {
                Id = 2,
                DateViewed = DateTime.Now.AddDays(-2),
                UserMovieId = 2
            },
            new dal.entities.Viewing
            {
                Id = 3,
                DateViewed = DateTime.Now.AddDays(-3),
                UserMovieId = 3
            },   
            new dal.entities.Viewing
            {
                Id = 4,
                DateViewed = DateTime.Now.AddDays(-4),
                UserMovieId = 4
            }
        );
        
        db.Reviews.AddRange(
            new dal.entities.Review
            {
                Id = 1,
                ReviewText = "Review Text 1",
                Score = 4,
                ViewingId = 1
            },
            new dal.entities.Review
            {
                Id = 2,
                ReviewText = "Review Text 2",
                Score = 5,
                ViewingId = 2
            }
        );
        
        db.SaveChanges();
    }
}
