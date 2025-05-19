using FluentValidation;
using Microsoft.OpenApi.Models;
using movielogger.api.mappings;
using movielogger.services.interfaces;
using movielogger.services.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);


        builder.Services
            .AddScoped<IUsersService, UsersService>()
            .AddScoped<IAccountsService, AccountsService>()
            .AddScoped<IGenresService, GenresService>()
            .AddScoped<ILibraryService, LibraryService>()
            .AddScoped<IMoviesService, MoviesService>()
            .AddScoped<IReviewsService, ReviewsService>()
            .AddScoped<IViewingsService, ViewingsService>();

        builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

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

