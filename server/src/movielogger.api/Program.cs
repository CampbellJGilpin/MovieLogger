using FluentValidation;
using Microsoft.OpenApi.Models;
using movielogger.api.mappings;
using movielogger.services.interfaces;
using movielogger.services.services;

var builder = WebApplication.CreateBuilder(args);

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();