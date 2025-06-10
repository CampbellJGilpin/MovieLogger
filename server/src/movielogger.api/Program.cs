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
using movielogger.dal.extensions;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.SetIsOriginAllowed(origin => true)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

        // Configure JWT authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
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
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception}");
                    return Task.CompletedTask;
                }
            };
        });

        var isTesting = builder.Environment.EnvironmentName == "Testing";

        if (!isTesting)
        {
            var dbConnection = "DATABASE_URL".GetValue();
            
            builder.Services.AddDbContext<AssessmentDbContext>(options =>
                options.UseNpgsql(dbConnection ?? builder.Configuration.GetConnectionString("DefaultConnection")));
            
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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();

        // Important: UseAuthentication must come before UseAuthorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapGet("/", () => "MovieLogger API");

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

