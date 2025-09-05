using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    /// <param name="loginDto">Dados de login</param>
    /// <returns>Token de acesso e dados do usuário</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _authService.LoginAsync(loginDto);
            if (resultado == null)
            {
                _logger.LogWarning("Tentativa de login falhada para email: {Email}", loginDto.Email);
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            _logger.LogInformation("Login realizado com sucesso para usuário: {UserId}", resultado.Usuario.Id);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login para email: {Email}", loginDto.Email);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="registrarDto">Dados do usuário a ser registrado</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("registrar")]
    [Authorize] // Só usuários autenticados podem registrar novos usuários
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto registrarDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar se o usuário atual pode registrar usuários na empresa
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            if (empresaIdClaim == null || !Guid.TryParse(empresaIdClaim, out var empresaIdUsuario))
                return Forbid("Usuário não está associado a uma empresa");

            if (registrarDto.EmpresaId != empresaIdUsuario)
                return Forbid("Usuário só pode registrar usuários na própria empresa");

            var resultado = await _authService.RegistrarUsuarioAsync(registrarDto);
            if (!resultado)
            {
                _logger.LogWarning("Falha ao registrar usuário com email: {Email}", registrarDto.Email);
                return BadRequest(new { message = "Falha ao registrar usuário. Verifique se o email já não está em uso." });
            }

            _logger.LogInformation("Usuário registrado com sucesso: {Email}", registrarDto.Email);
            return Ok(new { message = "Usuário registrado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário: {Email}", registrarDto.Email);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Realiza logout do usuário atual
    /// </summary>
    /// <returns>Resultado da operação</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return BadRequest(new { message = "Usuário não encontrado" });

            var resultado = await _authService.LogoutAsync(userId);
            if (!resultado)
                return BadRequest(new { message = "Falha ao realizar logout" });

            _logger.LogInformation("Logout realizado com sucesso para usuário: {UserId}", userId);
            return Ok(new { message = "Logout realizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar logout");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém dados do usuário logado
    /// </summary>
    /// <returns>Dados do usuário atual</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetUsuarioLogado()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return BadRequest(new { message = "Usuário não encontrado" });

            var usuario = await _authService.GetUsuarioLogadoAsync(userId);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado" });

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dados do usuário logado");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Altera a senha do usuário atual
    /// </summary>
    /// <param name="alterarSenhaDto">Dados para alteração de senha</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("alterar-senha")]
    [Authorize]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDto alterarSenhaDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return BadRequest(new { message = "Usuário não encontrado" });

            var resultado = await _authService.AlterarSenhaAsync(userId, alterarSenhaDto.SenhaAtual, alterarSenhaDto.NovaSenha);
            if (!resultado)
            {
                _logger.LogWarning("Falha ao alterar senha para usuário: {UserId}", userId);
                return BadRequest(new { message = "Falha ao alterar senha. Verifique se a senha atual está correta." });
            }

            _logger.LogInformation("Senha alterada com sucesso para usuário: {UserId}", userId);
            return Ok(new { message = "Senha alterada com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar senha");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém módulos liberados para a empresa do usuário
    /// </summary>
    /// <returns>Lista de módulos liberados</returns>
    [HttpGet("modulos-liberados")]
    [Authorize]
    public async Task<IActionResult> GetModulosLiberados()
    {
        try
        {
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            if (empresaIdClaim == null || !Guid.TryParse(empresaIdClaim, out var empresaId))
                return BadRequest(new { message = "Empresa não encontrada" });

            var modulos = await _authService.GetModulosLiberadosAsync(empresaId);
            return Ok(modulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter módulos liberados");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}

public class AlterarSenhaDto
{
    [Required(ErrorMessage = "Senha atual é obrigatória")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Nova senha deve ter no mínimo 6 caracteres")]
    public string NovaSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmação de nova senha é obrigatória")]
    [Compare("NovaSenha", ErrorMessage = "Nova senha e confirmação devem ser iguais")]
    public string ConfirmarNovaSenha { get; set; } = string.Empty;
}