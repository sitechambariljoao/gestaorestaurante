using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class ProdutoIngredienteConfiguration : IEntityTypeConfiguration<ProdutoIngrediente>
{
    public void Configure(EntityTypeBuilder<ProdutoIngrediente> builder)
    {
        builder.ToTable("ProdutoIngrediente", "Produtos");

        builder.HasKey(pi => new { pi.ProdutoId, pi.IngredienteId });

        builder.Property(pi => pi.Quantidade)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(pi => pi.UnidadeMedida)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(pi => pi.DataVinculo)
            .HasColumnType("datetime2")
            .IsRequired();

        // Relacionamentos
        builder.HasOne(pi => pi.Produto)
            .WithMany()
            .HasForeignKey(pi => pi.ProdutoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.Ingrediente)
            .WithMany()
            .HasForeignKey(pi => pi.IngredienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}