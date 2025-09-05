using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Application.DTOs;

public class ProdutoDto
{
    public Guid Id { get; set; }
    public Guid CategoriaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
    public bool Ativa { get; set; }
    public bool ProdutoVenda { get; set; }
    public bool ProdutoEstoque { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAlteracao { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public string CentroCustoNome { get; set; } = string.Empty;
    public string SubAgrupamentoNome { get; set; } = string.Empty;
    public string AgrupamentoNome { get; set; } = string.Empty;
    public string EmpresaNome { get; set; } = string.Empty;
    public int TotalIngredientes { get; set; }
}

public class CreateProdutoDto
{
    [Required(ErrorMessage = "ID da categoria é obrigatório")]
    public Guid CategoriaId { get; set; }

    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "Unidade de medida é obrigatória")]
    [StringLength(10, ErrorMessage = "Unidade de medida deve ter no máximo 10 caracteres")]
    public string UnidadeMedida { get; set; } = string.Empty;

    public bool ProdutoVenda { get; set; } = true;

    public bool ProdutoEstoque { get; set; } = false;
}

public class UpdateProdutoDto
{
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "Unidade de medida é obrigatória")]
    [StringLength(10, ErrorMessage = "Unidade de medida deve ter no máximo 10 caracteres")]
    public string UnidadeMedida { get; set; } = string.Empty;

    public bool ProdutoVenda { get; set; } = true;

    public bool ProdutoEstoque { get; set; } = false;
}