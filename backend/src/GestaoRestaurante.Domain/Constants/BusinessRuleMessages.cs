namespace GestaoRestaurante.Domain.Constants;

/// <summary>
/// Mensagens padronizadas para regras de negócio e validações
/// </summary>
public static class BusinessRuleMessages
{
    public static class Validation
    {
        public const string Required = "O campo {0} é obrigatório";
        public const string MaxLength = "O campo {0} deve ter no máximo {1} caracteres";
        public const string MinLength = "O campo {0} deve ter no mínimo {1} caracteres";
        public const string InvalidFormat = "O campo {0} possui formato inválido";
        public const string InvalidRange = "O campo {0} deve estar entre {1} e {2}";
        public const string MustBePositive = "O campo {0} deve ser um valor positivo";
        public const string MustBeUnique = "Já existe um registro com este {0}";
        public const string InvalidEmail = "O email informado possui formato inválido";
        public const string InvalidCnpj = "O CNPJ informado é inválido";
        public const string InvalidCpf = "O CPF informado é inválido";
        public const string InvalidCep = "O CEP informado é inválido";
        public const string InvalidTelefone = "O telefone informado é inválido";
    }
    
    public static class BusinessRules
    {
        public const string EntityNotFound = "{0} não encontrada";
        public const string EntityInactive = "{0} está inativa";
        public const string CannotDeleteWithDependents = "Não é possível excluir {0} que possui {1} vinculados";
        public const string CannotInactivateWithDependents = "Não é possível inativar {0} que possui {1} ativos";
        public const string OnlyOneActiveAllowed = "Só é permitida uma {0} ativa por {1}";
        public const string MustHaveParent = "{0} de nível {1} deve ter um {2}";
        public const string CannotHaveParent = "{0} de nível {1} não pode ter um {2}";
        public const string InvalidHierarchy = "A hierarquia entre {0} e {1} é inválida";
        public const string DuplicateInContext = "Já existe uma {0} com {1} '{2}' neste contexto";
    }
    
    public static class Authentication
    {
        public const string InvalidCredentials = "Credenciais inválidas";
        public const string UserNotFound = "Usuário não encontrado";
        public const string UserInactive = "Usuário inativo";
        public const string InvalidToken = "Token inválido ou expirado";
        public const string InsufficientPermissions = "Permissões insuficientes para esta operação";
        public const string ModuleAccessDenied = "Acesso negado ao módulo {0}";
    }
    
    public static class System
    {
        public const string UnexpectedError = "Ocorreu um erro inesperado. Tente novamente.";
        public const string DatabaseError = "Erro ao acessar o banco de dados";
        public const string ExternalServiceError = "Erro ao comunicar com serviço externo";
        public const string ConcurrencyError = "Os dados foram modificados por outro usuário. Recarregue e tente novamente.";
        public const string OperationNotAllowed = "Operação não permitida no estado atual";
    }
    
    // Constantes específicas para entidades
    public const string EMPRESA_NAO_ENCONTRADA = "Empresa não encontrada";
    public const string FILIAL_NAO_ENCONTRADA = "Filial não encontrada";
    public const string AGRUPAMENTO_NAO_ENCONTRADO = "Agrupamento não encontrado";
    public const string SUBAGRUPAMENTO_NAO_ENCONTRADO = "SubAgrupamento não encontrado";
    public const string CENTRO_CUSTO_NAO_ENCONTRADO = "Centro de Custo não encontrado";
    public const string CATEGORIA_NAO_ENCONTRADA = "Categoria não encontrada";
    public const string PRODUTO_NAO_ENCONTRADO = "Produto não encontrado";
    public const string USUARIO_NAO_ENCONTRADO = "Usuário não encontrado";
}