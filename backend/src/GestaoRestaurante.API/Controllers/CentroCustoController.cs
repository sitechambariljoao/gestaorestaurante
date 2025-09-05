using Microsoft.AspNetCore.Mvc;
using MediatR;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Features.CentrosCusto.Commands.CreateCentroCusto;
using GestaoRestaurante.Application.Features.CentrosCusto.Commands.UpdateCentroCusto;
using GestaoRestaurante.Application.Features.CentrosCusto.Commands.DeleteCentroCusto;
using GestaoRestaurante.Application.Features.CentrosCusto.Queries.GetAllCentrosCusto;
using GestaoRestaurante.Application.Features.CentrosCusto.Queries.GetCentroCustoById;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.API.Authorization;
using GestaoRestaurante.API.Models;
using GestaoRestaurante.API.Filters;

namespace GestaoRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ModuleAuthorization(ModuleNames.CENTRO_CUSTO)]
[ServiceFilter(typeof(ValidationActionFilter))]
[ServiceFilter(typeof(LoggingActionFilter))]
[ServiceFilter(typeof(PerformanceActionFilter))]
[ServiceFilter(typeof(ResponseWrapperFilter))]
public class CentroCustoController(IMediator mediator, IApplicationMetrics metrics, ILogger<CentroCustoController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IApplicationMetrics _metrics = metrics;
    private readonly ILogger<CentroCustoController> _logger = logger;


    /// <summary>
    /// Retorna todos os centros de custo ativos
    /// </summary>
    /// <param name="subAgrupamentoId">Filtro opcional por sub-agrupamento</param>
    /// <returns>Lista de centros de custo</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CentroCustoDto>>> GetCentrosCusto([FromQuery] Guid? subAgrupamentoId = null)
    {
        _metrics.IncrementCounter("centrocusto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "get_all",
            ["has_subagrupamento_filter"] = subAgrupamentoId.HasValue.ToString()
        });

        var query = new GetAllCentrosCustoQuery
        {
            SubAgrupamentoId = subAgrupamentoId
        };
        
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        if (result.Value == null || !result.Value.Any())
        {
            return NoContent();
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retorna um centro de custo específico por ID
    /// </summary>
    /// <param name="id">ID do centro de custo</param>
    /// <returns>Centro de custo encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CentroCustoDto>> GetCentroCusto(Guid id)
    {
        _metrics.IncrementCounter("centrocusto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "get_by_id",
            ["centrocusto_id"] = id.ToString()
        });

        var query = new GetCentroCustoByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            return errorMessage.Contains("não encontrado") ? NotFound(errorMessage) : StatusCode(500, errorMessage);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo centro de custo
    /// </summary>
    /// <param name="createDto">Dados do centro de custo</param>
    /// <returns>Centro de custo criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CentroCustoDto>> CreateCentroCusto(CreateCentroCustoDto createDto)
    {
        _metrics.IncrementCounter("centrocusto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "create"
        });

        var command = new CreateCentroCustoCommand(
            createDto.SubAgrupamentoId,
            createDto.Codigo,
            createDto.Nome,
            createDto.Descricao
        );
        
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("já existe") || e.Contains("Já existe")))
            {
                return Conflict(string.Join(", ", result.Errors));
            }
            
            var errors = new Dictionary<string, string[]> 
            { 
                { "general", result.Errors.ToArray() } 
            };
            return BadRequest(new ValidationErrorResponse { Errors = errors });
        }

        return CreatedAtAction(nameof(GetCentroCusto), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza um centro de custo existente
    /// </summary>
    /// <param name="id">ID do centro de custo</param>
    /// <param name="updateDto">Dados atualizados do centro de custo</param>
    /// <returns>Centro de custo atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CentroCustoDto>> UpdateCentroCusto(Guid id, UpdateCentroCustoDto updateDto)
    {
        _metrics.IncrementCounter("centrocusto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "update",
            ["centrocusto_id"] = id.ToString()
        });

        var command = new UpdateCentroCustoCommand(
            id,
            updateDto.Codigo,
            updateDto.Nome,
            updateDto.Descricao
        );
        
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            
            if (errorMessage.Contains("não encontrado"))
            {
                return NotFound(errorMessage);
            }
            
            if (result.Errors.Any(e => e.Contains("já existe") || e.Contains("Já existe")))
            {
                return Conflict(errorMessage);
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
    /// Desativa um centro de custo (soft delete)
    /// </summary>
    /// <param name="id">ID do centro de custo</param>
    /// <returns>Confirmação da desativação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCentroCusto(Guid id)
    {
        _metrics.IncrementCounter("centrocusto.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "delete",
            ["centrocusto_id"] = id.ToString()
        });

        var command = new DeleteCentroCustoCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            
            if (errorMessage.Contains("não encontrado"))
            {
                return NotFound(errorMessage);
            }
            
            if (errorMessage.Contains("categoria(s)"))
            {
                return BadRequest(errorMessage);
            }
            
            return StatusCode(500, errorMessage);
        }

        return NoContent();
    }

}