using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Infrastructure.Data.Context;
using GestaoRestaurante.Infrastructure.Data.Repositories;

namespace GestaoRestaurante.Shared.Configurations;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<GestaoRestauranteContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IFilialRepository, FilialRepository>();
        services.AddScoped<IAgrupamentoRepository, AgrupamentoRepository>();
        services.AddScoped<ISubAgrupamentoRepository, SubAgrupamentoRepository>();
        services.AddScoped<ICentroCustoRepository, CentroCustoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();

        return services;
    }
}