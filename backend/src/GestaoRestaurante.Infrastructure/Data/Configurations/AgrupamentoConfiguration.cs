using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class AgrupamentoConfiguration : IEntityTypeConfiguration<Agrupamento>
{
    public void Configure(EntityTypeBuilder<Agrupamento> builder)
    {
        builder.ToTable("Agrupamento", "CentroCusto");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(a => a.FilialId)
            .IsRequired();

        builder.Property(a => a.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Descricao)
            .HasMaxLength(500);

        builder.Property(a => a.Ativa)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(a => a.DataCriacao)
            .HasColumnType("datetime2")
            .IsRequired();

        // Índices
        builder.HasIndex(a => new { a.FilialId, a.Codigo })
            .IsUnique()
            .HasDatabaseName("IX_Agrupamento_FilialId_Codigo");

        // Relacionamentos
        builder.HasOne(a => a.Filial)
            .WithMany(f => f.Agrupamentos)
            .HasForeignKey(a => a.FilialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.SubAgrupamentos)
            .WithOne(sa => sa.Agrupamento)
            .HasForeignKey(sa => sa.AgrupamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}