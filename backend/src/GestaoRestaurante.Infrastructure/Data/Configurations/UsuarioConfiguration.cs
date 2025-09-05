using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario", "Core");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(u => u.EmpresaId)
            .IsRequired();

        builder.Property(u => u.FilialId)
            .IsRequired();

        builder.Property(u => u.Nome)
            .HasMaxLength(255)
            .IsRequired();

        // Email e senha são gerenciados pelo Identity
        // Remover configurações de propriedades do IdentityUser

        builder.Property(u => u.Ativo)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(u => u.DataCriacao)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(u => u.UltimoLogin)
            .HasColumnType("datetime2");

        // Índices
        builder.HasIndex(u => u.EmpresaId)
            .HasDatabaseName("IX_Usuario_EmpresaId");

        builder.HasIndex(u => u.FilialId)
            .HasDatabaseName("IX_Usuario_FilialId");

        // Relacionamentos
        builder.HasOne(u => u.Empresa)
            .WithMany() // Removido relacionamento bidirecional
            .HasForeignKey(u => u.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Filial)
            .WithMany(f => f.Usuarios)
            .HasForeignKey(u => u.FilialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}