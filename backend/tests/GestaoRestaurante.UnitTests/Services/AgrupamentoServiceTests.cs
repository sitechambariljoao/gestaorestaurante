using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Services;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using Moq;
using Xunit;

namespace GestaoRestaurante.UnitTests.Services
{
    public class AgrupamentoServiceTests
    {
        private readonly Mock<IAgrupamentoRepository> _agrupamentoRepositoryMock;
        private readonly Mock<IEmpresaRepository> _empresaRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AgrupamentoService _agrupamentoService;

        public AgrupamentoServiceTests()
        {
            _agrupamentoRepositoryMock = new Mock<IAgrupamentoRepository>();
            _empresaRepositoryMock = new Mock<IEmpresaRepository>();
            _mapperMock = new Mock<IMapper>();
            _agrupamentoService = new AgrupamentoService(
                _agrupamentoRepositoryMock.Object,
                _empresaRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodosAgrupamentos()
        {
            // Arrange
            var agrupamentos = new List<Agrupamento>
            {
                new Agrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Agrupamento 1", 
                    Codigo = "AGR001",
                    Ativo = true 
                },
                new Agrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Agrupamento 2", 
                    Codigo = "AGR002",
                    Ativo = true 
                }
            };

            var agrupamentoDtos = new List<AgrupamentoDto>
            {
                new AgrupamentoDto 
                { 
                    Id = agrupamentos[0].Id, 
                    Nome = "Agrupamento 1", 
                    Codigo = "AGR001",
                    Ativo = true 
                },
                new AgrupamentoDto 
                { 
                    Id = agrupamentos[1].Id, 
                    Nome = "Agrupamento 2", 
                    Codigo = "AGR002",
                    Ativo = true 
                }
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(agrupamentos);
            _mapperMock.Setup(x => x.Map<IEnumerable<AgrupamentoDto>>(agrupamentos))
                .Returns(agrupamentoDtos);

            // Act
            var resultado = await _agrupamentoService.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(agrupamentoDtos);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarAgrupamento()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var agrupamento = new Agrupamento 
            { 
                Id = agrupamentoId, 
                Nome = "Agrupamento Teste", 
                Codigo = "AGR001",
                Ativo = true 
            };
            var agrupamentoDto = new AgrupamentoDto 
            { 
                Id = agrupamentoId, 
                Nome = "Agrupamento Teste", 
                Codigo = "AGR001",
                Ativo = true 
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync(agrupamento);
            _mapperMock.Setup(x => x.Map<AgrupamentoDto>(agrupamento))
                .Returns(agrupamentoDto);

            // Act
            var resultado = await _agrupamentoService.GetByIdAsync(agrupamentoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(agrupamentoDto);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync((Agrupamento)null);

            // Act
            var resultado = await _agrupamentoService.GetByIdAsync(agrupamentoId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarAgrupamento()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var empresa = new Empresa 
            { 
                Id = empresaId, 
                RazaoSocial = "Empresa Teste", 
                Ativa = true 
            };

            var agrupamentoDto = new AgrupamentoDto
            {
                Nome = "Novo Agrupamento",
                Codigo = "AGR001",
                EmpresaId = empresaId,
                Descricao = "Descrição do agrupamento",
                Ativo = true
            };

            var agrupamento = new Agrupamento
            {
                Id = Guid.NewGuid(),
                Nome = "Novo Agrupamento",
                Codigo = "AGR001",
                EmpresaId = empresaId,
                Descricao = "Descrição do agrupamento",
                Ativo = true
            };

            var agrupamentoCriado = new Agrupamento
            {
                Id = agrupamento.Id,
                Nome = "Novo Agrupamento",
                Codigo = "AGR001",
                EmpresaId = empresaId,
                Descricao = "Descrição do agrupamento",
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _empresaRepositoryMock.Setup(x => x.GetByIdAsync(empresaId))
                .ReturnsAsync(empresa);
            _mapperMock.Setup(x => x.Map<Agrupamento>(agrupamentoDto))
                .Returns(agrupamento);
            _agrupamentoRepositoryMock.Setup(x => x.AddAsync(agrupamento))
                .ReturnsAsync(agrupamentoCriado);
            _mapperMock.Setup(x => x.Map<AgrupamentoDto>(agrupamentoCriado))
                .Returns(new AgrupamentoDto 
                { 
                    Id = agrupamentoCriado.Id,
                    Nome = agrupamentoCriado.Nome,
                    Codigo = agrupamentoCriado.Codigo,
                    EmpresaId = agrupamentoCriado.EmpresaId,
                    Descricao = agrupamentoCriado.Descricao,
                    Ativo = agrupamentoCriado.Ativo
                });

            // Act
            var resultado = await _agrupamentoService.CreateAsync(agrupamentoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(agrupamentoCriado.Id);
            resultado.Nome.Should().Be("Novo Agrupamento");
            resultado.Codigo.Should().Be("AGR001");
            _agrupamentoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Agrupamento>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ComEmpresaInexistente_DeveLancarExcecao()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var agrupamentoDto = new AgrupamentoDto
            {
                Nome = "Novo Agrupamento",
                EmpresaId = empresaId
            };

            _empresaRepositoryMock.Setup(x => x.GetByIdAsync(empresaId))
                .ReturnsAsync((Empresa)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _agrupamentoService.CreateAsync(agrupamentoDto));
            
            exception.Message.Should().Contain("Empresa não encontrada");
        }

        [Fact]
        public async Task CreateAsync_ComEmpresaInativa_DeveLancarExcecao()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var empresa = new Empresa 
            { 
                Id = empresaId, 
                RazaoSocial = "Empresa Inativa", 
                Ativa = false 
            };

            var agrupamentoDto = new AgrupamentoDto
            {
                Nome = "Novo Agrupamento",
                EmpresaId = empresaId
            };

            _empresaRepositoryMock.Setup(x => x.GetByIdAsync(empresaId))
                .ReturnsAsync(empresa);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _agrupamentoService.CreateAsync(agrupamentoDto));
            
            exception.Message.Should().Contain("Empresa deve estar ativa");
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarAgrupamento()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var empresaId = Guid.NewGuid();
            
            var agrupamentoExistente = new Agrupamento
            {
                Id = agrupamentoId,
                Nome = "Agrupamento Antigo",
                EmpresaId = empresaId,
                Ativo = true
            };

            var empresa = new Empresa 
            { 
                Id = empresaId, 
                RazaoSocial = "Empresa Teste", 
                Ativa = true 
            };

            var agrupamentoDto = new AgrupamentoDto
            {
                Id = agrupamentoId,
                Nome = "Agrupamento Atualizado",
                EmpresaId = empresaId,
                Ativo = true
            };

            var agrupamentoAtualizado = new Agrupamento
            {
                Id = agrupamentoId,
                Nome = "Agrupamento Atualizado",
                EmpresaId = empresaId,
                Ativo = true
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync(agrupamentoExistente);
            _empresaRepositoryMock.Setup(x => x.GetByIdAsync(empresaId))
                .ReturnsAsync(empresa);
            _agrupamentoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Agrupamento>()))
                .ReturnsAsync(agrupamentoAtualizado);
            _mapperMock.Setup(x => x.Map<AgrupamentoDto>(agrupamentoAtualizado))
                .Returns(agrupamentoDto);

            // Act
            var resultado = await _agrupamentoService.UpdateAsync(agrupamentoId, agrupamentoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Agrupamento Atualizado");
            _agrupamentoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Agrupamento>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComAgrupamentoInexistente_DeveRetornarNull()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var agrupamentoDto = new AgrupamentoDto { Id = agrupamentoId, Nome = "Teste" };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync((Agrupamento)null);

            // Act
            var resultado = await _agrupamentoService.UpdateAsync(agrupamentoId, agrupamentoDto);

            // Assert
            resultado.Should().BeNull();
            _agrupamentoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Agrupamento>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ComAgrupamentoExistente_DeveExcluirAgrupamento()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var agrupamento = new Agrupamento 
            { 
                Id = agrupamentoId, 
                Nome = "Agrupamento para Excluir", 
                Ativo = true 
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync(agrupamento);
            _agrupamentoRepositoryMock.Setup(x => x.DeleteAsync(agrupamentoId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _agrupamentoService.DeleteAsync(agrupamentoId);

            // Assert
            resultado.Should().BeTrue();
            _agrupamentoRepositoryMock.Verify(x => x.DeleteAsync(agrupamentoId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ComAgrupamentoInexistente_DeveRetornarFalse()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync((Agrupamento)null);

            // Act
            var resultado = await _agrupamentoService.DeleteAsync(agrupamentoId);

            // Assert
            resultado.Should().BeFalse();
            _agrupamentoRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetByEmpresaAsync_DeveRetornarAgrupamentosPorEmpresa()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var agrupamentos = new List<Agrupamento>
            {
                new Agrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Agrupamento A", 
                    EmpresaId = empresaId, 
                    Ativo = true 
                },
                new Agrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Agrupamento B", 
                    EmpresaId = empresaId, 
                    Ativo = true 
                }
            };

            var agrupamentoDtos = new List<AgrupamentoDto>
            {
                new AgrupamentoDto 
                { 
                    Id = agrupamentos[0].Id, 
                    Nome = "Agrupamento A", 
                    EmpresaId = empresaId, 
                    Ativo = true 
                },
                new AgrupamentoDto 
                { 
                    Id = agrupamentos[1].Id, 
                    Nome = "Agrupamento B", 
                    EmpresaId = empresaId, 
                    Ativo = true 
                }
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByEmpresaAsync(empresaId))
                .ReturnsAsync(agrupamentos);
            _mapperMock.Setup(x => x.Map<IEnumerable<AgrupamentoDto>>(agrupamentos))
                .Returns(agrupamentoDtos);

            // Act
            var resultado = await _agrupamentoService.GetByEmpresaAsync(empresaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(a => a.EmpresaId == empresaId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var empresa = new Empresa 
            { 
                Id = empresaId, 
                RazaoSocial = "Empresa Teste", 
                Ativa = true 
            };

            var agrupamentoDto = new AgrupamentoDto
            {
                Nome = nomeInvalido,
                EmpresaId = empresaId
            };

            _empresaRepositoryMock.Setup(x => x.GetByIdAsync(empresaId))
                .ReturnsAsync(empresa);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _agrupamentoService.CreateAsync(agrupamentoDto));
            
            exception.Message.Should().Contain("Nome é obrigatório");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_ComCodigoInvalido_DeveLancarExcecao(string codigoInvalido)
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var empresa = new Empresa 
            { 
                Id = empresaId, 
                RazaoSocial = "Empresa Teste", 
                Ativa = true 
            };

            var agrupamentoDto = new AgrupamentoDto
            {
                Nome = "Agrupamento Teste",
                Codigo = codigoInvalido,
                EmpresaId = empresaId
            };

            _empresaRepositoryMock.Setup(x => x.GetByIdAsync(empresaId))
                .ReturnsAsync(empresa);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _agrupamentoService.CreateAsync(agrupamentoDto));
            
            exception.Message.Should().Contain("Código é obrigatório");
        }

        [Fact]
        public async Task GetByCodigoAsync_ComCodigoValido_DeveRetornarAgrupamento()
        {
            // Arrange
            var codigo = "AGR001";
            var empresaId = Guid.NewGuid();
            var agrupamento = new Agrupamento 
            { 
                Id = Guid.NewGuid(), 
                Codigo = codigo,
                Nome = "Agrupamento Teste", 
                EmpresaId = empresaId,
                Ativo = true 
            };
            var agrupamentoDto = new AgrupamentoDto 
            { 
                Id = agrupamento.Id, 
                Codigo = codigo,
                Nome = "Agrupamento Teste", 
                EmpresaId = empresaId,
                Ativo = true 
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByCodigoAsync(codigo, empresaId))
                .ReturnsAsync(agrupamento);
            _mapperMock.Setup(x => x.Map<AgrupamentoDto>(agrupamento))
                .Returns(agrupamentoDto);

            // Act
            var resultado = await _agrupamentoService.GetByCodigoAsync(codigo, empresaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Codigo.Should().Be(codigo);
            resultado.EmpresaId.Should().Be(empresaId);
        }

        [Fact]
        public async Task GetByCodigoAsync_ComCodigoInexistente_DeveRetornarNull()
        {
            // Arrange
            var codigo = "AGR999";
            var empresaId = Guid.NewGuid();

            _agrupamentoRepositoryMock.Setup(x => x.GetByCodigoAsync(codigo, empresaId))
                .ReturnsAsync((Agrupamento)null);

            // Act
            var resultado = await _agrupamentoService.GetByCodigoAsync(codigo, empresaId);

            // Assert
            resultado.Should().BeNull();
        }
    }
}