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
using movielogger.dal.extensions;
using movielogger.messaging.Configuration;
using movielogger.messaging.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

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
        });

        // Configure RabbitMQ
        builder.Services.Configure<RabbitMQConfig>(
            builder.Configuration.GetSection("RabbitMQ"));

        var isTesting = builder.Environment.EnvironmentName == "Testing";

        if (!isTesting)
        {
            var dbConnection = "DATABASE_URL".GetValue();
            
            builder.Services.AddDbContext<MovieLoggerDbContext>(options =>
                options.UseNpgsql(dbConnection ?? builder.Configuration.GetConnectionString("DefaultConnection")));
            
            builder.Services.AddScoped<IAssessmentDbContext>(provider => 
                provider.GetRequiredService<MovieLoggerDbContext>());
        }

        // Add Memory Cache
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<ICacheService, InMemoryCacheService>();

        builder.Services
            .AddScoped<IUsersService, UsersService>()
            .AddScoped<IAccountsService, AccountsService>()
            .AddScoped<IGenresService, GenresService>()
            .AddScoped<ILibraryService, LibraryService>()
            .AddScoped<IMoviesService, MoviesService>()
            .AddScoped<CachedMoviesService>()
            .AddScoped<MoviesServiceFactory>()
            .AddScoped<IReviewsService, ReviewsService>()
            .AddScoped<IViewingsService, ViewingsService>()
            .AddScoped<IAuditService, AuditService>();

        // Configure MoviesService with factory pattern
        builder.Services.AddScoped<IMoviesService>(provider =>
        {
            var factory = provider.GetRequiredService<MoviesServiceFactory>();
            return factory.Create();
        });

        // Register RabbitMQ services
        builder.Services
            .AddSingleton<IMessagePublisher, RabbitMQPublisher>()
            .AddSingleton<IMessageConsumer, RabbitMQConsumer>()
            .AddScoped<AuditEventConsumer>()
            .AddHostedService<AuditEventConsumerHostedService>();

        builder.Services.AddAutoMapper(
            typeof(ApiMappingProfile).Assembly,
            typeof(ServicesMappingProfile).Assembly);

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddEndpointsApiExplorer();
        
        // Add Health Checks
        builder.Services.AddHealthChecks();

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

        if (!app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ASPNETCORE_USE_HTTPS_REDIRECTION") == "true")
        {
            app.UseHttpsRedirection();
        }
        
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Map the health check endpoint
        app.MapHealthChecks("/health");

        app.Run();
    }
}
