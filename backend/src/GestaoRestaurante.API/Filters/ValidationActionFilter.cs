using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GestaoRestaurante.API.Models;

namespace GestaoRestaurante.API.Filters;

/// <summary>
/// Action Filter que valida automaticamente o ModelState e retorna erros padronizados
/// </summary>
public class ValidationActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            var response = new ApiResponseWrapper<object>
            {
                Success = false,
                Data = null,
                Message = "Dados inválidos fornecidos",
                Errors = errors.SelectMany(kvp => kvp.Value).ToList(),
                Timestamp = DateTime.UtcNow,
                CorrelationId = context.HttpContext.TraceIdentifier
            };

            context.Result = new BadRequestObjectResult(response);
        }

        base.OnActionExecuting(context);
    }
}