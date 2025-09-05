using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categoria", "Categorias", t => {
            t.HasCheckConstraint("CK_Categoria_Nivel", "Nivel BETWEEN 1 AND 3");
            t.HasCheckConstraint("CK_Categoria_Hierarquia", 
                "(Nivel = 1 AND CategoriaPaiId IS NULL) OR " +
                "(Nivel > 1 AND CategoriaPaiId IS NOT NULL)");
        });

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(c => c.CentroCustoId)
            .IsRequired();

        builder.Property(c => c.CategoriaPaiId);

        builder.Property(c => c.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasMaxLength(500);

        builder.Property(c => c.Nivel)
            .IsRequired();

        builder.Property(c => c.Ativa)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(c => c.DataCriacao)
            .HasColumnType("datetime2")
            .IsRequired();

        // Índices
        builder.HasIndex(c => new { c.CentroCustoId, c.Codigo })
            .IsUnique()
            .HasDatabaseName("IX_Categoria_CentroCustoId_Codigo");

        builder.HasIndex(c => c.CategoriaPaiId)
            .HasDatabaseName("IX_Categoria_CategoriaPaiId");

        // Relacionamentos
        builder.HasOne(c => c.CentroCusto)
            .WithMany(cc => cc.Categorias)
            .HasForeignKey(c => c.CentroCustoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CategoriaPai)
            .WithMany(c => c.CategoriasFilhas)
            .HasForeignKey(c => c.CategoriaPaiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.CategoriasFilhas)
            .WithOne(c => c.CategoriaPai)
            .HasForeignKey(c => c.CategoriaPaiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Produtos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}