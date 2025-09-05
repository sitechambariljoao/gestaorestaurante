using Microsoft.AspNetCore.Mvc;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.API.Authorization;
using GestaoRestaurante.API.Models;
using GestaoRestaurante.Application.Features.Agrupamentos.Queries.GetAllAgrupamentos;
using GestaoRestaurante.Application.Features.Agrupamentos.Queries.GetAgrupamentoById;
using GestaoRestaurante.Application.Features.Agrupamentos.Commands.CreateAgrupamento;
using GestaoRestaurante.Application.Features.Agrupamentos.Commands.UpdateAgrupamento;
using GestaoRestaurante.Application.Features.Agrupamentos.Commands.DeleteAgrupamento;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.API.Filters;
using MediatR;

namespace GestaoRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ModuleAuthorization(ModuleNames.CENTRO_CUSTO)]
[ServiceFilter(typeof(ValidationActionFilter))]
[ServiceFilter(typeof(LoggingActionFilter))]
[ServiceFilter(typeof(PerformanceActionFilter))]
[ServiceFilter(typeof(ResponseWrapperFilter))]
public class AgrupamentoController(
    IMediator mediator,
    IApplicationMetrics metrics,
    ILogger<AgrupamentoController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IApplicationMetrics _metrics = metrics;
    private readonly ILogger<AgrupamentoController> _logger = logger;


    /// <summary>
    /// Retorna todos os agrupamentos ativos
    /// </summary>
    /// <param name="filialId">Filtro opcional por filial</param>
    /// <returns>Lista de agrupamentos</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AgrupamentoDto>>> GetAgrupamentos(
        [FromQuery] Guid? filialId = null)
    {
        _metrics.IncrementCounter("agrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "get_all",
            ["has_filial_filter"] = filialId.HasValue.ToString()
        });

        var query = new GetAllAgrupamentosQuery 
        { 
            FilialId = filialId 
        };
        
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("apenas")))
            {
                return BadRequest(result.Errors[0]);
            }
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        // Retorna 204 No Content se não houver agrupamentos cadastrados
        if (result.Value == null || !result.Value.Any())
        {
            return NoContent();
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Retorna um agrupamento específico por ID
    /// </summary>
    /// <param name="id">ID do agrupamento</param>
    /// <returns>Agrupamento encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AgrupamentoDto>> GetAgrupamento(Guid id)
    {
        _metrics.IncrementCounter("agrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "get_by_id",
            ["agrupamento_id"] = id.ToString()
        });

        var query = new GetAgrupamentoByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Contains("Agrupamento não encontrado"))
            {
                return NotFound(result.Errors[0]);
            }
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo agrupamento
    /// </summary>
    /// <param name="createDto">Dados do agrupamento</param>
    /// <returns>Agrupamento criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AgrupamentoDto>> CreateAgrupamento(CreateAgrupamentoDto createDto)
    {
        _metrics.IncrementCounter("agrupamento.controller.requests", new Dictionary<string, string> 
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

        var command = new CreateAgrupamentoCommand(createDto);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("já existe") || e.Contains("Já existe")))
            {
                return Conflict(string.Join(", ", result.Errors));
            }
            
            return BadRequest(string.Join(", ", result.Errors));
        }

        return CreatedAtAction(nameof(GetAgrupamento), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza um agrupamento existente
    /// </summary>
    /// <param name="id">ID do agrupamento</param>
    /// <param name="updateDto">Dados atualizados do agrupamento</param>
    /// <returns>Agrupamento atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AgrupamentoDto>> UpdateAgrupamento(Guid id, UpdateAgrupamentoDto updateDto)
    {
        _metrics.IncrementCounter("agrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "update",
            ["agrupamento_id"] = id.ToString()
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

        var command = new UpdateAgrupamentoCommand(id, updateDto);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Contains("Agrupamento não encontrado"))
            {
                return NotFound(result.Errors.First());
            }
            
            if (result.Errors.Any(e => e.Contains("já existe") || e.Contains("Já existe")))
            {
                return Conflict(string.Join(", ", result.Errors));
            }
            
            return BadRequest(string.Join(", ", result.Errors));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Desativa um agrupamento (soft delete)
    /// </summary>
    /// <param name="id">ID do agrupamento</param>
    /// <returns>Confirmação da desativação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAgrupamento(Guid id)
    {
        _metrics.IncrementCounter("agrupamento.controller.requests", new Dictionary<string, string> 
        { 
            ["endpoint"] = "delete",
            ["agrupamento_id"] = id.ToString()
        });

        var command = new DeleteAgrupamentoCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Contains("Agrupamento não encontrado"))
            {
                return NotFound(result.Errors[0]);
            }
            
            if (result.Errors.Any(e => e.Contains("sub-agrupamentos") || e.Contains("dependências")))
            {
                return BadRequest(result.Errors[0]);
            }
            
            return StatusCode(500, string.Join(", ", result.Errors));
        }

        return NoContent();
    }

}