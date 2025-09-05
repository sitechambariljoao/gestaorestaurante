using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Infrastructure.Data.Context;
using GestaoRestaurante.Infrastructure.Data.SeedData;

namespace GestaoRestaurante.Shared.Configurations;

public static class DataSeederConfiguration
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GestaoRestauranteContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        
        try
        {
            await DataSeeder.SeedAsync(context, userManager);
            logger.LogInformation("Dados iniciais carregados com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao carregar dados iniciais");
        }
    }
}