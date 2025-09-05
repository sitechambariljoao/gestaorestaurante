using Microsoft.AspNetCore.Authorization;

namespace GestaoRestaurante.API.Authorization;

public class ModuleAuthorizationHandler : AuthorizationHandler<ModuleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ModuleRequirement requirement)
    {
        // Verificar se o usuário está autenticado
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Verificar se o usuário está ativo
        var ativoClaimValue = context.User.FindFirst("Ativo")?.Value;
        if (ativoClaimValue != "True")
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Obter módulos liberados do token
        var modulosLiberados = context.User.FindAll("Modulo").Select(c => c.Value).ToList();

        // Verificar se o usuário tem permissão para o módulo específico
        if (modulosLiberados.Contains(requirement.ModuleName))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}

public class ModuleRequirement : IAuthorizationRequirement
{
    public string ModuleName { get; }

    public ModuleRequirement(string moduleName)
    {
        ModuleName = moduleName;
    }
}

public class ModuleAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public ModuleAuthorizationPolicyProvider(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("Module_"))
        {
            var moduleName = policyName["Module_".Length..];
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new ModuleRequirement(moduleName))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}