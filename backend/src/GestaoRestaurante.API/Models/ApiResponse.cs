namespace GestaoRestaurante.API.Models;

/// <summary>
/// Resposta padrão da API com informações de erro
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public static ApiResponse SuccessResponse(object? data = null, string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse ErrorResponse(string? message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

/// <summary>
/// Resposta padrão para erros de validação
/// </summary>
public class ValidationErrorResponse
{
    public string Message { get; set; } = "Erro de validação";
    public Dictionary<string, string[]> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = string.Empty;
}