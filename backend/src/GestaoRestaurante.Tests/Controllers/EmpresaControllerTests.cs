using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GestaoRestaurante.API.Controllers;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Features.Empresas.Queries.GetAllEmpresas;
using GestaoRestaurante.Application.Features.Empresas.Queries.GetEmpresaById;
using GestaoRestaurante.Application.Features.Empresas.Commands.CreateEmpresa;
using GestaoRestaurante.Application.Features.Empresas.Commands.UpdateEmpresa;
using GestaoRestaurante.Application.Features.Empresas.Commands.DeleteEmpresa;
using GestaoRestaurante.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GestaoRestaurante.Tests.Controllers
{
    public class EmpresaControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<EmpresaController>> _loggerMock;
        private readonly EmpresaController _controller;

        public EmpresaControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<EmpresaController>>();
            _controller = new EmpresaController(_mediatorMock.Object, _loggerMock.Object);
        }

        #region GetEmpresas Tests

        [Fact]
        public async Task GetEmpresas_DeveRetornarOk_QuandoExistemEmpresas()
        {
            // Arrange
            var empresas = new List<EmpresaDto>
            {
                new() { Id = Guid.NewGuid(), RazaoSocial = "Empresa 1", NomeFantasia = "Fantasia 1" },
                new() { Id = Guid.NewGuid(), RazaoSocial = "Empresa 2", NomeFantasia = "Fantasia 2" }
            };

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllEmpresasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<EmpresaDto>>.Success(empresas));

            // Act
            var result = await _controller.GetEmpresas();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var data = okResult.Value.Should().BeAssignableTo<IEnumerable<EmpresaDto>>().Subject;
            data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetEmpresas_DeveRetornarNoContent_QuandoNaoExistemEmpresas()
        {
            // Arrange
            var empresas = new List<EmpresaDto>();

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllEmpresasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<EmpresaDto>>.Success(empresas));

            // Act
            var result = await _controller.GetEmpresas();

            // Assert
            result.Result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetEmpresas_DeveRetornarServerError_QuandoOcorreErro()
        {
            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllEmpresasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<EmpresaDto>>.Failure("Erro interno"));

            // Act
            var result = await _controller.GetEmpresas();

            // Assert
            var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region GetEmpresa Tests

        [Fact]
        public async Task GetEmpresa_DeveRetornarOk_QuandoEmpresaExiste()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var empresa = new EmpresaDto { Id = empresaId, RazaoSocial = "Empresa Teste", NomeFantasia = "Teste" };

            _mediatorMock.Setup(x => x.Send(It.Is<GetEmpresaByIdQuery>(q => q.Id == empresaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmpresaDto>.Success(empresa));

            // Act
            var result = await _controller.GetEmpresa(empresaId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var data = okResult.Value.Should().BeOfType<EmpresaDto>().Subject;
            data.Id.Should().Be(empresaId);
        }

        [Fact]
        public async Task GetEmpresa_DeveRetornarNotFound_QuandoEmpresaNaoExiste()
        {
            // Arrange
            var empresaId = Guid.NewGuid();

            _mediatorMock.Setup(x => x.Send(It.Is<GetEmpresaByIdQuery>(q => q.Id == empresaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmpresaDto>.Failure("Empresa não encontrada"));

            // Act
            var result = await _controller.GetEmpresa(empresaId);

            // Assert
            var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        #endregion
    }
}