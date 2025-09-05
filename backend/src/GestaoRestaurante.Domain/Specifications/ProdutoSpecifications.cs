using System.Linq.Expressions;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Specifications;

/// <summary>
/// Especificação para filtrar produtos ativos
/// </summary>
public class ProdutoAtivoSpecification : Specification<Produto>
{
    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.Ativa;
    }
}

/// <summary>
/// Especificação para filtrar produtos por categoria
/// </summary>
public class ProdutoPorCategoriaSpecification : Specification<Produto>
{
    private readonly Guid _categoriaId;

    public ProdutoPorCategoriaSpecification(Guid categoriaId)
    {
        _categoriaId = categoriaId;
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.CategoriaId == _categoriaId;
    }
}

/// <summary>
/// Especificação para filtrar produtos por código
/// </summary>
public class ProdutoPorCodigoSpecification : Specification<Produto>
{
    private readonly string _codigo;

    public ProdutoPorCodigoSpecification(string codigo)
    {
        _codigo = codigo.ToUpperInvariant();
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.Codigo.ToUpper() == _codigo;
    }
}

/// <summary>
/// Especificação para filtrar produtos por nome (busca parcial)
/// </summary>
public class ProdutoPorNomeSpecification : Specification<Produto>
{
    private readonly string _nome;

    public ProdutoPorNomeSpecification(string nome)
    {
        _nome = nome.ToLowerInvariant();
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.Nome.ToLower().Contains(_nome);
    }
}

/// <summary>
/// Especificação para filtrar produtos por faixa de preço
/// </summary>
public class ProdutoPorFaixaPrecoSpecification : Specification<Produto>
{
    private readonly decimal _precoMinimo;
    private readonly decimal _precoMaximo;

    public ProdutoPorFaixaPrecoSpecification(decimal precoMinimo, decimal precoMaximo)
    {
        _precoMinimo = precoMinimo;
        _precoMaximo = precoMaximo;
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.Preco >= _precoMinimo && produto.Preco <= _precoMaximo;
    }
}

/// <summary>
/// Especificação para filtrar produtos configurados para venda
/// </summary>
public class ProdutoParaVendaSpecification : Specification<Produto>
{
    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.ProdutoVenda;
    }
}

/// <summary>
/// Especificação para filtrar produtos configurados para controle de estoque
/// </summary>
public class ProdutoComControleEstoqueSpecification : Specification<Produto>
{
    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.ProdutoEstoque;
    }
}

/// <summary>
/// Especificação para filtrar produtos por unidade de medida
/// </summary>
public class ProdutoPorUnidadeMedidaSpecification : Specification<Produto>
{
    private readonly string _unidadeMedida;

    public ProdutoPorUnidadeMedidaSpecification(string unidadeMedida)
    {
        _unidadeMedida = unidadeMedida.ToUpperInvariant();
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.UnidadeMedida.ToUpper() == _unidadeMedida;
    }
}

/// <summary>
/// Especificação para filtrar produtos com preço acima de um valor
/// </summary>
public class ProdutoPrecoAcimaDeSpecification : Specification<Produto>
{
    private readonly decimal _preco;

    public ProdutoPrecoAcimaDeSpecification(decimal preco)
    {
        _preco = preco;
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.Preco > _preco;
    }
}

/// <summary>
/// Especificação para filtrar produtos com preço abaixo de um valor
/// </summary>
public class ProdutoPrecoAbaixoDeSpecification : Specification<Produto>
{
    private readonly decimal _preco;

    public ProdutoPrecoAbaixoDeSpecification(decimal preco)
    {
        _preco = preco;
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.Preco < _preco;
    }
}

/// <summary>
/// Especificação para filtrar produtos que contém texto na descrição
/// </summary>
public class ProdutoComDescricaoSpecification : Specification<Produto>
{
    private readonly string _texto;

    public ProdutoComDescricaoSpecification(string texto)
    {
        _texto = texto.ToLowerInvariant();
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto => produto.Descricao != null && produto.Descricao.ToLower().Contains(_texto);
    }
}

/// <summary>
/// Especificação combinada para busca complexa de produtos
/// </summary>
public class ProdutoBuscaCompletaSpecification : Specification<Produto>
{
    private readonly string _termoBusca;
    private readonly Guid? _categoriaId;
    private readonly decimal? _precoMinimo;
    private readonly decimal? _precoMaximo;
    private readonly bool _apenasAtivos;
    private readonly bool? _apenasVenda;

    public ProdutoBuscaCompletaSpecification(
        string termoBusca = "",
        Guid? categoriaId = null,
        decimal? precoMinimo = null,
        decimal? precoMaximo = null,
        bool apenasAtivos = true,
        bool? apenasVenda = null)
    {
        _termoBusca = termoBusca.ToLowerInvariant();
        _categoriaId = categoriaId;
        _precoMinimo = precoMinimo;
        _precoMaximo = precoMaximo;
        _apenasAtivos = apenasAtivos;
        _apenasVenda = apenasVenda;
    }

    public override Expression<Func<Produto, bool>> ToExpression()
    {
        return produto =>
            // Filtro de ativo
            (!_apenasAtivos || produto.Ativa) &&
            
            // Filtro de categoria
            (!_categoriaId.HasValue || produto.CategoriaId == _categoriaId.Value) &&
            
            // Filtro de preço
            (!_precoMinimo.HasValue || produto.Preco >= _precoMinimo.Value) &&
            (!_precoMaximo.HasValue || produto.Preco <= _precoMaximo.Value) &&
            
            // Filtro de venda
            (!_apenasVenda.HasValue || produto.ProdutoVenda == _apenasVenda.Value) &&
            
            // Filtro de texto (busca no nome, código ou descrição)
            (string.IsNullOrEmpty(_termoBusca) ||
             produto.Nome.ToLower().Contains(_termoBusca) ||
             produto.Codigo.ToLower().Contains(_termoBusca) ||
             (produto.Descricao != null && produto.Descricao.ToLower().Contains(_termoBusca)));
    }
}