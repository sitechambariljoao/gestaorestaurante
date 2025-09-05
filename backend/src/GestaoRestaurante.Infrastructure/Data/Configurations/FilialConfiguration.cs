using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Infrastructure.Data.Configurations;

public class FilialConfiguration : IEntityTypeConfiguration<Filial>
{
    public void Configure(EntityTypeBuilder<Filial> builder)
    {
        builder.ToTable("Filial", "Filiais");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(f => f.EmpresaId)
            .IsRequired();

        builder.Property(f => f.Nome)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(f => f.Matriz)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(f => f.CnpjFilial)
            .HasMaxLength(18);

        builder.Property(f => f.Email)
            .HasMaxLength(255);

        builder.Property(f => f.Telefone)
            .HasMaxLength(20);

        // Configuração do Value Object Endereco da Filial
        builder.OwnsOne(f => f.EnderecoFilial, endereco =>
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
        });

        builder.Property(f => f.Ativa)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(f => f.DataCriacao)
            .HasColumnType("datetime2")
            .IsRequired();

        // Ignora as propriedades calculadas para o mapeamento EF
        builder.Ignore(f => f.Cnpj);
        builder.Ignore(f => f.Endereco);

        // Índices
        builder.HasIndex(f => f.CnpjFilial)
            .IsUnique()
            .HasFilter("CnpjFilial IS NOT NULL")
            .HasDatabaseName("IX_Filial_CnpjFilial");

        builder.HasIndex(f => new { f.EmpresaId, f.Nome })
            .IsUnique()
            .HasDatabaseName("IX_Filial_EmpresaId_Nome");

        // Índice para garantir apenas uma matriz por empresa
        builder.HasIndex(f => new { f.EmpresaId, f.Matriz })
            .IsUnique()
            .HasFilter("Matriz = 1")
            .HasDatabaseName("IX_Filial_EmpresaId_Matriz_Unica");

        // Relacionamentos
        builder.HasOne(f => f.Empresa)
            .WithMany(e => e.Filiais)
            .HasForeignKey(f => f.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Os relacionamentos many-to-many serão configurados nas configurações das entidades de ligação
    }
}