using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Indexes;

/// <summary>
/// Configuração centralizada de índices para otimização de consultas
/// </summary>
public static class DatabaseIndexes
{
    /// <summary>
    /// Aplica todos os índices nas entidades
    /// </summary>
    public static void ApplyIndexes(ModelBuilder modelBuilder)
    {
        ApplyEmpresaIndexes(modelBuilder);
        ApplyFilialIndexes(modelBuilder);
        ApplyProdutoIndexes(modelBuilder);
        ApplyUsuarioIndexes(modelBuilder);
        ApplyCategoriaIndexes(modelBuilder);
        ApplyAuditIndexes(modelBuilder);
    }

    private static void ApplyEmpresaIndexes(ModelBuilder modelBuilder)
    {
        var empresa = modelBuilder.Entity<Empresa>();

        // Índice único para CNPJ (já existe via configuração)
        empresa.HasIndex(e => e.Cnpj)
            .IsUnique()
            .HasDatabaseName("IX_Empresa_Cnpj");

        // Índice único para Email (já existe via configuração)
        empresa.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Empresa_Email");

        // Índice para busca por status ativo
        empresa.HasIndex(e => e.Ativa)
            .HasDatabaseName("IX_Empresa_Ativa");

        // Índice para busca por data de criação
        empresa.HasIndex(e => e.DataCriacao)
            .HasDatabaseName("IX_Empresa_DataCriacao");

        // Índice para busca por nome fantasia (busca textual)
        empresa.HasIndex(e => e.NomeFantasia)
            .HasDatabaseName("IX_Empresa_NomeFantasia");
    }

    private static void ApplyFilialIndexes(ModelBuilder modelBuilder)
    {
        var filial = modelBuilder.Entity<Filial>();

        // Índice único para CNPJ
        filial.HasIndex(f => f.Cnpj)
            .IsUnique()
            .HasDatabaseName("IX_Filial_Cnpj");

        // Índice único para Email
        filial.HasIndex(f => f.Email)
            .IsUnique()
            .HasDatabaseName("IX_Filial_Email");

        // Índice para empresa + ativa (consulta comum)
        filial.HasIndex(f => new { f.EmpresaId, f.Ativa })
            .HasDatabaseName("IX_Filial_Empresa_Ativa");

        // Índice para busca por nome
        filial.HasIndex(f => f.Nome)
            .HasDatabaseName("IX_Filial_Nome");
    }

    private static void ApplyProdutoIndexes(ModelBuilder modelBuilder)
    {
        var produto = modelBuilder.Entity<Produto>();

        // Índice único para código (já existe via configuração)
        produto.HasIndex(p => p.Codigo)
            .IsUnique()
            .HasDatabaseName("IX_Produto_Codigo");

        // Índice para categoria + ativo (consulta muito comum)
        produto.HasIndex(p => new { p.CategoriaId, p.Ativa })
            .HasDatabaseName("IX_Produto_Categoria_Ativa");

        // Índice para busca por nome
        produto.HasIndex(p => p.Nome)
            .HasDatabaseName("IX_Produto_Nome");

        // Índice para busca por preço (relatórios)
        produto.HasIndex(p => p.Preco)
            .HasDatabaseName("IX_Produto_Preco");

        // Índice composto para busca avançada
        produto.HasIndex(p => new { p.Ativa, p.Preco, p.CategoriaId })
            .HasDatabaseName("IX_Produto_Ativa_Preco_Categoria");

        // Índice para data de criação
        produto.HasIndex(p => p.DataCriacao)
            .HasDatabaseName("IX_Produto_DataCriacao");
    }

    private static void ApplyUsuarioIndexes(ModelBuilder modelBuilder)
    {
        var usuario = modelBuilder.Entity<Usuario>();

        // Email já tem índice único do Identity

        // Índice para empresa (consulta comum)
        usuario.HasIndex(u => u.EmpresaId)
            .HasDatabaseName("IX_Usuario_EmpresaId");

        // Índice composto para empresa + ativo
        usuario.HasIndex(u => new { u.EmpresaId, u.Ativo })
            .HasDatabaseName("IX_Usuario_Empresa_Ativo");

        // Índice para último login
        usuario.HasIndex(u => u.UltimoLogin)
            .HasDatabaseName("IX_Usuario_UltimoLogin");
    }

    private static void ApplyCategoriaIndexes(ModelBuilder modelBuilder)
    {
        var categoria = modelBuilder.Entity<Categoria>();

        // Índice para centro de custo + ativa
        categoria.HasIndex(c => new { c.CentroCustoId, c.Ativa })
            .HasDatabaseName("IX_Categoria_CentroCusto_Ativa");

        // Índice para busca por código
        categoria.HasIndex(c => c.Codigo)
            .HasDatabaseName("IX_Categoria_Codigo");

        // Índice para busca por nome
        categoria.HasIndex(c => c.Nome)
            .HasDatabaseName("IX_Categoria_Nome");

        // Índice para hierarquia (categoria pai)
        categoria.HasIndex(c => c.CategoriaPaiId)
            .HasDatabaseName("IX_Categoria_CategoriaPai");

        // Índice para nível hierárquico
        categoria.HasIndex(c => c.Nivel)
            .HasDatabaseName("IX_Categoria_Nivel");
    }

    private static void ApplyAuditIndexes(ModelBuilder modelBuilder)
    {
        var logOperacao = modelBuilder.Entity<LogOperacao>();

        // Índice para busca por entidade
        logOperacao.HasIndex(l => new { l.Entidade, l.EntidadeId })
            .HasDatabaseName("IX_LogOperacao_Entidade_Id");

        // Índice para busca por usuário
        logOperacao.HasIndex(l => l.UsuarioId)
            .HasDatabaseName("IX_LogOperacao_Usuario");

        // Índice para busca por data
        logOperacao.HasIndex(l => l.DataOperacao)
            .HasDatabaseName("IX_LogOperacao_Data");

        // Índice composto para relatórios de auditoria
        logOperacao.HasIndex(l => new { l.Entidade, l.DataOperacao, l.UsuarioId })
            .HasDatabaseName("IX_LogOperacao_Entidade_Data_Usuario");
    }

