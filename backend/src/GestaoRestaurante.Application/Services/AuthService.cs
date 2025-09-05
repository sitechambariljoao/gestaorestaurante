using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoRestaurante.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<Usuario> _userManager;
    private readonly GestaoRestauranteContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<Usuario> userManager,
        GestaoRestauranteContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var usuario = await _userManager.FindByEmailAsync(loginDto.Email);
        if (usuario == null)
            return null;

        // Verificar senha usando UserManager
        var senhaValida = await _userManager.CheckPasswordAsync(usuario, loginDto.Senha);
        if (!senhaValida)
            return null;

        // Atualizar último login
        usuario.UltimoLogin = DateTime.UtcNow;
        await _userManager.UpdateAsync(usuario);

        // Gerar token JWT
        var token = await GerarTokenJwt(usuario);
        var expiracao = DateTime.UtcNow.AddHours(8);

        // Buscar dados adicionais do usuário
        var usuarioCompleto = await GetUsuarioCompletoAsync(usuario.Id);

        return new LoginResponseDto
        {
            Token = token,
            Expiracao = expiracao,
            Usuario = usuarioCompleto!
        };
    }

    public async Task<bool> RegistrarUsuarioAsync(RegistrarUsuarioDto registrarDto)
    {
        // Verificar se empresa existe e está ativa
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(e => e.Id == registrarDto.EmpresaId && e.Ativa);
        if (empresa == null)
            return false;

        // Verificar se email já existe
        var usuarioExistente = await _userManager.FindByEmailAsync(registrarDto.Email);
        if (usuarioExistente != null)
            return false;

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            UserName = registrarDto.Email,
            Email = registrarDto.Email,
            Nome = registrarDto.Nome,
            EmpresaId = registrarDto.EmpresaId,
            Cpf = registrarDto.Cpf,
            Perfil = registrarDto.Perfil,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            EmailConfirmed = true // Para simplificar, vamos considerar emails confirmados automaticamente
        };

        var resultado = await _userManager.CreateAsync(usuario, registrarDto.Senha);
        if (!resultado.Succeeded)
            return false;

        // Vincular usuário às filiais
        if (registrarDto.FiliaisAcesso.Any())
        {
            var usuarioFiliais = registrarDto.FiliaisAcesso.Select(filialId => new UsuarioFilial
            {
                UsuarioId = usuario.Id,
                FilialId = filialId,
                DataVinculo = DateTime.UtcNow,
                Ativo = true
            }).ToList();

            _context.UsuarioFiliais.AddRange(usuarioFiliais);
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        if (Guid.TryParse(userId, out var guidUserId))
        {
            var usuario = await _userManager.FindByIdAsync(guidUserId.ToString());
            if (usuario != null)
            {
                // Para JWT, o logout é tratado no cliente removendo o token
                // Aqui podemos registrar o logout ou invalidar tokens se necessário
                return true;
            }
        }
        return false;
    }

    public async Task<UsuarioLogadoDto?> GetUsuarioLogadoAsync(string userId)
    {
        if (Guid.TryParse(userId, out var guidUserId))
        {
            return await GetUsuarioCompletoAsync(guidUserId);
        }
        return null;
    }

    public async Task<bool> AlterarSenhaAsync(string userId, string senhaAtual, string novaSenha)
    {
        if (Guid.TryParse(userId, out var guidUserId))
        {
            var usuario = await _userManager.FindByIdAsync(guidUserId.ToString());
            if (usuario != null)
            {
                var resultado = await _userManager.ChangePasswordAsync(usuario, senhaAtual, novaSenha);
                return resultado.Succeeded;
            }
        }
        return false;
    }

    public async Task<List<string>> GetModulosLiberadosAsync(Guid empresaId)
    {
        var assinatura = await _context.AssinaturasEmpresa
            .Include(a => a.Plano)
            .ThenInclude(p => p.Modulos)
            .FirstOrDefaultAsync(a => a.EmpresaId == empresaId && a.Ativa && a.DataVencimento > DateTime.UtcNow);

        if (assinatura == null)
            return new List<string>();

        return assinatura.Plano.Modulos
            .Where(m => m.Liberado)
            .Select(m => m.NomeModulo)
            .ToList();
    }

    private async Task<string> GerarTokenJwt(Usuario usuario)
    {
        var modulosLiberados = await GetModulosLiberadosAsync(usuario.EmpresaId);
        var filiaisAcesso = await _context.UsuarioFiliais
            .Where(uf => uf.UsuarioId == usuario.Id && uf.Ativo)
            .Select(uf => uf.FilialId.ToString())
            .ToListAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email ?? string.Empty),
            new("Perfil", usuario.Perfil),
            new("EmpresaId", usuario.EmpresaId.ToString()),
            new("Ativo", usuario.Ativo.ToString())
        };

        // Adicionar módulos liberados como claims
        foreach (var modulo in modulosLiberados)
        {
            claims.Add(new Claim("Modulo", modulo));
        }

        // Adicionar filiais de acesso como claims
        foreach (var filialId in filiaisAcesso)
        {
            claims.Add(new Claim("FilialAcesso", filialId));
        }

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"] ?? "MinhaChaveSecretaSuperSegura123456789"));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<UsuarioLogadoDto?> GetUsuarioCompletoAsync(Guid userId)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Empresa)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (usuario == null)
            return null;

        var modulosLiberados = await GetModulosLiberadosAsync(usuario.EmpresaId);
        var filiaisAcesso = await _context.UsuarioFiliais
            .Where(uf => uf.UsuarioId == userId && uf.Ativo)
            .Select(uf => uf.FilialId)
            .ToListAsync();

        return new UsuarioLogadoDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email ?? string.Empty,
            Perfil = usuario.Perfil,
            EmpresaId = usuario.EmpresaId,
            EmpresaNome = usuario.Empresa.NomeFantasia,
            ModulosLiberados = modulosLiberados,
            FiliaisAcesso = filiaisAcesso
        };
    }
}