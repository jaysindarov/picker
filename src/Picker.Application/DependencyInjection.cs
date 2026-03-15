using Microsoft.Extensions.DependencyInjection;
using Picker.Application.Services.Implementations;
using Picker.Application.Services.Interfaces;

namespace Picker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICuisineService, CuisineService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IFoodService, FoodService>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IRatingService, RatingService>();
        return services;
    }
}
