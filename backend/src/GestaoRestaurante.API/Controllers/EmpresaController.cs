using Microsoft.AspNetCore.Mvc;
using MediatR;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Features.Empresas.Commands.CreateEmpresa;
using GestaoRestaurante.Application.Features.Empresas.Commands.UpdateEmpresa;
using GestaoRestaurante.Application.Features.Empresas.Commands.DeleteEmpresa;
using GestaoRestaurante.Application.Features.Empresas.Queries.GetAllEmpresas;
using GestaoRestaurante.Application.Features.Empresas.Queries.GetEmpresaById;
using GestaoRestaurante.API.Models;
using GestaoRestaurante.API.Authorization;

namespace GestaoRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ModuleAuthorization(ModuleNames.EMPRESAS)]
public class EmpresaController(IMediator mediator, ILogger<EmpresaController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<EmpresaController> _logger = logger;


    /// <summary>
    /// Retorna todas as empresas ativas
    /// </summary>
    /// <returns>Lista de empresas</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EmpresaDto>>> GetEmpresas()
    {
        var query = new GetAllEmpresasQuery();
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
    /// Retorna uma empresa específica por ID
    /// </summary>
    /// <param name="id">ID da empresa</param>
    /// <returns>Empresa encontrada</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmpresaDto>> GetEmpresa(Guid id)
    {
        var query = new GetEmpresaByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            return errorMessage.Contains("não encontrada") ? NotFound(errorMessage) : StatusCode(500, errorMessage);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria uma nova empresa
    /// </summary>
    /// <param name="createDto">Dados da empresa</param>
    /// <returns>Empresa criada</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmpresaDto>> CreateEmpresa(CreateEmpresaDto createDto)
    {
        var command = new CreateEmpresaCommand(
            createDto.RazaoSocial,
            createDto.NomeFantasia,
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

        return CreatedAtAction(nameof(GetEmpresa), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza uma empresa existente
    /// </summary>
    /// <param name="id">ID da empresa</param>
    /// <param name="updateDto">Dados atualizados da empresa</param>
    /// <returns>Empresa atualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmpresaDto>> UpdateEmpresa(Guid id, UpdateEmpresaDto updateDto)
    {
        // ModelState é automaticamente validado pelo FluentValidation
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

        var command = new UpdateEmpresaCommand(
            id,
            updateDto.RazaoSocial,
            updateDto.NomeFantasia,
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
    /// Desativa uma empresa (soft delete)
    /// </summary>
    /// <param name="id">ID da empresa</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteEmpresa(Guid id)
    {
        var command = new DeleteEmpresaCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(", ", result.Errors);
            if (errorMessage.Contains("não encontrada"))
            {
                return NotFound(errorMessage);
            }
            
            if (errorMessage.Contains("filiais ativas") || errorMessage.Contains("inativa"))
            {
                return Conflict(errorMessage);
            }
            
            return StatusCode(500, errorMessage);
        }

        return NoContent();
    }

}