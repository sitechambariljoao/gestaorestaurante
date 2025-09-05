using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class UsuarioFilialConfiguration : IEntityTypeConfiguration<UsuarioFilial>
{
    public void Configure(EntityTypeBuilder<UsuarioFilial> builder)
    {
        builder.ToTable("UsuarioFilial", "Filiais");

        builder.HasKey(uf => new { uf.UsuarioId, uf.FilialId });

        builder.Property(uf => uf.DataVinculo)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(uf => uf.Ativo)
            .HasDefaultValue(true)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(uf => uf.Usuario)
            .WithMany()
            .HasForeignKey(uf => uf.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uf => uf.Filial)
            .WithMany()
            .HasForeignKey(uf => uf.FilialId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}