namespace GestaoRestaurante.API.Models;

/// <summary>
/// Wrapper padronizado para respostas da API
/// </summary>
/// <typeparam name="T">Tipo dos dados</typeparam>
public class ApiResponseWrapper<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? RequestId { get; set; }
    public string? CorrelationId { get; set; }
    public ApiMetadata? Metadata { get; set; }

    public static ApiResponseWrapper<T> SuccessResponse(T data, string? message = null, ApiMetadata? metadata = null)
    {
        return new ApiResponseWrapper<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operação realizada com sucesso",
            Metadata = metadata
        };
    }

    public static ApiResponseWrapper<T> ErrorResponse(string message, IEnumerable<string>? errors = null)
    {
        return new ApiResponseWrapper<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? []
        };
    }

    public static ApiResponseWrapper<T> ErrorResponse(IEnumerable<string> errors)
    {
        return new ApiResponseWrapper<T>
        {
            Success = false,
            Message = "Erro na operação",
            Errors = errors
        };
    }
}

/// <summary>
/// Wrapper padronizado para respostas sem dados específicos
/// </summary>
public class ApiResponseWrapper : ApiResponseWrapper<object>
{
    public static ApiResponseWrapper SuccessResponse(string message = "Operação realizada com sucesso")
    {
        return new ApiResponseWrapper
        {
            Success = true,
            Message = message
        };
    }

    public new static ApiResponseWrapper ErrorResponse(string message, IEnumerable<string>? errors = null)
    {
        return new ApiResponseWrapper
        {
            Success = false,
            Message = message,
            Errors = errors ?? []
        };
    }
}

/// <summary>
/// Metadados adicionais da resposta
/// </summary>
public class ApiMetadata
{
    public PaginationMetadata? Pagination { get; set; }
    public Dictionary<string, object>? AdditionalInfo { get; set; }
}

/// <summary>
/// Metadados de paginação
/// </summary>
public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }

    public static PaginationMetadata FromPagedResult<T>(PagedResult<T> pagedResult)
    {
        return new PaginationMetadata
        {
            CurrentPage = pagedResult.PageIndex + 1, // Base 1 para UI
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages,
            TotalRecords = pagedResult.TotalCount,
            HasPrevious = pagedResult.HasPreviousPage,
            HasNext = pagedResult.HasNextPage
        };
    }
}

/// <summary>
/// Classe para importar PagedResult da Infrastructure
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => PageIndex + 1 < TotalPages;
}