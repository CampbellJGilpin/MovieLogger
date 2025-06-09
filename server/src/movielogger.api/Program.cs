using FluentValidation;
using Microsoft.OpenApi.Models;
using movielogger.api.mappings;
using movielogger.services.interfaces;
using movielogger.services.services;
using System.Text;
using movielogger.services.mapping;
using movielogger.dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.conventions;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

        // Configure JWT authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        var isTesting = builder.Environment.EnvironmentName == "Testing";

        if (!isTesting)
        {
            builder.Services.AddDbContext<AssessmentDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            builder.Services.AddScoped<IAssessmentDbContext>(provider => 
                provider.GetRequiredService<AssessmentDbContext>());
        }

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
            
            // Configure Swagger to use JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Add controllers with API route prefix
        builder.Services.AddControllers(options =>
        {
            options.UseGeneralRoutePrefix("api");
        });

        var app = builder.Build();

        // Enable CORS - must be before other middleware
        app.UseCors();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

public static class MvcOptionsExtensions
{
    public static void UseGeneralRoutePrefix(this MvcOptions options, string prefix)
    {
        options.Conventions.Add(new RoutePrefixConvention(new RouteAttribute(prefix)));
    }
}

