using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class SubAgrupamentoConfiguration : IEntityTypeConfiguration<SubAgrupamento>
{
    public void Configure(EntityTypeBuilder<SubAgrupamento> builder)
    {
        builder.ToTable("SubAgrupamento", "CentroCusto");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.Id)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(sa => sa.AgrupamentoId)
            .IsRequired();

        builder.Property(sa => sa.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(sa => sa.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(sa => sa.Descricao)
            .HasMaxLength(500);

        builder.Property(sa => sa.Ativa)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(sa => sa.DataCriacao)
            .HasColumnType("datetime2")
            .IsRequired();

        // Índices
        builder.HasIndex(sa => new { sa.AgrupamentoId, sa.Codigo })
            .IsUnique()
            .HasDatabaseName("IX_SubAgrupamento_AgrupamentoId_Codigo");

        // Relacionamentos
        builder.HasOne(sa => sa.Agrupamento)
            .WithMany(a => a.SubAgrupamentos)
            .HasForeignKey(sa => sa.AgrupamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(sa => sa.CentrosCusto)
            .WithOne(cc => cc.SubAgrupamento)
            .HasForeignKey(cc => cc.SubAgrupamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}