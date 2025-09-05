using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GestaoRestaurante.API.Models;
using System.Text.Json;

namespace GestaoRestaurante.API.Filters;

/// <summary>
/// Filter que envolve automaticamente as respostas em um wrapper padronizado
/// </summary>
public class ResponseWrapperFilter : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);

        // Só aplicar wrapper para responses de sucesso
        if (context.Result is ObjectResult objectResult && 
            objectResult.StatusCode >= 200 && 
            objectResult.StatusCode < 300)
        {
            var wrapper = new ApiResponseWrapper<object>
            {
                Success = true,
                Data = objectResult.Value,
                Message = "Operação realizada com sucesso",
                Timestamp = DateTime.UtcNow,
                CorrelationId = context.HttpContext.TraceIdentifier
            };

            context.Result = new ObjectResult(wrapper)
            {
                StatusCode = objectResult.StatusCode
            };
        }
    }

    private static bool ShouldWrapResponse(ActionExecutedContext context)
    {
        // Não aplicar wrapper para:
        // 1. Responses que já são ApiResponseWrapper
        // 2. Endpoints de Health Check
        // 3. Swagger endpoints
        // 4. Responses de erro (serão tratados pelo middleware de exceções)
        
        if (context.Result is ObjectResult objectResult)
        {
            // Já é um wrapper
            if (objectResult.Value?.GetType().Name.Contains("ApiResponseWrapper") == true)
                return false;
        }

        var actionName = context.ActionDescriptor.DisplayName?.ToLower() ?? "";
        
        // Não aplicar wrapper para endpoints específicos
        return !actionName.Contains("health") && 
               !actionName.Contains("swagger") &&
               !actionName.Contains("metrics");
    }

    private static ObjectResult CreateWrappedResponse(ObjectResult original)
    {
        var wrapper = new ApiResponseWrapper<object>
        {
            Success = true,
            Data = original.Value,
            Message = "Operação realizada com sucesso",
            Timestamp = DateTime.UtcNow
        };

        return new ObjectResult(wrapper)
        {
            StatusCode = original.StatusCode
        };
    }
}