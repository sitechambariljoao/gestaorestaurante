using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Application.DTOs;

public class AgrupamentoDto
{
    public Guid Id { get; set; }
    public Guid FilialId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public string FilialNome { get; set; } = string.Empty;
    public string EmpresaNome { get; set; } = string.Empty;
    public int TotalSubAgrupamentos { get; set; }
}

public class CreateAgrupamentoDto
{
    [Required(ErrorMessage = "ID da filial é obrigatório")]
    public Guid FilialId { get; set; }

    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(20, ErrorMessage = "Código deve ter no máximo 20 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }
}

public class UpdateAgrupamentoDto
{
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(20, ErrorMessage = "Código deve ter no máximo 20 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }
}