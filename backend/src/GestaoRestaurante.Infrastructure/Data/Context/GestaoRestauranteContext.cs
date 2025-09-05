using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Infrastructure.Data.Indexes;

namespace GestaoRestaurante.Infrastructure.Data.Context;

public class GestaoRestauranteContext : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>
{
    public GestaoRestauranteContext(DbContextOptions<GestaoRestauranteContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Filial> Filiais { get; set; }
    public DbSet<UsuarioFilial> UsuarioFiliais { get; set; }
    public DbSet<Agrupamento> Agrupamentos { get; set; }
    public DbSet<SubAgrupamento> SubAgrupamentos { get; set; }
    public DbSet<CentroCusto> CentrosCusto { get; set; }
    public DbSet<FilialAgrupamento> FilialAgrupamentos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<ProdutoIngrediente> ProdutoIngredientes { get; set; }
    public DbSet<Ingrediente> Ingredientes { get; set; }
    public DbSet<Mesa> Mesas { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ItemPedido> ItensPedido { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<RegistroJornada> RegistrosJornada { get; set; }
    public DbSet<MovimentacaoFinanceira> MovimentacoesFinanceiras { get; set; }
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
    public DbSet<PlanoAssinatura> PlanosAssinatura { get; set; }
    public DbSet<ModuloPlano> ModulosPlano { get; set; }
    public DbSet<AssinaturaEmpresa> AssinaturasEmpresa { get; set; }
    public DbSet<LogOperacao> LogsOperacao { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações de entidades automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestaoRestauranteContext).Assembly);

        // Aplicar índices customizados para otimização (Fase 5)
        DatabaseIndexes.ApplyIndexes(modelBuilder);

        // Configurações globais
        ConfigureGlobalSettings(modelBuilder);
    }

    private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {
        // Configurar precision para propriedades decimais globalmente
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        // Configurar string default length
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(string) && !p.GetMaxLength().HasValue))
        {
            property.SetMaxLength(255);
        }

        // Configurar DateTime para usar datetime2
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
        {
            property.SetColumnType("datetime2");
        }

        // Configurar nomes de tabelas no singular
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && tableName.EndsWith("s"))
            {
                entityType.SetTableName(tableName.TrimEnd('s'));
            }
        }

        // Configurar esquemas para entidades sem configuração específica
        ConfigureEntitySchemas(modelBuilder);
    }

    private static void ConfigureEntitySchemas(ModelBuilder modelBuilder)
    {
        // Esquema Core - Sistema, usuários, logs, assinaturas
        modelBuilder.Entity<PlanoAssinatura>().ToTable("PlanoAssinatura", "Core");
        modelBuilder.Entity<ModuloPlano>().ToTable("ModuloPlano", "Core");
        modelBuilder.Entity<AssinaturaEmpresa>().ToTable("AssinaturaEmpresa", "Core");
        modelBuilder.Entity<LogOperacao>().ToTable("LogOperacao", "Core");

        // Esquema Produtos - Ingredientes (relacionado a produtos)
        modelBuilder.Entity<Ingrediente>().ToTable("Ingrediente", "Produtos");

        // Esquema Cardapio - Mesas (módulo cardápio)
        modelBuilder.Entity<Mesa>().ToTable("Mesa", "Cardapio");

        // Esquema Pedidos - Pedidos e itens
        modelBuilder.Entity<Pedido>().ToTable("Pedido", "Pedidos");
        modelBuilder.Entity<ItemPedido>().ToTable("ItemPedido", "Pedidos");

        // Esquema Funcionarios - Funcionários e jornada
        modelBuilder.Entity<Funcionario>().ToTable("Funcionario", "Funcionarios");
        modelBuilder.Entity<RegistroJornada>().ToTable("RegistroJornada", "Funcionarios");

        // Esquema Financeiro - Movimentações financeiras
        modelBuilder.Entity<MovimentacaoFinanceira>().ToTable("MovimentacaoFinanceira", "Financeiro");

        // Esquema Estoque - Movimentações de estoque
        modelBuilder.Entity<MovimentacaoEstoque>().ToTable("MovimentacaoEstoque", "Estoque");

        // Configurar tabelas do Identity para esquema Core
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>().ToTable("Role", "Core");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>().ToTable("UserRole", "Core");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>().ToTable("UserClaim", "Core");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>().ToTable("UserLogin", "Core");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>().ToTable("UserToken", "Core");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>().ToTable("RoleClaim", "Core");
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity.GetType().GetProperty("DataCriacao") != null ||
                        e.Entity.GetType().GetProperty("DataUltimaAlteracao") != null);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                var dataCriacaoProperty = entry.Entity.GetType().GetProperty("DataCriacao");
                if (dataCriacaoProperty != null && dataCriacaoProperty.PropertyType == typeof(DateTime))
                {
                    dataCriacaoProperty.SetValue(entry.Entity, DateTime.UtcNow);
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                var dataAlteracaoProperty = entry.Entity.GetType().GetProperty("DataUltimaAlteracao");
                if (dataAlteracaoProperty != null && dataAlteracaoProperty.PropertyType == typeof(DateTime?))
                {
                    dataAlteracaoProperty.SetValue(entry.Entity, DateTime.UtcNow);
                }
            }
        }
    }
}