    /// <summary>
    /// Scripts para criação de índices adicionais via SQL (para casos especiais)
    /// </summary>
    public static class CustomIndexes
    {
        /// <summary>
        /// Índice de texto completo para busca por nome de empresa
        /// </summary>
        public const string EmpresaFullTextIndex = @"
            CREATE FULLTEXT CATALOG GestaoRestaurante_Catalog;
            
            CREATE FULLTEXT INDEX ON Empresas(Nome)
            KEY INDEX PK_Empresas
            ON GestaoRestaurante_Catalog;";

        /// <summary>
        /// Índice de texto completo para busca por nome de produto
        /// </summary>
        public const string ProdutoFullTextIndex = @"
            CREATE FULLTEXT INDEX ON Produtos(Nome, Descricao)
            KEY INDEX PK_Produtos
            ON GestaoRestaurante_Catalog;";

        /// <summary>
        /// Índice funcional para busca case-insensitive em CNPJ
        /// </summary>
        public const string CnpjCaseInsensitiveIndex = @"
            CREATE NONCLUSTERED INDEX IX_Empresa_Cnpj_Upper
            ON Empresas (UPPER(Cnpj))
            INCLUDE (Id, Nome);";

        /// <summary>
        /// Índices para otimização de relatórios (columnstore)
        /// </summary>
        public const string RelatoriosColumnstoreIndex = @"
            CREATE NONCLUSTERED COLUMNSTORE INDEX IX_Produtos_Columnstore
            ON Produtos (Id, Nome, Codigo, Preco, CategoriaId, DataCriacao, Ativa);
            
            CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LogOperacao_Columnstore
            ON LogOperacao (Id, Entidade, Operacao, DataOperacao, UsuarioId);";

        /// <summary>
        /// Índices filtrados para melhor performance em consultas específicas
        /// </summary>
        public static readonly string[] FilteredIndexes = new[]
        {
            // Apenas empresas ativas
            "CREATE NONCLUSTERED INDEX IX_Empresa_Ativa_Filtered ON Empresas (PlanoAssinatura, DataCriacao) WHERE Ativa = 1;",
            
            // Apenas produtos ativos
            "CREATE NONCLUSTERED INDEX IX_Produto_Ativo_Filtered ON Produtos (CategoriaId, Preco) WHERE Ativo = 1;",
            
            // Apenas usuários ativos
            "CREATE NONCLUSTERED INDEX IX_Usuario_Ativo_Filtered ON AspNetUsers (EmpresaId, UltimoLogin) WHERE Ativo = 1;",
            
            // Log de operações recentes (últimos 30 dias)
            "CREATE NONCLUSTERED INDEX IX_LogOperacao_Recent ON LogOperacao (Entidade, UsuarioId) WHERE DataOperacao >= DATEADD(day, -30, GETDATE());"
        };
    }

    /// <summary>
    /// Estatísticas e análise de uso de índices
    /// </summary>
    public static class IndexAnalysis
    {
        /// <summary>
        /// Query para verificar uso dos índices
        /// </summary>
        public const string IndexUsageQuery = @"
            SELECT 
                i.name AS IndexName,
                s.user_seeks,
                s.user_scans,
                s.user_lookups,
                s.user_updates,
                s.last_user_seek,
                s.last_user_scan,
                s.last_user_lookup,
                s.last_user_update,
                CASE 
                    WHEN s.user_seeks + s.user_scans + s.user_lookups = 0 THEN 'UNUSED'
                    WHEN s.user_seeks + s.user_scans + s.user_lookups < s.user_updates THEN 'EXPENSIVE'
                    ELSE 'GOOD'
                END AS IndexStatus
            FROM sys.indexes i
            LEFT JOIN sys.dm_db_index_usage_stats s ON i.object_id = s.object_id AND i.index_id = s.index_id
            WHERE i.object_id = OBJECT_ID('Empresas')
               OR i.object_id = OBJECT_ID('Produtos')
               OR i.object_id = OBJECT_ID('Filiais')
            ORDER BY s.user_seeks + s.user_scans + s.user_lookups DESC;";

        /// <summary>
        /// Query para identificar índices faltantes
        /// </summary>
        public const string MissingIndexesQuery = @"
            SELECT 
                migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) AS improvement_measure,
                'CREATE INDEX [IX_' + OBJECT_NAME(mid.object_id) + '_' + 
                REPLACE(REPLACE(REPLACE(ISNULL(mid.equality_columns,''), ', ', '_'), '[', ''), ']', '') + 
                CASE WHEN mid.inequality_columns IS NOT NULL THEN '_' + 
                REPLACE(REPLACE(REPLACE(mid.inequality_columns, ', ', '_'), '[', ''), ']', '') ELSE '' END + ']'
                + ' ON ' + mid.statement + ' (' + ISNULL (mid.equality_columns,'')
                + CASE WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN ',' ELSE '' END
                + ISNULL (mid.inequality_columns, '') + ')'
                + ISNULL (' INCLUDE (' + mid.included_columns + ')', '') AS CreateIndexStatement,
                migs.user_seeks,
                migs.user_scans,
                migs.avg_total_user_cost,
                migs.avg_user_impact
            FROM sys.dm_db_missing_index_groups mig
            INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
            INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
            WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
            ORDER BY improvement_measure DESC;";
    }
}