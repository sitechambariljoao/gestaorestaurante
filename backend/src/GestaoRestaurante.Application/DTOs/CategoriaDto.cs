using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Application.DTOs;

public class CategoriaDto
{
    public Guid Id { get; set; }
    public Guid CentroCustoId { get; set; }
    public Guid? CategoriaPaiId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Nivel { get; set; }
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public string CentroCustoNome { get; set; } = string.Empty;
    public string SubAgrupamentoNome { get; set; } = string.Empty;
    public string AgrupamentoNome { get; set; } = string.Empty;
    public string EmpresaNome { get; set; } = string.Empty;
    public string? CategoriaPaiNome { get; set; }
    public int TotalCategoriasFilhas { get; set; }
    public int TotalProdutos { get; set; }
    public List<CategoriaDto> CategoriasFilhas { get; set; } = new();
}

public class CreateCategoriaDto
{
    [Required(ErrorMessage = "ID do centro de custo é obrigatório")]
    public Guid CentroCustoId { get; set; }

    public Guid? CategoriaPaiId { get; set; }

    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(20, ErrorMessage = "Código deve ter no máximo 20 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    [Range(1, 3, ErrorMessage = "Nível deve ser entre 1 e 3")]
    public int Nivel { get; set; } = 1;
}

public class UpdateCategoriaDto
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