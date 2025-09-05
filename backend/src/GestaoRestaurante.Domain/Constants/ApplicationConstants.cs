namespace GestaoRestaurante.Domain.Constants;

/// <summary>
/// Constantes da aplicação para evitar magic numbers/strings
/// </summary>
public static class ApplicationConstants
{
    public static class FieldLengths
    {
        // Identificadores
        public const int CodigoMaxLength = 20;
        public const int NomeMaxLength = 200;
        public const int DescricaoMaxLength = 500;
        
        // Documentos
        public const int CnpjMaxLength = 18;
        public const int CpfMaxLength = 14;
        public const int InscricaoEstadualMaxLength = 15;
        
        // Contato
        public const int EmailMaxLength = 100;
        public const int TelefoneMaxLength = 20;
        
        // Endereço
        public const int LogradouroMaxLength = 200;
        public const int NumeroMaxLength = 20;
        public const int ComplementoMaxLength = 100;
        public const int CepMaxLength = 10;
        public const int BairroMaxLength = 100;
        public const int CidadeMaxLength = 100;
        public const int EstadoMaxLength = 2;
        
        // Usuário
        public const int CargoMaxLength = 50;
        public const int ObservacoesMaxLength = 500;
        
        // Sistema
        public const int TokenMaxLength = 1000;
        public const int HashMaxLength = 255;
    }
    
    public static class BusinessRules
    {
        // Categorias
        public const int CategoriaMinLevel = 1;
        public const int CategoriaMaxLevel = 3;
        
        // Produtos
        public const decimal ProdutoMinPrice = 0.01m;
        public const decimal ProdutoMaxPrice = 999999.99m;
        
        // Estoque
        public const decimal EstoqueMinQuantity = 0m;
        public const decimal EstoqueMaxQuantity = 999999.99m;
        
        // Paginação
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
        
        // Cache
        public const int DefaultCacheExpirationMinutes = 30;
        public const int LongCacheExpirationHours = 24;
    }
    
    public static class ValidationPatterns
    {
        public const string CnpjPattern = @"^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$";
        public const string CpfPattern = @"^\d{3}\.\d{3}\.\d{3}-\d{2}$";
        public const string CepPattern = @"^\d{5}-?\d{3}$";
        public const string TelefonePattern = @"^(\(?\d{2}\)?\s?)?(\d{4,5}-?\d{4})$";
        public const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    }
}