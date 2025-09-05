using Microsoft.AspNetCore.Mvc;
using GestaoRestaurante.API.Authorization;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Features.Produtos.Queries.GetAllProdutos;
using GestaoRestaurante.Application.Features.Produtos.Queries.GetProdutoById;
using GestaoRestaurante.Application.Features.Produtos.Queries.SearchProdutos;
using GestaoRestaurante.Application.Features.Produtos.Commands.CreateProduto;
using GestaoRestaurante.Application.Features.Produtos.Commands.UpdateProduto;
using GestaoRestaurante.Application.Features.Produtos.Commands.DeleteProduto;
using GestaoRestaurante.Application.Features.Produtos.Commands.UpdatePreco;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.API.Filters;
using MediatR;
using GestaoRestaurante.API.Models;

namespace GestaoRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ModuleAuthorization(ModuleNames.PRODUTOS)]
[ServiceFilter(typeof(ValidationActionFilter))]
[ServiceFilter(typeof(LoggingActionFilter))]
[ServiceFilter(typeof(PerformanceActionFilter))]
[ServiceFilter(typeof(ResponseWrapperFilter))]
public class ProdutoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationMetrics _metrics;
    private readonly ILogger<ProdutoController> _logger;

    public ProdutoController(
        IMediator mediator, 
        IApplicationMetrics metrics, 
        ILogger<ProdutoController> logger)
    {
        _mediator = mediator;
        _metrics = metrics;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todos os produtos ativos
    /// </summary>
    /// <param name="categoriaId">Filtro opcional por categoria</param>
    /// <param name="produtoVenda">Filtro opcional por produtos de venda</param>
    /// <param name="produtoEstoque">Filtro opcional por produtos de estoque</param>
    /// <returns>Lista de produtos</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetProdutos(
        [FromQuery] Guid? categoriaId = null,
        [FromQuery] bool? produtoVenda = null,
        [FromQuery] bool? produtoEstoque = null)
    {
        _metrics.IncrementCounter("produto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "get_all",
            ["has_filters"] = (categoriaId.HasValue || produtoVenda.HasValue || produtoEstoque.HasValue).ToString()
        });

        var query = new GetAllProdutosQuery 
        { 
            CategoriaId = categoriaId, 
            ProdutoVenda = produtoVenda, 
            ProdutoEstoque = produtoEstoque 
        };
        
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retorna um produto específico por ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Produto encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProdutoDto>> GetProduto(Guid id)
    {
        _metrics.IncrementCounter("produto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "get_by_id",
            ["produto_id"] = id.ToString()
        });

        var query = new GetProdutoByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Contains("Produto não encontrado"))
            {
                return NotFound(result.Errors.First());
            }
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    /// <param name="createDto">Dados do produto</param>
    /// <returns>Produto criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProdutoDto>> CreateProduto(CreateProdutoDto createDto)
    {
        _metrics.IncrementCounter("produto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "create"
        });

        // ModelState é automaticamente validado pelo FluentValidation através do ValidationActionFilter
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            
            return BadRequest(new ValidationErrorResponse { Errors = errors });
        }

        var command = new CreateProdutoCommand(
            createDto.CategoriaId,
            createDto.Codigo,
            createDto.Nome,
            createDto.Descricao,
            createDto.Preco,
            createDto.UnidadeMedida,
            createDto.ProdutoVenda,
            createDto.ProdutoEstoque
        );
        
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("já existe") || e.Contains("código")))
            {
                return Conflict(string.Join(", ", result.Errors));
            }
            
            return BadRequest(string.Join(", ", result.Errors));
        }

        return CreatedAtAction(nameof(GetProduto), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="updateDto">Dados atualizados do produto</param>
    /// <returns>Produto atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProdutoDto>> UpdateProduto(Guid id, UpdateProdutoDto updateDto)
    {
        _metrics.IncrementCounter("produto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "update",
            ["produto_id"] = id.ToString()
        });

        // ModelState é automaticamente validado pelo FluentValidation através do ValidationActionFilter
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            
            return BadRequest(new ValidationErrorResponse { Errors = errors });
        }

        var command = new UpdateProdutoCommand(id, updateDto);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Contains("Produto não encontrado"))
            {
                return NotFound(result.Errors.First());
            }
            
            if (result.Errors.Any(e => e.Contains("já existe") || e.Contains("código")))
            {
                return Conflict(string.Join(", ", result.Errors));
            }
            
            return BadRequest(string.Join(", ", result.Errors));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Desativa um produto (soft delete)
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Confirmação da desativação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduto(Guid id)
    {
        _metrics.IncrementCounter("produto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "delete",
            ["produto_id"] = id.ToString()
        });

        var command = new DeleteProdutoCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Contains("Produto não encontrado"))
            {
                return NotFound(result.Errors.First());
            }
            
            if (result.Errors.Any(e => e.Contains("histórico") || e.Contains("movimentações")))
            {
                return BadRequest(result.Errors.First());
            }
            
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        return NoContent();
    }

    /// <summary>
    /// Busca produtos por termo de pesquisa
    /// </summary>
    /// <param name="termo">Termo a ser pesquisado</param>
    /// <param name="produtoVenda">Filtro opcional por produtos de venda</param>
    /// <returns>Lista de produtos encontrados</returns>
    [HttpGet("buscar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> BuscarProdutos(
        [FromQuery] string termo,
        [FromQuery] bool? produtoVenda = null)
    {
        _metrics.IncrementCounter("produto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "search",
            ["term_length"] = termo?.Length.ToString() ?? "0",
            ["has_venda_filter"] = produtoVenda.HasValue.ToString()
        });

        var query = new SearchProdutosQuery(termo, produtoVenda);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("obrigatório") || e.Contains("caracteres")))
            {
                return BadRequest(result.Errors.First());
            }
            
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Atualiza o preço de um produto
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="novoPreco">Novo preço do produto</param>
    /// <returns>Produto com preço atualizado</returns>
    [HttpPatch("{id}/preco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProdutoDto>> UpdatePreco(Guid id, [FromBody] decimal novoPreco)
    {
        _metrics.IncrementCounter("produto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "update_preco",
            ["produto_id"] = id.ToString(),
            ["novo_preco"] = novoPreco.ToString("F2")
        });

        var command = new UpdatePrecoCommand(id, novoPreco);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Contains("Produto não encontrado"))
            {
                return NotFound(result.Errors.First());
            }
            
            if (result.Errors.Any(e => e.Contains("Preço deve ser maior")))
            {
                return BadRequest(result.Errors.First());
            }
            
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        return Ok(result.Value);
    }
}