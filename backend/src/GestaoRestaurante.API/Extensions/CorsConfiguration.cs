using Microsoft.Extensions.DependencyInjection;

namespace GestaoRestaurante.API.Extensions;

public static class CorsConfiguration
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Development", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            options.AddPolicy("Production", builder =>
            {
                builder.WithOrigins("https://yourdomain.com")
                       .WithMethods("GET", "POST", "PUT", "DELETE")
                       .WithHeaders("Content-Type", "Authorization");
            });
        });

        return services;
    }
}