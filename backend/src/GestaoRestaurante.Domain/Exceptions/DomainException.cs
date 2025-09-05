namespace GestaoRestaurante.Domain.Exceptions;

/// <summary>
/// Base exception for domain-specific errors
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for business rule violations
/// </summary>
public class BusinessRuleException : DomainException
{
    public string RuleName { get; }
    
    public BusinessRuleException(string ruleName, string message) : base(message)
    {
        RuleName = ruleName;
    }
    
    public BusinessRuleException(string ruleName, string message, Exception innerException) : base(message, innerException)
    {
        RuleName = ruleName;
    }
}

/// <summary>
/// Exception for validation errors
/// </summary>
public class ValidationException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(IDictionary<string, string[]> errors) 
        : base("Uma ou mais validações falharam")
    {
        Errors = errors.ToDictionary(x => x.Key, x => x.Value);
    }
    
    public ValidationException(string field, string error) : base($"Validation failed for {field}: {error}")
    {
        Errors = new Dictionary<string, string[]> { { field, new[] { error } } };
    }
}

/// <summary>
/// Exception for entity not found scenarios
/// </summary>
public class NotFoundException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }
    
    public NotFoundException(string entityName, object entityId) 
        : base($"{entityName} with ID '{entityId}' was not found")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}

/// <summary>
/// Exception for concurrency conflicts
/// </summary>
public class ConcurrencyException : DomainException
{
    public ConcurrencyException() : base("The record was modified by another user. Please reload and try again.") { }
    
    public ConcurrencyException(string message) : base(message) { }
}

/// <summary>
/// Exception for unauthorized access
/// </summary>
public class UnauthorizedException : DomainException
{
    public string Resource { get; }
    
    public UnauthorizedException(string resource) : base($"Access denied to resource: {resource}")
    {
        Resource = resource;
    }
    
    public UnauthorizedException(string message, string resource) : base(message)
    {
        Resource = resource;
    }
}

/// <summary>
/// Exception for configuration errors
/// </summary>
public class ConfigurationException : DomainException
{
    public string ConfigurationKey { get; }
    
    public ConfigurationException(string configurationKey, string message) : base(message)
    {
        ConfigurationKey = configurationKey;
    }
}

/// <summary>
/// Exception for external service integration errors
/// </summary>
public class ExternalServiceException : DomainException
{
    public string ServiceName { get; }
    public string? ServiceResponse { get; }
    
    public ExternalServiceException(string serviceName, string message) : base(message)
    {
        ServiceName = serviceName;
    }
    
    public ExternalServiceException(string serviceName, string message, string serviceResponse) : base(message)
    {
        ServiceName = serviceName;
        ServiceResponse = serviceResponse;
    }
    
    public ExternalServiceException(string serviceName, string message, Exception innerException) 
        : base(message, innerException)
    {
        ServiceName = serviceName;
    }
}

/// <summary>
/// Exception for duplicate entity scenarios
/// </summary>
public class DuplicateEntityException : DomainException
{
    public string EntityName { get; }
    public string Field { get; }
    public object Value { get; }
    
    public DuplicateEntityException(string entityName, string field, object value) 
        : base($"{entityName} with {field} '{value}' already exists")
    {
        EntityName = entityName;
        Field = field;
        Value = value;
    }
}

/// <summary>
/// Exception for operations not allowed in current state
/// </summary>
public class InvalidStateException : DomainException
{
    public string CurrentState { get; }
    public string RequestedOperation { get; }
    
    public InvalidStateException(string currentState, string requestedOperation) 
        : base($"Cannot perform operation '{requestedOperation}' in current state '{currentState}'")
    {
        CurrentState = currentState;
        RequestedOperation = requestedOperation;
    }
}