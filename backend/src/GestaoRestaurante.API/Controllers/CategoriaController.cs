using Microsoft.AspNetCore.Mvc;
using MediatR;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Features.Categorias.Commands.CreateCategoria;
using GestaoRestaurante.Application.Features.Categorias.Commands.UpdateCategoria;
using GestaoRestaurante.Application.Features.Categorias.Commands.DeleteCategoria;
using GestaoRestaurante.Application.Features.Categorias.Queries.GetAllCategorias;
using GestaoRestaurante.Application.Features.Categorias.Queries.GetCategoriaById;
using GestaoRestaurante.API.Authorization;
using GestaoRestaurante.API.Models;
using GestaoRestaurante.API.Filters;

namespace GestaoRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ModuleAuthorization(ModuleNames.CATEGORIAS)]
[ServiceFilter(typeof(ValidationActionFilter))]
[ServiceFilter(typeof(LoggingActionFilter))]
[ServiceFilter(typeof(PerformanceActionFilter))]
[ServiceFilter(typeof(ResponseWrapperFilter))]
public class CategoriaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CategoriaController> _logger;

    public CategoriaController(IMediator mediator, ILogger<CategoriaController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as categorias ativas
    /// </summary>
    /// <param name="centroCustoId">Filtro opcional por centro de custo</param>
    /// <param name="nivel">Filtro opcional por nível hierárquico (1, 2 ou 3)</param>
    /// <returns>Lista de categorias</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias([FromQuery] Guid? centroCustoId = null, [FromQuery] int? nivel = null)
    {
        var query = new GetAllCategoriasQuery
        {
            CentroCustoId = centroCustoId,
            Nivel = nivel
        };
        
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            return errorMessage.Contains("Nível deve estar") ? BadRequest(errorMessage) : StatusCode(500, errorMessage);
        }

        if (result.Value == null || !result.Value.Any())
        {
            return NoContent();
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retorna uma categoria específica por ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>Categoria encontrada</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(Guid id)
    {
        var query = new GetCategoriaByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            return errorMessage.Contains("não encontrada") ? NotFound(errorMessage) : StatusCode(500, errorMessage);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    /// <param name="createDto">Dados da categoria</param>
    /// <returns>Categoria criada</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoriaDto>> CreateCategoria(CreateCategoriaDto createDto)
    {
        var command = new CreateCategoriaCommand(
            createDto.CentroCustoId,
            createDto.CategoriaPaiId,
            createDto.Codigo,
            createDto.Nome,
            createDto.Descricao,
            createDto.Nivel
        );
        
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            var errors = new Dictionary<string, string[]> 
            { 
                { "general", result.Errors.ToArray() } 
            };
            return BadRequest(new ValidationErrorResponse { Errors = errors });
        }

        return CreatedAtAction(nameof(GetCategoria), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza uma categoria existente
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <param name="updateDto">Dados atualizados da categoria</param>
    /// <returns>Categoria atualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoriaDto>> UpdateCategoria(Guid id, UpdateCategoriaDto updateDto)
    {
        var command = new UpdateCategoriaCommand(
            id,
            updateDto.Codigo,
            updateDto.Nome,
            updateDto.Descricao
        );
        
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            
            if (errorMessage.Contains("não encontrada"))
            {
                return NotFound(errorMessage);
            }
            
            var errors = new Dictionary<string, string[]> 
            { 
                { "general", result.Errors.ToArray() } 
            };
            return BadRequest(new ValidationErrorResponse { Errors = errors });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Desativa uma categoria (soft delete)
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>Confirmação da desativação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteCategoria(Guid id)
    {
        var command = new DeleteCategoriaCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            
            if (errorMessage.Contains("não encontrada"))
            {
                return NotFound(errorMessage);
            }
            
            if (errorMessage.Contains("categoria(s) filha(s)") || errorMessage.Contains("produto(s)"))
            {
                return BadRequest(errorMessage);
            }
            
            return StatusCode(500, errorMessage);
        }

        return NoContent();
    }

}