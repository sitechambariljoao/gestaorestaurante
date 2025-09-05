using Microsoft.AspNetCore.Mvc;
using MediatR;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.CreateSubAgrupamento;
using GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.UpdateSubAgrupamento;
using GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.DeleteSubAgrupamento;
using GestaoRestaurante.Application.Features.SubAgrupamentos.Queries.GetAllSubAgrupamentos;
using GestaoRestaurante.Application.Features.SubAgrupamentos.Queries.GetSubAgrupamentoById;
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
public class SubAgrupamentoController(IMediator mediator, IApplicationMetrics metrics, ILogger<SubAgrupamentoController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IApplicationMetrics _metrics = metrics;
    private readonly ILogger<SubAgrupamentoController> _logger = logger;


    /// <summary>
    /// Retorna todos os sub-agrupamentos ativos
    /// </summary>
    /// <param name="agrupamentoId">Filtro opcional por agrupamento</param>
    /// <returns>Lista de sub-agrupamentos</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SubAgrupamentoDto>>> GetSubAgrupamentos([FromQuery] Guid? agrupamentoId = null)
    {
        _metrics.IncrementCounter("subagrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "get_all",
            ["has_agrupamento_filter"] = agrupamentoId.HasValue.ToString()
        });

        var query = new GetAllSubAgrupamentosQuery
        {
            AgrupamentoId = agrupamentoId
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
    /// Retorna um sub-agrupamento específico por ID
    /// </summary>
    /// <param name="id">ID do sub-agrupamento</param>
    /// <returns>Sub-agrupamento encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubAgrupamentoDto>> GetSubAgrupamento(Guid id)
    {
        _metrics.IncrementCounter("subagrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "get_by_id",
            ["subagrupamento_id"] = id.ToString()
        });

        var query = new GetSubAgrupamentoByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            return errorMessage.Contains("não encontrado") ? NotFound(errorMessage) : StatusCode(500, errorMessage);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo sub-agrupamento
    /// </summary>
    /// <param name="createDto">Dados do sub-agrupamento</param>
    /// <returns>Sub-agrupamento criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubAgrupamentoDto>> CreateSubAgrupamento(CreateSubAgrupamentoDto createDto)
    {
        _metrics.IncrementCounter("subagrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "create"
        });

        var command = new CreateSubAgrupamentoCommand(
            createDto.AgrupamentoId,
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

        return CreatedAtAction(nameof(GetSubAgrupamento), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza um sub-agrupamento existente
    /// </summary>
    /// <param name="id">ID do sub-agrupamento</param>
    /// <param name="updateDto">Dados atualizados do sub-agrupamento</param>
    /// <returns>Sub-agrupamento atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubAgrupamentoDto>> UpdateSubAgrupamento(Guid id, UpdateSubAgrupamentoDto updateDto)
    {
        _metrics.IncrementCounter("subagrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "update",
            ["subagrupamento_id"] = id.ToString()
        });

        var command = new UpdateSubAgrupamentoCommand(
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
    /// Desativa um sub-agrupamento (soft delete)
    /// </summary>
    /// <param name="id">ID do sub-agrupamento</param>
    /// <returns>Confirmação da desativação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSubAgrupamento(Guid id)
    {
        _metrics.IncrementCounter("subagrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "delete",
            ["subagrupamento_id"] = id.ToString()
        });

        var command = new DeleteSubAgrupamentoCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            
            if (errorMessage.Contains("não encontrado"))
            {
                return NotFound(errorMessage);
            }
            
            if (errorMessage.Contains("centro(s) de custo"))
            {
                return BadRequest(errorMessage);
            }
            
            return StatusCode(500, errorMessage);
        }

        return NoContent();
    }
}