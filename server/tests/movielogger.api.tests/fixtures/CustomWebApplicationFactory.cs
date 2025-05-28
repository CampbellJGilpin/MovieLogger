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
                options.UseInMemoryDatabase("TestDb");
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
        db.Genres.RemoveRange(db.Genres);
        db.SaveChanges();
        
        db.Genres.AddRange(
            new dal.entities.Genre { Title = "Drama" },
            new dal.entities.Genre { Title = "Comedy" }
        );

        db.SaveChanges();
    }
}
