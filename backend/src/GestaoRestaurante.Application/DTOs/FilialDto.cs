using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Application.DTOs;

public class FilialDto
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public EnderecoDto? Endereco { get; set; }
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public string EmpresaNome { get; set; } = string.Empty;
}

public class CreateFilialDto
{
    [Required(ErrorMessage = "ID da empresa é obrigatório")]
    public Guid EmpresaId { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(18, MinimumLength = 14, ErrorMessage = "CNPJ deve ter entre 14 e 18 caracteres")]
    public string? Cnpj { get; set; }

    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string? Email { get; set; }

    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }

    public EnderecoDto? Endereco { get; set; }
}

public class UpdateFilialDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(18, MinimumLength = 14, ErrorMessage = "CNPJ deve ter entre 14 e 18 caracteres")]
    public string? Cnpj { get; set; }

    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string? Email { get; set; }

    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }

    public EnderecoDto? Endereco { get; set; }
}