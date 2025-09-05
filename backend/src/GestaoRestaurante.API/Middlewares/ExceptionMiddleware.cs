using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado na requisição {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            // Custom Domain Exceptions
            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Erro de validação";
                response.Details = validationEx.Message;
                response.ValidationErrors = validationEx.Errors;
                break;

            case BusinessRuleException businessRuleEx:
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Message = "Regra de negócio violada";
                response.Details = businessRuleEx.Message;
                response.RuleName = businessRuleEx.RuleName;
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Recurso não encontrado";
                response.Details = notFoundEx.Message;
                response.EntityName = notFoundEx.EntityName;
                break;

            case UnauthorizedException unauthorizedEx:
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.Message = "Acesso negado";
                response.Details = unauthorizedEx.Message;
                response.Resource = unauthorizedEx.Resource;
                break;

            case ConcurrencyException concurrencyEx:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = "Conflito de concorrência";
                response.Details = concurrencyEx.Message;
                break;

            // Standard .NET Exceptions
            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Dados inválidos fornecidos";
                response.Details = argEx.Message;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Acesso não autorizado";
                response.Details = "Você não tem permissão para acessar este recurso";
                break;

            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Recurso não encontrado";
                response.Details = exception.Message;
                break;

            case InvalidOperationException invOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Operação inválida";
                response.Details = invOpEx.Message;
                break;

            case DbUpdateException dbEx:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = "Erro de banco de dados";
                
                if (dbEx.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    response.Details = "Já existe um registro com essas informações";
                }
                else if (dbEx.InnerException?.Message.Contains("FOREIGN KEY") == true)
                {
                    response.Details = "Violação de integridade referencial";
                }
                else
                {
                    response.Details = "Erro ao salvar dados no banco";
                }
                break;

            case TimeoutException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.Message = "Timeout na requisição";
                response.Details = "A operação demorou muito para ser concluída";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Erro interno do servidor";
                
                if (_environment.IsDevelopment())
                {
                    response.Details = exception.Message;
                    response.StackTrace = exception.StackTrace;
                }
                else
                {
                    response.Details = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
                }
                break;
        }

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = Guid.NewGuid().ToString();
    
    // Custom exception properties
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; set; }
    public string? RuleName { get; set; }
    public string? EntityName { get; set; }
    public string? Resource { get; set; }
}