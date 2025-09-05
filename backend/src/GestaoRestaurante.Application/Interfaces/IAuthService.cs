using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<bool> RegistrarUsuarioAsync(RegistrarUsuarioDto registrarDto);
    Task<bool> LogoutAsync(string userId);
    Task<UsuarioLogadoDto?> GetUsuarioLogadoAsync(string userId);
    Task<bool> AlterarSenhaAsync(string userId, string senhaAtual, string novaSenha);
    Task<List<string>> GetModulosLiberadosAsync(Guid empresaId);
}