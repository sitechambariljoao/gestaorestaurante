using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
    public UsuarioLogadoDto Usuario { get; set; } = null!;
}

public class UsuarioLogadoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public Guid EmpresaId { get; set; }
    public string EmpresaNome { get; set; } = string.Empty;
    public List<string> ModulosLiberados { get; set; } = new();
    public List<Guid> FiliaisAcesso { get; set; } = new();
}

public class RegistrarUsuarioDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [MaxLength(100, ErrorMessage = "Email não pode exceder 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("Senha", ErrorMessage = "Senha e confirmação devem ser iguais")]
    public string ConfirmarSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "EmpresaId é obrigatório")]
    public Guid EmpresaId { get; set; }

    [MaxLength(18, ErrorMessage = "CPF não pode exceder 18 caracteres")]
    public string? Cpf { get; set; }

    [MaxLength(50, ErrorMessage = "Perfil não pode exceder 50 caracteres")]
    public string Perfil { get; set; } = "USUARIO";

    public List<Guid> FiliaisAcesso { get; set; } = new();
}

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAlteracao { get; set; }
    public Guid EmpresaId { get; set; }
    public string EmpresaNome { get; set; } = string.Empty;
    public List<Guid> FiliaisAcesso { get; set; } = new();
}