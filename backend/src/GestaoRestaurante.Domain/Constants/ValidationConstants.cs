namespace GestaoRestaurante.Domain.Constants;

/// <summary>
/// Constantes específicas para validações do FluentValidation
/// </summary>
public static class ValidationConstants
{
    public static class ErrorMessages
    {
        // Campos obrigatórios
        public const string EmpresaIdRequired = "ID da empresa é obrigatório";
        public const string FilialIdRequired = "ID da filial é obrigatório";
        public const string AgrupamentoIdRequired = "ID do agrupamento é obrigatório";
        public const string SubAgrupamentoIdRequired = "ID do sub-agrupamento é obrigatório";
        public const string CentroCustoIdRequired = "ID do centro de custo é obrigatório";
        public const string CategoriaIdRequired = "ID da categoria é obrigatório";
        
        public const string CodigoRequired = "Código é obrigatório";
        public const string NomeRequired = "Nome é obrigatório";
        public const string EmailRequired = "Email é obrigatório";
        public const string CnpjRequired = "CNPJ é obrigatório";
        public const string CpfRequired = "CPF é obrigatório";
        public const string PrecoRequired = "Preço é obrigatório";
        public const string UnidadeMedidaRequired = "Unidade de medida é obrigatória";
        
        // Validações de tamanho
        public const string CodigoMaxLength = "Código deve ter no máximo {0} caracteres";
        public const string NomeMaxLength = "Nome deve ter no máximo {0} caracteres";
        public const string DescricaoMaxLength = "Descrição deve ter no máximo {0} caracteres";
        public const string EmailMaxLength = "Email deve ter no máximo {0} caracteres";
        public const string TelefoneMaxLength = "Telefone deve ter no máximo {0} caracteres";
        public const string UnidadeMedidaMaxLength = "Unidade de medida deve ter no máximo {0} caracteres";
        
        // Validações de formato
        public const string EmailInvalid = "Email deve ter um formato válido";
        public const string CnpjInvalid = "CNPJ deve ter um formato válido";
        public const string CpfInvalid = "CPF deve ter um formato válido";
        public const string TelefoneInvalid = "Telefone deve ter um formato válido";
        public const string CepInvalid = "CEP deve ter um formato válido";
        
        // Validações de valor
        public const string PrecoPositivo = "Preço deve ser maior que zero";
        public const string QuantidadePositiva = "Quantidade deve ser maior que zero";
        
        // Validações de unicidade
        public const string CodigoJaExiste = "Já existe um registro com este código";
        public const string NomeJaExiste = "Já existe um registro com este nome";
        public const string EmailJaExiste = "Já existe um usuário com este email";
        public const string CnpjJaExiste = "Já existe uma empresa com este CNPJ";
        
        // Validações contextuais
        public const string CodigoUnicoNoContexto = "Já existe um registro com este código neste contexto";
        public const string NomeUnicoNoContexto = "Já existe um registro com este nome neste contexto";
    }

    public static class Rules
    {
        // Tamanhos mínimos
        public const int MinCodigoLength = 2;
        public const int MinNomeLength = 2;
        public const int MinDescricaoLength = 3;
        
        // Valores mínimos
        public const double MinPreco = 0.01;
        public const double MinQuantidade = 0.01;
        
        // Expressões regulares para validação
        public static class Patterns
        {
            public const string CodigoAlfanumerico = @"^[a-zA-Z0-9]+$";
            public const string NomeComEspacos = @"^[a-zA-ZÀ-ÿ0-9\s.,'-]+$";
            public const string ApenasLetras = @"^[a-zA-ZÀ-ÿ\s]+$";
            public const string ApenasNumeros = @"^\d+$";
        }
    }
}