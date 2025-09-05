using GestaoRestaurante.Domain.ValueObjects;
using GestaoRestaurante.Domain.Events;
using GestaoRestaurante.Domain.Aggregates;
using GestaoRestaurante.Domain.Exceptions;

namespace GestaoRestaurante.Domain.Entities;

public class Empresa : BaseEntity, IAggregateRoot
{
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public Endereco Endereco { get; set; } = null!;
    
    // Versão para controle de concorrência otimista
    public long Version { get; private set; } = 1;
    
    // Relacionamentos
    public virtual ICollection<Filial> Filiais { get; set; } = [];
    public virtual AssinaturaEmpresa? AssinaturaAtiva { get; set; }

    // Construtores
    public Empresa() { } // Para EF Core e Seeder

    public Empresa(string razaoSocial, string nomeFantasia, string cnpj, string email, Endereco endereco, string? telefone = null)
    {
        AtualizarDados(razaoSocial, nomeFantasia, cnpj, email, endereco, telefone);
        
        // Disparar evento de criação
        AddDomainEvent(new EmpresaCriadaEvent(Id, razaoSocial, nomeFantasia, cnpj, email));
    }

    // Métodos de domínio
    public void AtualizarDados(string razaoSocial, string nomeFantasia, string cnpj, string email, Endereco endereco, string? telefone = null)
    {
        ValidarDados(razaoSocial, nomeFantasia, cnpj, email, endereco);
        
        RazaoSocial = razaoSocial.Trim();
        NomeFantasia = nomeFantasia.Trim();
        Cnpj = cnpj.Trim();
        Email = email.Trim().ToLowerInvariant();
        Endereco = endereco;
        Telefone = telefone?.Trim();
        AtualizarTimestamp();
        IncrementVersion();
    }

    public void AtualizarTelefone(string? telefone)
    {
        Telefone = telefone?.Trim();
        AtualizarTimestamp();
        IncrementVersion();
    }

    public bool PossuiFilialMatriz() => Filiais.Any(f => f.Matriz);

    public Filial? ObterFilialMatriz() => Filiais.FirstOrDefault(f => f.Matriz);

    public override void Ativar()
    {
        base.Ativar();
        AddDomainEvent(new EmpresaReativadaEvent(Id, RazaoSocial));
    }

    public override void Inativar()
    {
        base.Inativar();
        AddDomainEvent(new EmpresaInativadaEvent(Id, RazaoSocial));
    }

    // Implementação do IAggregateRoot
    public void ValidateInvariants()
    {
        var errors = new List<string>();

        // Validar dados obrigatórios
        if (string.IsNullOrWhiteSpace(RazaoSocial))
            errors.Add("Razão Social é obrigatória");
        
        if (string.IsNullOrWhiteSpace(NomeFantasia))
            errors.Add("Nome Fantasia é obrigatório");
        
        if (string.IsNullOrWhiteSpace(Cnpj))
            errors.Add("CNPJ é obrigatório");
        
        if (string.IsNullOrWhiteSpace(Email))
            errors.Add("Email é obrigatório");
        
        if (Endereco == null)
            errors.Add("Endereço é obrigatório");

        // Validar tamanhos
        if (RazaoSocial?.Length > 200)
            errors.Add("Razão Social deve ter no máximo 200 caracteres");
        
        if (NomeFantasia?.Length > 200)
            errors.Add("Nome Fantasia deve ter no máximo 200 caracteres");
        
        if (Cnpj?.Length != 14)
            errors.Add("CNPJ deve ter exatamente 14 dígitos");
        
        if (Email?.Length > 100)
            errors.Add("Email deve ter no máximo 100 caracteres");
        
        if (Telefone?.Length > 20)
            errors.Add("Telefone deve ter no máximo 20 caracteres");

        // Validar regras de negócio específicas
        ValidateBusinessRules(errors);

        if (errors.Count > 0)
            throw new DomainValidationException($"Empresa inválida: {string.Join(", ", errors)}");
    }

    private void ValidateBusinessRules(List<string> errors)
    {
        // Uma empresa deve ter pelo menos uma filial matriz
        if (Filiais.Count > 0 && !Filiais.Any(f => f.Matriz))
            errors.Add("Empresa deve ter uma filial matriz");

        // Apenas uma filial pode ser matriz
        if (Filiais.Count(f => f.Matriz) > 1)
            errors.Add("Empresa pode ter apenas uma filial matriz");

        // Se empresa tem assinatura, deve estar válida
        if (AssinaturaAtiva != null)
        {
            if (AssinaturaAtiva.DataVencimento <= DateTime.UtcNow && AssinaturaAtiva.Ativa)
                errors.Add("Assinatura vencida não pode estar ativa");
        }
    }

    private void IncrementVersion()
    {
        Version++;
    }

    private static void ValidarDados(string razaoSocial, string nomeFantasia, string cnpj, string email, Endereco endereco)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(razaoSocial, nameof(razaoSocial));
        ArgumentException.ThrowIfNullOrWhiteSpace(nomeFantasia, nameof(nomeFantasia));
        ArgumentException.ThrowIfNullOrWhiteSpace(cnpj, nameof(cnpj));
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentNullException.ThrowIfNull(endereco, nameof(endereco));
    }
}