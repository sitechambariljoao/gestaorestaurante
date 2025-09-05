using GestaoRestaurante.Domain.ValueObjects;

namespace GestaoRestaurante.Domain.Entities;

public class Filial : BaseEntity
{
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica se esta filial é a matriz da empresa
    /// </summary>
    public bool Matriz { get; set; } = false;
    
    public string? CnpjFilial { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public Endereco? EnderecoFilial { get; set; }
    
    /// <summary>
    /// CNPJ efetivo da filial - se for matriz, retorna o CNPJ da empresa
    /// </summary>
    public string Cnpj => Matriz ? Empresa?.Cnpj ?? CnpjFilial ?? string.Empty : CnpjFilial ?? string.Empty;
    
    /// <summary>
    /// Endereço efetivo da filial - se for matriz, retorna o endereço da empresa
    /// </summary>
    public Endereco? Endereco => Matriz ? Empresa?.Endereco : EnderecoFilial;
    
    // Relacionamentos
    public virtual Empresa Empresa { get; set; } = null!;
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public virtual ICollection<Agrupamento> Agrupamentos { get; set; } = new List<Agrupamento>();

    // Construtores
    public Filial() { } // Para EF Core e Seeder

    public Filial(Guid empresaId, string nome, bool matriz = false, string? cnpjFilial = null, 
                  string? email = null, string? telefone = null, Endereco? enderecoFilial = null)
    {
        AtualizarDados(empresaId, nome, matriz, cnpjFilial, email, telefone, enderecoFilial);
    }

    // Métodos de domínio
    public void AtualizarDados(Guid empresaId, string nome, bool matriz = false, string? cnpjFilial = null, 
                               string? email = null, string? telefone = null, Endereco? enderecoFilial = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome, nameof(nome));
        
        EmpresaId = empresaId;
        Nome = nome.Trim();
        Matriz = matriz;
        CnpjFilial = cnpjFilial?.Trim();
        Email = email?.Trim().ToLowerInvariant();
        Telefone = telefone?.Trim();
        EnderecoFilial = enderecoFilial;
        AtualizarTimestamp();
    }
    
    /// <summary>
    /// Valida se esta filial pode ser definida como matriz
    /// </summary>
    public bool PodeSerMatriz()
    {
        if (!Matriz) return true; // Se não é matriz, sempre pode
        
        // Se é matriz, verifica se há outra matriz na mesma empresa
        return Empresa?.Filiais?.Count(f => f.Matriz && f.Id != Id) == 0;
    }
    
    /// <summary>
    /// Define esta filial como matriz da empresa
    /// </summary>
    public void DefinirComoMatriz()
    {
        if (!PodeSerMatriz())
            throw new InvalidOperationException("Não é possível definir esta filial como matriz. Já existe uma matriz para esta empresa.");
            
        Matriz = true;
        AtualizarTimestamp();
    }
    
    /// <summary>
    /// Remove o status de matriz desta filial
    /// </summary>
    public void RemoverStatusMatriz()
    {
        Matriz = false;
        AtualizarTimestamp();
    }
}