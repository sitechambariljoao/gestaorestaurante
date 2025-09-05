using System.Net;
using System.Text.Json;
using GestaoRestaurante.API.Models;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.API.Middleware;

/// <summary>
/// Middleware global para tratamento de exceções
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var requestId = context.TraceIdentifier;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var clientIp = GetClientIpAddress(context);

        // Log estruturado da exceção
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["RequestPath"] = requestPath,
            ["RequestMethod"] = requestMethod,
            ["ClientIp"] = clientIp,
            ["UserAgent"] = userAgent,
            ["ExceptionType"] = exception.GetType().Name
        });

        // Determinar resposta baseada no tipo de exceção
        var (statusCode, response) = CreateErrorResponse(exception, requestId);

        // Log com nível apropriado
        LogException(exception, statusCode, requestPath);

        // Configurar resposta HTTP
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        // Serializar e enviar resposta
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private (HttpStatusCode statusCode, ApiResponseWrapper response) CreateErrorResponse(Exception exception, string requestId)
    {
        return exception switch
        {
            // Exceções de validação do FluentValidation
            FluentValidation.ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                CreateValidationErrorResponse(validationEx, requestId)
            ),

            // Exceções de domínio customizadas
            DomainValidationException domainEx => (
                HttpStatusCode.BadRequest,
                ApiResponseWrapper.ErrorResponse(
                    "Erro de validação de domínio", 
                    new[] { domainEx.Message }
                )
            ),

            BusinessRuleException businessEx => (
                HttpStatusCode.UnprocessableEntity,
                ApiResponseWrapper.ErrorResponse(
                    "Regra de negócio violada", 
                    new[] { businessEx.Message }
                )
            ),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                ApiResponseWrapper.ErrorResponse(
                    "Recurso não encontrado", 
                    new[] { notFoundEx.Message }
                )
            ),

            UnauthorizedAccessException _ => (
                HttpStatusCode.Unauthorized,
                ApiResponseWrapper.ErrorResponse(
                    "Acesso não autorizado", 
                    new[] { "Você não tem permissão para acessar este recurso" }
                )
            ),

            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                ApiResponseWrapper.ErrorResponse(
                    "Parâmetro inválido", 
                    new[] { argEx.Message }
                )
            ),

            TimeoutException _ => (
                HttpStatusCode.RequestTimeout,
                ApiResponseWrapper.ErrorResponse(
                    "Timeout na operação", 
                    new[] { "A operação demorou muito para ser concluída" }
                )
            ),

            InvalidOperationException invalidOpEx => (
                HttpStatusCode.Conflict,
                ApiResponseWrapper.ErrorResponse(
                    "Operação inválida", 
                    new[] { invalidOpEx.Message }
                )
            ),

            // Exceções genéricas
            _ => (
                HttpStatusCode.InternalServerError,
                CreateGenericErrorResponse(exception, requestId)
            )
        };
    }

    private ApiResponseWrapper CreateValidationErrorResponse(FluentValidation.ValidationException validationEx, string requestId)
    {
        var errors = validationEx.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(e => e.ErrorMessage).ToArray()
            );

        return new ApiResponseWrapper
        {
            Success = false,
            Message = "Dados inválidos fornecidos",
            Errors = errors.SelectMany(kvp => kvp.Value),
            RequestId = requestId,
            Timestamp = DateTime.UtcNow
        };
    }

    private ApiResponseWrapper CreateGenericErrorResponse(Exception exception, string requestId)
    {
        var message = _environment.IsDevelopment() 
            ? exception.Message 
            : "Ocorreu um erro interno no servidor";

        var errors = _environment.IsDevelopment() && exception.StackTrace != null
            ? new[] { exception.StackTrace }
            : Array.Empty<string>();

        return new ApiResponseWrapper
        {
            Success = false,
            Message = message,
            Errors = errors,
            RequestId = requestId,
            Timestamp = DateTime.UtcNow
        };
    }

    private void LogException(Exception exception, HttpStatusCode statusCode, string requestPath)
    {
        var logLevel = statusCode switch
        {
            HttpStatusCode.BadRequest => LogLevel.Warning,
            HttpStatusCode.Unauthorized => LogLevel.Warning,
            HttpStatusCode.Forbidden => LogLevel.Warning,
            HttpStatusCode.NotFound => LogLevel.Information,
            HttpStatusCode.UnprocessableEntity => LogLevel.Warning,
            HttpStatusCode.InternalServerError => LogLevel.Error,
            _ => LogLevel.Error
        };

        _logger.Log(logLevel, exception,
            "Exceção capturada: {ExceptionType} - {StatusCode} - Path: {RequestPath} - Message: {Message}",
            exception.GetType().Name, statusCode, requestPath, exception.Message);

        // Log crítico para erros 500
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogCritical(exception,
                "ERRO CRÍTICO: Exceção não tratada resultou em 500 - Path: {RequestPath}",
                requestPath);
        }
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Verificar headers de proxy reverso
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
        {
            return forwarded.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

/// <summary>
/// Extensão para registrar o middleware
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}