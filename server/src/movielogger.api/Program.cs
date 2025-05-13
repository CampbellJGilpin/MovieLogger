using FluentValidation;
using FluentValidation.AspNetCore;
using movielogger.api.validators;
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

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<AddMovieRequestValidator>();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();