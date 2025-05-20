using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using movielogger.services.interfaces;
using NSubstitute;

namespace movielogger.api.tests.fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IMoviesService MoviesServiceMock { get; }
    public IGenresService GenresServiceMock { get; }
    public IReviewsService ReviewsServiceMock { get; }
    public IUsersService UsersService { get; }

    public CustomWebApplicationFactory()
    {
        MoviesServiceMock = Substitute.For<IMoviesService>();
        GenresServiceMock = Substitute.For<IGenresService>();
        ReviewsServiceMock = Substitute.For<IReviewsService>();
        UsersService = Substitute.For<IUsersService>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IMoviesService));
            services.RemoveAll(typeof(IGenresService));
            services.RemoveAll(typeof(IReviewsService));
            services.RemoveAll(typeof(IUsersService));
            services.AddSingleton(MoviesServiceMock);
            services.AddSingleton(GenresServiceMock);
            services.AddSingleton(ReviewsServiceMock);
            services.AddSingleton(UsersService);
        });
    }
}