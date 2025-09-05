using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresa", "Empresas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(e => e.RazaoSocial)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.NomeFantasia)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Cnpj)
            .HasMaxLength(18)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Telefone)
            .HasMaxLength(20);

        // Configuração do Value Object Endereco (obrigatório)
        builder.OwnsOne(e => e.Endereco, endereco =>
        {
            endereco.Property(end => end.Logradouro)
                .HasColumnName("Endereco_Logradouro")
                .HasMaxLength(200)
                .IsRequired();

            endereco.Property(end => end.Numero)
                .HasColumnName("Endereco_Numero")
                .HasMaxLength(20);

            endereco.Property(end => end.Complemento)
                .HasColumnName("Endereco_Complemento")
                .HasMaxLength(100);

            endereco.Property(end => end.Cep)
                .HasColumnName("Endereco_Cep")
                .HasMaxLength(10)
                .IsRequired();

            endereco.Property(end => end.Bairro)
                .HasColumnName("Endereco_Bairro")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(end => end.Cidade)
                .HasColumnName("Endereco_Cidade")
                .HasMaxLength(100)
                .IsRequired();

            endereco.Property(end => end.Estado)
                .HasColumnName("Endereco_Estado")
                .HasMaxLength(2)
                .IsRequired();
        })
        .Navigation(e => e.Endereco)
        .IsRequired();

        builder.Property(e => e.Ativa)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(e => e.DataCriacao)
            .HasColumnType("datetime2")
            .IsRequired();

        // Índices
        builder.HasIndex(e => e.Cnpj)
            .IsUnique()
            .HasDatabaseName("IX_Empresa_Cnpj");

        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Empresa_Email");

        // Relacionamentos
        builder.HasMany(e => e.Filiais)
            .WithOne(f => f.Empresa)
            .HasForeignKey(f => f.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AssinaturaAtiva)
            .WithOne(a => a.Empresa)
            .HasForeignKey<AssinaturaEmpresa>(a => a.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}