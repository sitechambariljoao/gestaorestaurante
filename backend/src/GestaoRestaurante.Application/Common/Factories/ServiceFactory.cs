using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Factories;
using FluentValidation;
using AutoMapper;

namespace GestaoRestaurante.Application.Common.Factories;

/// <summary>
/// Implementação base do service factory
/// </summary>
public class ServiceFactory<TService> : IServiceFactory<TService> where TService : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ServiceFactory<TService>> _logger;

    public ServiceFactory(IServiceProvider serviceProvider, ILogger<ServiceFactory<TService>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public virtual TService CreateService()
    {
        try
        {
            var service = _serviceProvider.GetRequiredService<TService>();
            _logger.LogDebug("Service {ServiceType} criado com sucesso", typeof(TService).Name);
            return service;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar service {ServiceType}", typeof(TService).Name);
            throw;
        }
    }

    public virtual TService CreateService(string configuration)
    {
        // Implementação base - pode ser sobrescrita para configuração específica
        return CreateService();
    }

    public virtual TService CreateService(Dictionary<string, object> parameters)
    {
        // Implementação base - pode ser sobrescrita para parâmetros específicos
        return CreateService();
    }
}

/// <summary>
/// Factory específica para validators
/// </summary>
public class ValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidatorFactory> _logger;

    public ValidatorFactory(IServiceProvider serviceProvider, ILogger<ValidatorFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public TValidator CreateValidator<TValidator, TModel>()
        where TValidator : class
        where TModel : class
    {
        try
        {
            var validator = _serviceProvider.GetRequiredService<TValidator>();
            _logger.LogDebug("Validator {ValidatorType} para {ModelType} criado", typeof(TValidator).Name, typeof(TModel).Name);
            return validator;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar validator {ValidatorType}", typeof(TValidator).Name);
            throw;
        }
    }

    public IEnumerable<TValidator> CreateValidators<TValidator, TModel>()
        where TValidator : class
        where TModel : class
    {
        try
        {
            var validators = _serviceProvider.GetServices<TValidator>();
            _logger.LogDebug("Validators {ValidatorType} para {ModelType} criados: {Count}", 
                typeof(TValidator).Name, typeof(TModel).Name, validators.Count());
            return validators;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar validators {ValidatorType}", typeof(TValidator).Name);
            throw;
        }
    }
}

/// <summary>
/// Factory para repositories
/// </summary>
public class RepositoryFactory : IRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RepositoryFactory> _logger;

    public RepositoryFactory(IServiceProvider serviceProvider, ILogger<RepositoryFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public TRepository CreateRepository<TRepository>() where TRepository : class
    {
        try
        {
            var repository = _serviceProvider.GetRequiredService<TRepository>();
            _logger.LogDebug("Repository {RepositoryType} criado", typeof(TRepository).Name);
            return repository;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar repository {RepositoryType}", typeof(TRepository).Name);
            throw;
        }
    }

    public TRepository CreateRepository<TRepository>(string connectionString) where TRepository : class
    {
        // Para futura implementação com connection strings dinâmicas
        _logger.LogDebug("Repository {RepositoryType} solicitado com connection string customizada", typeof(TRepository).Name);
        return CreateRepository<TRepository>();
    }

    public TRepository CreateReadOnlyRepository<TRepository>() where TRepository : class
    {
        // Para futura implementação com contextos read-only
        _logger.LogDebug("Repository {RepositoryType} read-only solicitado", typeof(TRepository).Name);
        return CreateRepository<TRepository>();
    }
}

