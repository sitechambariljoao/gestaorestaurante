using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produto", "Produtos", t => {
            t.HasCheckConstraint("CK_Produto_Preco", "Preco > 0");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(p => p.CategoriaId)
            .IsRequired();

        builder.Property(p => p.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasMaxLength(500);

        builder.Property(p => p.Preco)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.UnidadeMedida)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Ativa)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(p => p.ProdutoVenda)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(p => p.ProdutoEstoque)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(p => p.DataCriacao)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(p => p.DataUltimaAlteracao)
            .HasColumnType("datetime2");

        // Índices
        builder.HasIndex(p => new { p.CategoriaId, p.Codigo })
            .IsUnique()
            .HasDatabaseName("IX_Produto_CategoriaId_Codigo");

        builder.HasIndex(p => p.Nome)
            .HasDatabaseName("IX_Produto_Nome");

        // Relacionamentos
        builder.HasOne(p => p.Categoria)
            .WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // O relacionamento many-to-many será configurado na configuração da entidade de ligação

        builder.HasMany(p => p.ItensPedido)
            .WithOne(ip => ip.Produto)
            .HasForeignKey(ip => ip.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.MovimentacoesEstoque)
            .WithOne(me => me.Produto)
            .HasForeignKey(me => me.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}