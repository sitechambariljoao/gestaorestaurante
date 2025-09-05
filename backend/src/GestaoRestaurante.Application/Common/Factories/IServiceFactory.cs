namespace GestaoRestaurante.Application.Common.Factories;

/// <summary>
/// Factory para criação de services com configuração dinâmica
/// </summary>
public interface IServiceFactory<TService>
{
    TService CreateService();
    TService CreateService(string configuration);
    TService CreateService(Dictionary<string, object> parameters);
}

/// <summary>
/// Factory específica para validators
/// </summary>
public interface IValidatorFactory
{
    TValidator CreateValidator<TValidator, TModel>()
        where TValidator : class
        where TModel : class;
        
    IEnumerable<TValidator> CreateValidators<TValidator, TModel>()
        where TValidator : class
        where TModel : class;
}

/// <summary>
/// Factory para repositories com diferentes contextos
/// </summary>
public interface IRepositoryFactory
{
    TRepository CreateRepository<TRepository>() where TRepository : class;
    TRepository CreateRepository<TRepository>(string connectionString) where TRepository : class;
    TRepository CreateReadOnlyRepository<TRepository>() where TRepository : class;
}

/// <summary>
/// Factory para queries customizadas
/// </summary>
public interface IQueryFactory
{
    TQuery CreateQuery<TQuery>() where TQuery : class;
    TQuery CreateQuery<TQuery>(object parameters) where TQuery : class;
    TQuery CreateDynamicQuery<TQuery>(string queryType, Dictionary<string, object> filters) where TQuery : class;
}

/// <summary>
/// Factory para commands com validação
/// </summary>
public interface ICommandFactory
{
    TCommand CreateCommand<TCommand>() where TCommand : class;
    TCommand CreateCommand<TCommand>(object data) where TCommand : class;
    Task<TCommand> CreateValidatedCommandAsync<TCommand>(object data) where TCommand : class;
}

/// <summary>
/// Factory geral para services do sistema
/// </summary>
public interface IServiceFactory
{
    TService CreateService<TService>() where TService : class;
    object CreateService(Type serviceType);
    TValidator CreateValidator<TValidator>() where TValidator : class;
    TRepository CreateRepository<TRepository>() where TRepository : class;
    TQuery CreateQuery<TQuery>() where TQuery : class;
    TCommand CreateCommand<TCommand>() where TCommand : class;
}