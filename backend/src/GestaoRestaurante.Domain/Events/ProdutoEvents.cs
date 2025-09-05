using GestaoRestaurante.Domain.ValueObjects;

namespace GestaoRestaurante.Domain.Events;

/// <summary>
/// Evento disparado quando um novo produto é criado
/// </summary>
public class ProdutoCriadoEvent : DomainEvent
{
    public Guid ProdutoId { get; }
    public Guid CategoriaId { get; }
    public string Codigo { get; }
    public string Nome { get; }
    public decimal Preco { get; }
    public string UnidadeMedida { get; }

    public ProdutoCriadoEvent(Guid produtoId, Guid categoriaId, string codigo, string nome, decimal preco, string unidadeMedida)
    {
        ProdutoId = produtoId;
        CategoriaId = categoriaId;
        Codigo = codigo;
        Nome = nome;
        Preco = preco;
        UnidadeMedida = unidadeMedida;
    }
}

/// <summary>
/// Evento disparado quando o preço de um produto é alterado
/// </summary>
public class ProdutoPrecoAlteradoEvent : DomainEvent
{
    public Guid ProdutoId { get; }
    public string Nome { get; }
    public decimal PrecoAnterior { get; }
    public decimal PrecoNovo { get; }
    public Percentual VariacaoPercentual { get; }
    public string TipoAlteracao { get; }

    public ProdutoPrecoAlteradoEvent(Guid produtoId, string nome, decimal precoAnterior, decimal precoNovo)
    {
        ProdutoId = produtoId;
        Nome = nome;
        PrecoAnterior = precoAnterior;
        PrecoNovo = precoNovo;
        
        var variacao = ((precoNovo - precoAnterior) / precoAnterior) * 100;
        VariacaoPercentual = Percentual.Create(Math.Abs(variacao));
        TipoAlteracao = precoNovo > precoAnterior ? "Aumento" : "Redução";
    }
}

/// <summary>
/// Evento disparado quando um produto é configurado para venda
/// </summary>
public class ProdutoConfiguradoParaVendaEvent : DomainEvent
{
    public Guid ProdutoId { get; }
    public string Nome { get; }
    public bool ProdutoVenda { get; }
    public bool ProdutoEstoque { get; }

    public ProdutoConfiguradoParaVendaEvent(Guid produtoId, string nome, bool produtoVenda, bool produtoEstoque)
    {
        ProdutoId = produtoId;
        Nome = nome;
        ProdutoVenda = produtoVenda;
        ProdutoEstoque = produtoEstoque;
    }
}

/// <summary>
/// Evento disparado quando um produto é inativado
/// </summary>
public class ProdutoInativadoEvent : DomainEvent
{
    public Guid ProdutoId { get; }
    public string Nome { get; }
    public string Codigo { get; }
    public string Motivo { get; }

    public ProdutoInativadoEvent(Guid produtoId, string nome, string codigo, string motivo = "")
    {
        ProdutoId = produtoId;
        Nome = nome;
        Codigo = codigo;
        Motivo = motivo;
    }
}

/// <summary>
/// Evento disparado quando um produto é movido para outra categoria
/// </summary>
public class ProdutoMovidoCategoriaEvent : DomainEvent
{
    public Guid ProdutoId { get; }
    public string Nome { get; }
    public Guid CategoriaAnteriorId { get; }
    public Guid CategoriaNovaId { get; }

    public ProdutoMovidoCategoriaEvent(Guid produtoId, string nome, Guid categoriaAnteriorId, Guid categoriaNovaId)
    {
        ProdutoId = produtoId;
        Nome = nome;
        CategoriaAnteriorId = categoriaAnteriorId;
        CategoriaNovaId = categoriaNovaId;
    }
}