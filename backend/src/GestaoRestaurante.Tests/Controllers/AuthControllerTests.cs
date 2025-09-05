using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using GestaoRestaurante.API.Controllers;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GestaoRestaurante.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        #region Login Tests

        [Fact]
        public async Task Login_DeveRetornarOk_QuandoCredenciaisValidas()
        {
            // Arrange
            var loginDto = new LoginDto 
            { 
                Email = "teste@empresa.com",
                Senha = "123456"
            };

            var loginResponse = new LoginResponseDto
            {
                Token = "jwt-token-mock",
                Usuario = new UsuarioLogadoDto { Id = Guid.NewGuid(), Email = "teste@empresa.com", Nome = "Usuario Teste" }
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var data = okResult.Value.Should().BeOfType<LoginResponseDto>().Subject;
            data.Token.Should().Be("jwt-token-mock");
            data.Usuario.Email.Should().Be("teste@empresa.com");
        }

        [Fact]
        public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisInvalidas()
        {
            // Arrange
            var loginDto = new LoginDto 
            { 
                Email = "teste@empresa.com",
                Senha = "senha-errada"
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync((LoginResponseDto?)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Login_DeveRetornarBadRequest_QuandoModelStateInvalido()
        {
            // Arrange
            var loginDto = new LoginDto();
            _controller.ModelState.AddModelError("Email", "Email é obrigatório");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Login_DeveRetornarInternalServerError_QuandoOcorreExcecao()
        {
            // Arrange
            var loginDto = new LoginDto 
            { 
                Email = "teste@empresa.com",
                Senha = "123456"
            };

            _authServiceMock.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new Exception("Erro de conexão"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region Registrar Tests

        [Fact]
        public async Task Registrar_DeveRetornarOk_QuandoDadosValidos()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var registrarDto = new RegistrarUsuarioDto 
            { 
                Email = "novo@empresa.com",
                Nome = "Novo Usuário",
                EmpresaId = empresaId,
                Senha = "123456"
            };

            SetupAuthenticatedUser(empresaId);

            _authServiceMock.Setup(x => x.RegistrarUsuarioAsync(registrarDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Registrar(registrarDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Registrar_DeveRetornarForbid_QuandoUsuarioNaoTemEmpresa()
        {
            // Arrange
            var registrarDto = new RegistrarUsuarioDto();
            SetupUserWithoutCompany();

            // Act
            var result = await _controller.Registrar(registrarDto);

            // Assert
            var forbidResult = result.Should().BeOfType<ForbidResult>().Subject;
        }

        [Fact]
        public async Task Registrar_DeveRetornarForbid_QuandoEmpresaDiferente()
        {
            // Arrange
            var empresaUsuario = Guid.NewGuid();
            var empresaDto = Guid.NewGuid();
            var registrarDto = new RegistrarUsuarioDto 
            { 
                EmpresaId = empresaDto
            };

            SetupAuthenticatedUser(empresaUsuario);

            // Act
            var result = await _controller.Registrar(registrarDto);

            // Assert
            var forbidResult = result.Should().BeOfType<ForbidResult>().Subject;
        }

        [Fact]
        public async Task Registrar_DeveRetornarBadRequest_QuandoServicoFalha()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var registrarDto = new RegistrarUsuarioDto 
            { 
                EmpresaId = empresaId
            };

            SetupAuthenticatedUser(empresaId);

            _authServiceMock.Setup(x => x.RegistrarUsuarioAsync(registrarDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Registrar(registrarDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_DeveRetornarOk_QuandoLogoutComSucesso()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            SetupAuthenticatedUserWithId(userId);

            _authServiceMock.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Logout_DeveRetornarBadRequest_QuandoUsuarioNaoEncontrado()
        {
            // Arrange
            SetupUserWithoutId();

            // Act
            var result = await _controller.Logout();

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Logout_DeveRetornarBadRequest_QuandoServicoFalha()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            SetupAuthenticatedUserWithId(userId);

            _authServiceMock.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Logout();

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        #endregion

        #region GetUsuarioLogado Tests

        [Fact]
        public async Task GetUsuarioLogado_DeveRetornarOk_QuandoUsuarioExiste()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var usuarioDto = new UsuarioDto { Id = Guid.Parse(userId), Email = "teste@empresa.com" };

            SetupAuthenticatedUserWithId(userId);

            _authServiceMock.Setup(x => x.GetUsuarioLogadoAsync(userId))
                .ReturnsAsync(new UsuarioLogadoDto { Id = Guid.Parse(userId), Email = "teste@empresa.com", Nome = "Usuario Teste" });

            // Act
            var result = await _controller.GetUsuarioLogado();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var data = okResult.Value.Should().BeOfType<UsuarioLogadoDto>().Subject;
            data.Email.Should().Be("teste@empresa.com");
        }

        [Fact]
        public async Task GetUsuarioLogado_DeveRetornarBadRequest_QuandoUsuarioNaoEncontrado()
        {
            // Arrange
            SetupUserWithoutId();

            // Act
            var result = await _controller.GetUsuarioLogado();

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task GetUsuarioLogado_DeveRetornarNotFound_QuandoServicoRetornaNulo()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            SetupAuthenticatedUserWithId(userId);

            _authServiceMock.Setup(x => x.GetUsuarioLogadoAsync(userId))
                .ReturnsAsync((UsuarioLogadoDto?)null);

            // Act
            var result = await _controller.GetUsuarioLogado();

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        #endregion

        #region AlterarSenha Tests

        [Fact]
        public async Task AlterarSenha_DeveRetornarOk_QuandoAlteracaoComSucesso()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var alterarSenhaDto = new AlterarSenhaDto 
            { 
                SenhaAtual = "senha123",
                NovaSenha = "novaSenha123",
                ConfirmarNovaSenha = "novaSenha123"
            };

            SetupAuthenticatedUserWithId(userId);

            _authServiceMock.Setup(x => x.AlterarSenhaAsync(userId, alterarSenhaDto.SenhaAtual, alterarSenhaDto.NovaSenha))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AlterarSenha(alterarSenhaDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task AlterarSenha_DeveRetornarBadRequest_QuandoModelStateInvalido()
        {
            // Arrange
            var alterarSenhaDto = new AlterarSenhaDto();
            _controller.ModelState.AddModelError("SenhaAtual", "Senha atual é obrigatória");

            // Act
            var result = await _controller.AlterarSenha(alterarSenhaDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task AlterarSenha_DeveRetornarBadRequest_QuandoServicoFalha()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var alterarSenhaDto = new AlterarSenhaDto 
            { 
                SenhaAtual = "senha123",
                NovaSenha = "novaSenha123"
            };

            SetupAuthenticatedUserWithId(userId);

            _authServiceMock.Setup(x => x.AlterarSenhaAsync(userId, alterarSenhaDto.SenhaAtual, alterarSenhaDto.NovaSenha))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AlterarSenha(alterarSenhaDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        #endregion

        #region GetModulosLiberados Tests

        [Fact]
        public async Task GetModulosLiberados_DeveRetornarOk_ComListaDeModulos()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var modulos = new List<string> { "EMPRESAS", "FILIAIS", "PRODUTOS" };

            SetupAuthenticatedUser(empresaId);

            _authServiceMock.Setup(x => x.GetModulosLiberadosAsync(empresaId))
                .ReturnsAsync(modulos);

            // Act
            var result = await _controller.GetModulosLiberados();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var data = okResult.Value.Should().BeAssignableTo<IEnumerable<string>>().Subject;
            data.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetModulosLiberados_DeveRetornarBadRequest_QuandoEmpresaNaoEncontrada()
        {
            // Arrange
            SetupUserWithoutCompany();

            // Act
            var result = await _controller.GetModulosLiberados();

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        #endregion

        #region Helper Methods

        private void SetupAuthenticatedUser(Guid empresaId)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new("EmpresaId", empresaId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        private void SetupAuthenticatedUserWithId(string userId)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new("EmpresaId", Guid.NewGuid().ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        private void SetupUserWithoutCompany()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        private void SetupUserWithoutId()
        {
            var claims = new List<Claim>
            {
                new("EmpresaId", Guid.NewGuid().ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #endregion
    }
}