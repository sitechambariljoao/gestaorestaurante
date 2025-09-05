using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Services;
using GestaoRestaurante.Application.Mappings;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Application.Common.Factories;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Performance;
using GestaoRestaurante.Application.Common.Events;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Application.Common.Monitoring;
using GestaoRestaurante.Domain.Services;

namespace GestaoRestaurante.Shared.Configurations;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<EmpresaMappingProfile>());

        // AutoMapper
        services.AddAutoMapper(typeof(EmpresaMappingProfile));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateEmpresaDtoValidator>();
        
        // Validadores customizados de banco de dados
        services.AddScoped<CreateEmpresaDbValidator>();
        services.AddScoped<UpdateEmpresaDbValidator>();
        services.AddScoped<CreateAgrupamentoDbValidator>();
        services.AddScoped<UpdateAgrupamentoDbValidator>();
        services.AddScoped<CreateFilialDbValidator>();
        services.AddScoped<UpdateFilialDbValidator>();
        services.AddScoped<CreateSubAgrupamentoDbValidator>();
        services.AddScoped<UpdateSubAgrupamentoDbValidator>();
        services.AddScoped<CreateCentroCustoDbValidator>();
        services.AddScoped<UpdateCentroCustoDbValidator>();
        services.AddScoped<CreateCategoriaDbValidator>();
        services.AddScoped<UpdateCategoriaDbValidator>();
        services.AddScoped<CreateProdutoDbValidator>();
        services.AddScoped<UpdateProdutoDbValidator>();

        // Domain Event Publisher
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

        // Caching Services
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();

        // Action Filters - Registered in API layer since they require MVC dependencies

        // Domain Services
        services.AddScoped<IEmpresaDomainService, EmpresaDomainService>();
        services.AddScoped<ICategoriaDomainService, CategoriaDomainService>();
        services.AddScoped<IProdutoDomainService, ProdutoDomainService>();

        // Application Services
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<IFilialService, FilialService>();
        services.AddScoped<IAgrupamentoService, AgrupamentoService>();
        services.AddScoped<ISubAgrupamentoService, SubAgrupamentoService>();
        services.AddScoped<ICentroCustoService, CentroCustoService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IAuthService, AuthService>();

        // Fase 5 - Performance & Quality Services
        services.AddSingleton<IPerformanceProfiler, PerformanceProfiler>();
        // services.AddSingleton<IApplicationMetrics, StubApplicationMetrics>();
        
        // Factories
        services.AddScoped(typeof(IServiceFactory<>), typeof(ServiceFactory<>));
        services.AddScoped<Application.Common.Factories.IValidatorFactory, ValidatorFactory>();
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        services.AddScoped<IQueryFactory, QueryFactory>();
        services.AddScoped<ICommandFactory, CommandFactory>();

        return services;
    }
}