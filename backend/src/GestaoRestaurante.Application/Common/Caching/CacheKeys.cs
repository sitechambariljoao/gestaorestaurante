namespace GestaoRestaurante.Application.Common.Caching;

/// <summary>
/// Constantes para chaves de cache padronizadas
/// </summary>
public static class CacheKeys
{
    // Prefixos por módulo
    public const string EMPRESA_PREFIX = "empresa:";
    public const string FILIAL_PREFIX = "filial:";
    public const string PRODUTO_PREFIX = "produto:";
    public const string CATEGORIA_PREFIX = "categoria:";
    public const string USER_PREFIX = "user:";

    // Empresas
    public static string EmpresaById(Guid id) => $"{EMPRESA_PREFIX}id:{id}";
    public static string EmpresaAllActive() => $"{EMPRESA_PREFIX}all:active";
    public static string EmpresaByEstado(string estado) => $"{EMPRESA_PREFIX}estado:{estado}";
    public static string EmpresaByPlano(string plano) => $"{EMPRESA_PREFIX}plano:{plano}";
    public static string EmpresaSearch(string query) => $"{EMPRESA_PREFIX}search:{query.GetHashCode()}";

    // Produtos  
    public static string ProdutoById(Guid id) => $"{PRODUTO_PREFIX}id:{id}";
    public static string ProdutoByCategoria(Guid categoriaId) => $"{PRODUTO_PREFIX}categoria:{categoriaId}";
    public static string ProdutoSearch(string searchHash) => $"{PRODUTO_PREFIX}search:{searchHash}";
    public static string ProdutoAllActive() => $"{PRODUTO_PREFIX}all:active";

    // Categorias
    public static string CategoriaById(Guid id) => $"{CATEGORIA_PREFIX}id:{id}";
    public static string CategoriaByCentroCusto(Guid centroCustoId) => $"{CATEGORIA_PREFIX}cc:{centroCustoId}";
    public static string CategoriaHierarquia(Guid centroCustoId) => $"{CATEGORIA_PREFIX}hierarquia:{centroCustoId}";

    // Filiais
    public static string FilialById(Guid id) => $"{FILIAL_PREFIX}id:{id}";
    public static string FilialByEmpresa(Guid empresaId) => $"{FILIAL_PREFIX}empresa:{empresaId}";

    // Usuários
    public static string UserById(Guid id) => $"{USER_PREFIX}id:{id}";
    public static string UserByEmail(string email) => $"{USER_PREFIX}email:{email.GetHashCode()}";
    public static string UserModules(Guid userId) => $"{USER_PREFIX}modules:{userId}";

    // Configurações e metadados
    public const string PLANOS_ASSINATURA = "system:planos";
    public const string MODULOS_SISTEMA = "system:modulos";

    // Padrões para limpeza
    public static string EmpresaPattern() => $"{EMPRESA_PREFIX}*";
    public static string ProdutoPattern() => $"{PRODUTO_PREFIX}*";
    public static string CategoriaPattern() => $"{CATEGORIA_PREFIX}*";
    public static string UserPattern(Guid userId) => $"{USER_PREFIX}*{userId}*";

    /// <summary>
    /// Gera hash para parâmetros complexos de busca
    /// </summary>
    public static string GenerateSearchHash(params object[] parameters)
    {
        var combined = string.Join("|", parameters.Where(p => p != null).Select(p => p.ToString()));
        return combined.GetHashCode().ToString();
    }

    /// <summary>
    /// Tempos de expiração padrão
    /// </summary>
    public static class Expiration
    {
        public static readonly TimeSpan Short = TimeSpan.FromMinutes(5);      // Dados que mudam frequentemente
        public static readonly TimeSpan Medium = TimeSpan.FromMinutes(30);    // Dados relativamente estáveis
        public static readonly TimeSpan Long = TimeSpan.FromHours(2);         // Dados raras vezes alterados
        public static readonly TimeSpan VeryLong = TimeSpan.FromHours(12);    // Configurações do sistema
    }
}