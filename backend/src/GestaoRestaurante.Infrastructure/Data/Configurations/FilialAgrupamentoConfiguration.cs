using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class FilialAgrupamentoConfiguration : IEntityTypeConfiguration<FilialAgrupamento>
{
    public void Configure(EntityTypeBuilder<FilialAgrupamento> builder)
    {
        builder.ToTable("FilialAgrupamento", "CentroCusto");

        // Configurar chave composta
        builder.HasKey(fa => new { fa.FilialId, fa.AgrupamentoId });

        // Configurar propriedades
        builder.Property(fa => fa.FilialId)
            .IsRequired();

        builder.Property(fa => fa.AgrupamentoId)
            .IsRequired();

        builder.Property(fa => fa.DataVinculo)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(fa => fa.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        // Configurar relacionamentos
        builder.HasOne(fa => fa.Filial)
            .WithMany()
            .HasForeignKey(fa => fa.FilialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fa => fa.Agrupamento)
            .WithMany()
            .HasForeignKey(fa => fa.AgrupamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar índices
        builder.HasIndex(fa => fa.DataVinculo)
            .HasDatabaseName("IX_FilialAgrupamentos_DataVinculo");

        builder.HasIndex(fa => fa.Ativo)
            .HasDatabaseName("IX_FilialAgrupamentos_Ativo");
    }
}