/// <summary>
/// Factory para queries
/// </summary>
public class QueryFactory : IQueryFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<QueryFactory> _logger;

    public QueryFactory(IServiceProvider serviceProvider, IMapper mapper, ILogger<QueryFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _logger = logger;
    }

    public TQuery CreateQuery<TQuery>() where TQuery : class
    {
        try
        {
            // Tentar criar via DI primeiro
            var query = _serviceProvider.GetService<TQuery>();
            if (query != null)
            {
                _logger.LogDebug("Query {QueryType} criada via DI", typeof(TQuery).Name);
                return query;
            }

            // Fallback para Activator
            query = Activator.CreateInstance<TQuery>();
            _logger.LogDebug("Query {QueryType} criada via Activator", typeof(TQuery).Name);
            return query;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar query {QueryType}", typeof(TQuery).Name);
            throw;
        }
    }

    public TQuery CreateQuery<TQuery>(object parameters) where TQuery : class
    {
        var query = CreateQuery<TQuery>();
        
        // Mapear parâmetros usando AutoMapper ou reflection
        if (parameters != null)
        {
            try
            {
                _mapper.Map(parameters, query);
                _logger.LogDebug("Parâmetros mapeados para query {QueryType}", typeof(TQuery).Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao mapear parâmetros para query {QueryType}", typeof(TQuery).Name);
            }
        }

        return query;
    }

    public TQuery CreateDynamicQuery<TQuery>(string queryType, Dictionary<string, object> filters) where TQuery : class
    {
        var query = CreateQuery<TQuery>();
        
        _logger.LogDebug("Query dinâmica {QueryType} criada com {FilterCount} filtros", 
            typeof(TQuery).Name, filters.Count);
        
        // Implementar lógica de filtros dinâmicos conforme necessário
        return query;
    }
}

/// <summary>
/// Factory para commands
/// </summary>
public class CommandFactory : ICommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<CommandFactory> _logger;

    public CommandFactory(IServiceProvider serviceProvider, IMapper mapper, ILogger<CommandFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _logger = logger;
    }

    public TCommand CreateCommand<TCommand>() where TCommand : class
    {
        try
        {
            var command = _serviceProvider.GetService<TCommand>() ?? Activator.CreateInstance<TCommand>();
            _logger.LogDebug("Command {CommandType} criado", typeof(TCommand).Name);
            return command;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar command {CommandType}", typeof(TCommand).Name);
            throw;
        }
    }

    public TCommand CreateCommand<TCommand>(object data) where TCommand : class
    {
        var command = CreateCommand<TCommand>();
        
        if (data != null)
        {
            try
            {
                _mapper.Map(data, command);
                _logger.LogDebug("Dados mapeados para command {CommandType}", typeof(TCommand).Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao mapear dados para command {CommandType}", typeof(TCommand).Name);
            }
        }

        return command;
    }

    public async Task<TCommand> CreateValidatedCommandAsync<TCommand>(object data) where TCommand : class
    {
        var command = CreateCommand<TCommand>(data);
        
        // Tentar validar usando FluentValidation
        try
        {
            var validator = _serviceProvider.GetService<IValidator<TCommand>>();
            if (validator != null)
            {
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    throw new ValidationException($"Command {typeof(TCommand).Name} inválido: {errors}");
                }
                
                _logger.LogDebug("Command {CommandType} validado com sucesso", typeof(TCommand).Name);
            }
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro na validação do command {CommandType}", typeof(TCommand).Name);
        }

        return command;
    }
}

/// <summary>
/// Factory geral simples para services do sistema
/// </summary>
public class SimpleServiceFactory : IServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SimpleServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TService CreateService<TService>() where TService : class
    {
        return _serviceProvider.GetRequiredService<TService>();
    }

    public object CreateService(Type serviceType)
    {
        return _serviceProvider.GetRequiredService(serviceType);
    }

    public TValidator CreateValidator<TValidator>() where TValidator : class
    {
        return _serviceProvider.GetRequiredService<TValidator>();
    }

    public TRepository CreateRepository<TRepository>() where TRepository : class
    {
        return _serviceProvider.GetRequiredService<TRepository>();
    }

    public TQuery CreateQuery<TQuery>() where TQuery : class
    {
        return _serviceProvider.GetRequiredService<TQuery>();
    }

    public TCommand CreateCommand<TCommand>() where TCommand : class
    {
        return _serviceProvider.GetRequiredService<TCommand>();
    }
}