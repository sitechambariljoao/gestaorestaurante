using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class CentroCustoConfiguration : IEntityTypeConfiguration<CentroCusto>
{
    public void Configure(EntityTypeBuilder<CentroCusto> builder)
    {
        builder.ToTable("CentroCusto", "CentroCusto");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Id)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(cc => cc.SubAgrupamentoId)
            .IsRequired();

        builder.Property(cc => cc.Codigo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(cc => cc.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(cc => cc.Descricao)
            .HasMaxLength(500);

        builder.Property(cc => cc.Ativa)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(cc => cc.DataCriacao)
            .HasColumnType("datetime2")
            .IsRequired();

        // Índices
        builder.HasIndex(cc => new { cc.SubAgrupamentoId, cc.Codigo })
            .IsUnique()
            .HasDatabaseName("IX_CentroCusto_SubAgrupamentoId_Codigo");

        // Relacionamentos
        builder.HasOne(cc => cc.SubAgrupamento)
            .WithMany(sa => sa.CentrosCusto)
            .HasForeignKey(cc => cc.SubAgrupamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(cc => cc.Categorias)
            .WithOne(c => c.CentroCusto)
            .HasForeignKey(c => c.CentroCustoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}