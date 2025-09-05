using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Application.DTOs;

public class EmpresaDto
{
    public Guid Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public EnderecoDto? Endereco { get; set; }
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<FilialDto> Filiais { get; set; } = new();
}

public class CreateEmpresaDto
{
    [Required(ErrorMessage = "Razão social é obrigatória")]
    [StringLength(255, ErrorMessage = "Razão social deve ter no máximo 255 caracteres")]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome fantasia é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome fantasia deve ter no máximo 255 caracteres")]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [StringLength(18, MinimumLength = 14, ErrorMessage = "CNPJ deve ter entre 14 e 18 caracteres")]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }

    public EnderecoDto? Endereco { get; set; }
}

public class UpdateEmpresaDto
{
    [Required(ErrorMessage = "Razão social é obrigatória")]
    [StringLength(255, ErrorMessage = "Razão social deve ter no máximo 255 caracteres")]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome fantasia é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome fantasia deve ter no máximo 255 caracteres")]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [StringLength(18, MinimumLength = 14, ErrorMessage = "CNPJ deve ter entre 14 e 18 caracteres")]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }

    public EnderecoDto? Endereco { get; set; }
}