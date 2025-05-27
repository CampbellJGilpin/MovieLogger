using FluentValidation;
using Microsoft.OpenApi.Models;
using movielogger.api.mappings;
using movielogger.services.interfaces;
using movielogger.services.services;
using System.Text;
using movielogger.services.mapping;
using movielogger.dal;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        builder.Services.AddDbContext<AssessmentDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IAssessmentDbContext>(provider => provider.GetRequiredService<AssessmentDbContext>());

        builder.Services
            .AddScoped<IUsersService, UsersService>()
            .AddScoped<IAccountsService, AccountsService>()
            .AddScoped<IGenresService, GenresService>()
            .AddScoped<ILibraryService, LibraryService>()
            .AddScoped<IMoviesService, MoviesService>()
            .AddScoped<IReviewsService, ReviewsService>()
            .AddScoped<IViewingsService, ViewingsService>();

        builder.Services.AddAutoMapper(
            typeof(ApiMappingProfile).Assembly,
            typeof(ServicesMappingProfile).Assembly);

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieLogger API", Version = "v1" });
        });

        builder.Services.AddControllers();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}

