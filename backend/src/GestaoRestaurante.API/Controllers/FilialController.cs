using Microsoft.AspNetCore.Mvc;
using MediatR;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Features.Filiais.Queries.GetAllFiliais;
using GestaoRestaurante.Application.Features.Filiais.Queries.GetFilialById;
using GestaoRestaurante.Application.Features.Filiais.Commands.CreateFilial;
using GestaoRestaurante.Application.Features.Filiais.Commands.UpdateFilial;
using GestaoRestaurante.Application.Features.Filiais.Commands.DeleteFilial;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.API.Authorization;
using GestaoRestaurante.API.Models;
using GestaoRestaurante.API.Filters;

namespace GestaoRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ModuleAuthorization(ModuleNames.FILIAIS)]
[ServiceFilter(typeof(ValidationActionFilter))]
[ServiceFilter(typeof(LoggingActionFilter))]
[ServiceFilter(typeof(PerformanceActionFilter))]
[ServiceFilter(typeof(ResponseWrapperFilter))]
public class FilialController(IMediator mediator, IApplicationMetrics metrics, ILogger<FilialController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IApplicationMetrics _metrics = metrics;
    private readonly ILogger<FilialController> _logger = logger;


    /// <summary>
    /// Retorna todas as filiais ativas
    /// </summary>
    /// <param name="empresaId">Filtro opcional por empresa</param>
    /// <returns>Lista de filiais</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<FilialDto>>> GetFiliais([FromQuery] Guid? empresaId = null)
    {
        _metrics.IncrementCounter("filial.controller.requests", new Dictionary<string, string> { ["endpoint"] = "get_all" });

        var query = new GetAllFiliaisQuery { EmpresaId = empresaId };
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
    /// Retorna uma filial específica por ID
    /// </summary>
    /// <param name="id">ID da filial</param>
    /// <returns>Filial encontrada</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FilialDto>> GetFilial(Guid id)
    {
        _metrics.IncrementCounter("filial.controller.requests", new Dictionary<string, string> { ["endpoint"] = "get_by_id" });

        var query = new GetFilialByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            return errorMessage.Contains("não encontrada") ? NotFound(errorMessage) : StatusCode(500, errorMessage);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria uma nova filial
    /// </summary>
    /// <param name="createDto">Dados da filial</param>
    /// <returns>Filial criada</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FilialDto>> CreateFilial(CreateFilialDto createDto)
    {
        _metrics.IncrementCounter("filial.controller.requests", new Dictionary<string, string> { ["endpoint"] = "create" });

        var command = new CreateFilialCommand(
            createDto.EmpresaId,
            createDto.Nome,
            createDto.Cnpj,
            createDto.Email,
            createDto.Telefone,
            createDto.Endereco
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

        return CreatedAtAction(nameof(GetFilial), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza uma filial existente
    /// </summary>
    /// <param name="id">ID da filial</param>
    /// <param name="updateDto">Dados atualizados da filial</param>
    /// <returns>Filial atualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FilialDto>> UpdateFilial(Guid id, UpdateFilialDto updateDto)
    {
        _metrics.IncrementCounter("filial.controller.requests", new Dictionary<string, string> { ["endpoint"] = "update" });

        var command = new UpdateFilialCommand(
            id,
            updateDto.Nome,
            updateDto.Cnpj,
            updateDto.Email,
            updateDto.Telefone,
            updateDto.Endereco
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
    /// Desativa uma filial (soft delete)
    /// </summary>
    /// <param name="id">ID da filial</param>
    /// <returns>Confirmação da desativação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteFilial(Guid id)
    {
        _metrics.IncrementCounter("filial.controller.requests", new Dictionary<string, string> { ["endpoint"] = "delete" });

        var command = new DeleteFilialCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            
            if (errorMessage.Contains("não encontrada"))
            {
                return NotFound(errorMessage);
            }
            
            if (errorMessage.Contains("única filial") || errorMessage.Contains("pelo menos uma"))
            {
                return BadRequest(errorMessage);
            }
            
            return StatusCode(500, errorMessage);
        }

        return NoContent();
    }
}