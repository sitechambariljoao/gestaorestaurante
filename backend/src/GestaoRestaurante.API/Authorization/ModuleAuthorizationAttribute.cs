using Microsoft.AspNetCore.Authorization;

namespace GestaoRestaurante.API.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ModuleAuthorizationAttribute : AuthorizeAttribute
{
    public ModuleAuthorizationAttribute(string moduleName)
    {
        Policy = $"Module_{moduleName}";
    }
}

public static class ModuleNames
{
    public const string EMPRESAS = "EMPRESAS";
    public const string FILIAIS = "FILIAIS";
    public const string CENTRO_CUSTO = "CENTRO_CUSTO";
    public const string CATEGORIAS = "CATEGORIAS";
    public const string PRODUTOS = "PRODUTOS";
    public const string CARDAPIO = "CARDAPIO";
    public const string ESTOQUE = "ESTOQUE";
    public const string PEDIDOS = "PEDIDOS";
    public const string FINANCEIRO = "FINANCEIRO";
    public const string FUNCIONARIOS = "FUNCIONARIOS";
    public const string RELATORIOS = "RELATORIOS";
    public const string USUARIOS = "USUARIOS";
}