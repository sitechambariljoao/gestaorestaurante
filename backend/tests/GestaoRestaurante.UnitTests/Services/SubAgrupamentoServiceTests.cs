using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SubAgrupamentoServiceTests
    {
        private readonly Mock<ISubAgrupamentoRepository> _subAgrupamentoRepositoryMock;
        private readonly Mock<IAgrupamentoRepository> _agrupamentoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SubAgrupamentoService _subAgrupamentoService;

        public SubAgrupamentoServiceTests()
        {
            _subAgrupamentoRepositoryMock = new Mock<ISubAgrupamentoRepository>();
            _agrupamentoRepositoryMock = new Mock<IAgrupamentoRepository>();
            _mapperMock = new Mock<IMapper>();
            _subAgrupamentoService = new SubAgrupamentoService(
                _subAgrupamentoRepositoryMock.Object,
                _agrupamentoRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodosSubAgrupamentos()
        {
            // Arrange
            var subAgrupamentos = new List<SubAgrupamento>
            {
                new SubAgrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "SubAgrupamento 1", 
                    Codigo = "SUB001",
                    Ativo = true 
                },
                new SubAgrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "SubAgrupamento 2", 
                    Codigo = "SUB002",
                    Ativo = true 
                }
            };

            var subAgrupamentoDtos = new List<SubAgrupamentoDto>
            {
                new SubAgrupamentoDto 
                { 
                    Id = subAgrupamentos[0].Id, 
                    Nome = "SubAgrupamento 1", 
                    Codigo = "SUB001",
                    Ativo = true 
                },
                new SubAgrupamentoDto 
                { 
                    Id = subAgrupamentos[1].Id, 
                    Nome = "SubAgrupamento 2", 
                    Codigo = "SUB002",
                    Ativo = true 
                }
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(subAgrupamentos);
            _mapperMock.Setup(x => x.Map<IEnumerable<SubAgrupamentoDto>>(subAgrupamentos))
                .Returns(subAgrupamentoDtos);

            // Act
            var resultado = await _subAgrupamentoService.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(subAgrupamentoDtos);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarSubAgrupamento()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var subAgrupamento = new SubAgrupamento 
            { 
                Id = subAgrupamentoId, 
                Nome = "SubAgrupamento Teste", 
                Codigo = "SUB001",
                Ativo = true 
            };
            var subAgrupamentoDto = new SubAgrupamentoDto 
            { 
                Id = subAgrupamentoId, 
                Nome = "SubAgrupamento Teste", 
                Codigo = "SUB001",
                Ativo = true 
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync(subAgrupamento);
            _mapperMock.Setup(x => x.Map<SubAgrupamentoDto>(subAgrupamento))
                .Returns(subAgrupamentoDto);

            // Act
            var resultado = await _subAgrupamentoService.GetByIdAsync(subAgrupamentoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(subAgrupamentoDto);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync((SubAgrupamento)null);

            // Act
            var resultado = await _subAgrupamentoService.GetByIdAsync(subAgrupamentoId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarSubAgrupamento()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var agrupamento = new Agrupamento 
            { 
                Id = agrupamentoId, 
                Nome = "Agrupamento Teste", 
                Ativo = true 
            };

            var subAgrupamentoDto = new SubAgrupamentoDto
            {
                Nome = "Novo SubAgrupamento",
                Codigo = "SUB001",
                AgrupamentoId = agrupamentoId,
                Descricao = "Descrição do sub-agrupamento",
                Ativo = true
            };

            var subAgrupamento = new SubAgrupamento
            {
                Id = Guid.NewGuid(),
                Nome = "Novo SubAgrupamento",
                Codigo = "SUB001",
                AgrupamentoId = agrupamentoId,
                Descricao = "Descrição do sub-agrupamento",
                Ativo = true
            };

            var subAgrupamentoCriado = new SubAgrupamento
            {
                Id = subAgrupamento.Id,
                Nome = "Novo SubAgrupamento",
                Codigo = "SUB001",
                AgrupamentoId = agrupamentoId,
                Descricao = "Descrição do sub-agrupamento",
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync(agrupamento);
            _mapperMock.Setup(x => x.Map<SubAgrupamento>(subAgrupamentoDto))
                .Returns(subAgrupamento);
            _subAgrupamentoRepositoryMock.Setup(x => x.AddAsync(subAgrupamento))
                .ReturnsAsync(subAgrupamentoCriado);
            _mapperMock.Setup(x => x.Map<SubAgrupamentoDto>(subAgrupamentoCriado))
                .Returns(new SubAgrupamentoDto 
                { 
                    Id = subAgrupamentoCriado.Id,
                    Nome = subAgrupamentoCriado.Nome,
                    Codigo = subAgrupamentoCriado.Codigo,
                    AgrupamentoId = subAgrupamentoCriado.AgrupamentoId,
                    Descricao = subAgrupamentoCriado.Descricao,
                    Ativo = subAgrupamentoCriado.Ativo
                });

            // Act
            var resultado = await _subAgrupamentoService.CreateAsync(subAgrupamentoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(subAgrupamentoCriado.Id);
            resultado.Nome.Should().Be("Novo SubAgrupamento");
            resultado.Codigo.Should().Be("SUB001");
            _subAgrupamentoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<SubAgrupamento>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ComAgrupamentoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var subAgrupamentoDto = new SubAgrupamentoDto
            {
                Nome = "Novo SubAgrupamento",
                AgrupamentoId = agrupamentoId
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync((Agrupamento)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _subAgrupamentoService.CreateAsync(subAgrupamentoDto));
            
            exception.Message.Should().Contain("Agrupamento não encontrado");
        }

        [Fact]
        public async Task CreateAsync_ComAgrupamentoInativo_DeveLancarExcecao()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var agrupamento = new Agrupamento 
            { 
                Id = agrupamentoId, 
                Nome = "Agrupamento Inativo", 
                Ativo = false 
            };

            var subAgrupamentoDto = new SubAgrupamentoDto
            {
                Nome = "Novo SubAgrupamento",
                AgrupamentoId = agrupamentoId
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync(agrupamento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _subAgrupamentoService.CreateAsync(subAgrupamentoDto));
            
            exception.Message.Should().Contain("Agrupamento deve estar ativo");
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarSubAgrupamento()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var agrupamentoId = Guid.NewGuid();
            
            var subAgrupamentoExistente = new SubAgrupamento
            {
                Id = subAgrupamentoId,
                Nome = "SubAgrupamento Antigo",
                AgrupamentoId = agrupamentoId,
                Ativo = true
            };

            var agrupamento = new Agrupamento 
            { 
                Id = agrupamentoId, 
                Nome = "Agrupamento Teste", 
                Ativo = true 
            };

            var subAgrupamentoDto = new SubAgrupamentoDto
            {
                Id = subAgrupamentoId,
                Nome = "SubAgrupamento Atualizado",
                AgrupamentoId = agrupamentoId,
                Ativo = true
            };

            var subAgrupamentoAtualizado = new SubAgrupamento
            {
                Id = subAgrupamentoId,
                Nome = "SubAgrupamento Atualizado",
                AgrupamentoId = agrupamentoId,
                Ativo = true
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync(subAgrupamentoExistente);
            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync(agrupamento);
            _subAgrupamentoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<SubAgrupamento>()))
                .ReturnsAsync(subAgrupamentoAtualizado);
            _mapperMock.Setup(x => x.Map<SubAgrupamentoDto>(subAgrupamentoAtualizado))
                .Returns(subAgrupamentoDto);

            // Act
            var resultado = await _subAgrupamentoService.UpdateAsync(subAgrupamentoId, subAgrupamentoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("SubAgrupamento Atualizado");
            _subAgrupamentoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SubAgrupamento>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComSubAgrupamentoInexistente_DeveRetornarNull()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var subAgrupamentoDto = new SubAgrupamentoDto { Id = subAgrupamentoId, Nome = "Teste" };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync((SubAgrupamento)null);

            // Act
            var resultado = await _subAgrupamentoService.UpdateAsync(subAgrupamentoId, subAgrupamentoDto);

            // Assert
            resultado.Should().BeNull();
            _subAgrupamentoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SubAgrupamento>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ComSubAgrupamentoExistente_DeveExcluirSubAgrupamento()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var subAgrupamento = new SubAgrupamento 
            { 
                Id = subAgrupamentoId, 
                Nome = "SubAgrupamento para Excluir", 
                Ativo = true 
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync(subAgrupamento);
            _subAgrupamentoRepositoryMock.Setup(x => x.DeleteAsync(subAgrupamentoId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _subAgrupamentoService.DeleteAsync(subAgrupamentoId);

            // Assert
            resultado.Should().BeTrue();
            _subAgrupamentoRepositoryMock.Verify(x => x.DeleteAsync(subAgrupamentoId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ComSubAgrupamentoInexistente_DeveRetornarFalse()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync((SubAgrupamento)null);

            // Act
            var resultado = await _subAgrupamentoService.DeleteAsync(subAgrupamentoId);

            // Assert
            resultado.Should().BeFalse();
            _subAgrupamentoRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetByAgrupamentoAsync_DeveRetornarSubAgrupamentosPorAgrupamento()
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var subAgrupamentos = new List<SubAgrupamento>
            {
                new SubAgrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "SubAgrupamento A", 
                    AgrupamentoId = agrupamentoId, 
                    Ativo = true 
                },
                new SubAgrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "SubAgrupamento B", 
                    AgrupamentoId = agrupamentoId, 
                    Ativo = true 
                }
            };

            var subAgrupamentoDtos = new List<SubAgrupamentoDto>
            {
                new SubAgrupamentoDto 
                { 
                    Id = subAgrupamentos[0].Id, 
                    Nome = "SubAgrupamento A", 
                    AgrupamentoId = agrupamentoId, 
                    Ativo = true 
                },
                new SubAgrupamentoDto 
                { 
                    Id = subAgrupamentos[1].Id, 
                    Nome = "SubAgrupamento B", 
                    AgrupamentoId = agrupamentoId, 
                    Ativo = true 
                }
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByAgrupamentoAsync(agrupamentoId))
                .ReturnsAsync(subAgrupamentos);
            _mapperMock.Setup(x => x.Map<IEnumerable<SubAgrupamentoDto>>(subAgrupamentos))
                .Returns(subAgrupamentoDtos);

            // Act
            var resultado = await _subAgrupamentoService.GetByAgrupamentoAsync(agrupamentoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(s => s.AgrupamentoId == agrupamentoId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var agrupamento = new Agrupamento 
            { 
                Id = agrupamentoId, 
                Nome = "Agrupamento Teste", 
                Ativo = true 
            };

            var subAgrupamentoDto = new SubAgrupamentoDto
            {
                Nome = nomeInvalido,
                AgrupamentoId = agrupamentoId
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync(agrupamento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _subAgrupamentoService.CreateAsync(subAgrupamentoDto));
            
            exception.Message.Should().Contain("Nome é obrigatório");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_ComCodigoInvalido_DeveLancarExcecao(string codigoInvalido)
        {
            // Arrange
            var agrupamentoId = Guid.NewGuid();
            var agrupamento = new Agrupamento 
            { 
                Id = agrupamentoId, 
                Nome = "Agrupamento Teste", 
                Ativo = true 
            };

            var subAgrupamentoDto = new SubAgrupamentoDto
            {
                Nome = "SubAgrupamento Teste",
                Codigo = codigoInvalido,
                AgrupamentoId = agrupamentoId
            };

            _agrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(agrupamentoId))
                .ReturnsAsync(agrupamento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _subAgrupamentoService.CreateAsync(subAgrupamentoDto));
            
            exception.Message.Should().Contain("Código é obrigatório");
        }

        [Fact]
        public async Task GetByCodigoAsync_ComCodigoValido_DeveRetornarSubAgrupamento()
        {
            // Arrange
            var codigo = "SUB001";
            var agrupamentoId = Guid.NewGuid();
            var subAgrupamento = new SubAgrupamento 
            { 
                Id = Guid.NewGuid(), 
                Codigo = codigo,
                Nome = "SubAgrupamento Teste", 
                AgrupamentoId = agrupamentoId,
                Ativo = true 
            };
            var subAgrupamentoDto = new SubAgrupamentoDto 
            { 
                Id = subAgrupamento.Id, 
                Codigo = codigo,
                Nome = "SubAgrupamento Teste", 
                AgrupamentoId = agrupamentoId,
                Ativo = true 
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByCodigoAsync(codigo, agrupamentoId))
                .ReturnsAsync(subAgrupamento);
            _mapperMock.Setup(x => x.Map<SubAgrupamentoDto>(subAgrupamento))
                .Returns(subAgrupamentoDto);

            // Act
            var resultado = await _subAgrupamentoService.GetByCodigoAsync(codigo, agrupamentoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Codigo.Should().Be(codigo);
            resultado.AgrupamentoId.Should().Be(agrupamentoId);
        }

        [Fact]
        public async Task GetByCodigoAsync_ComCodigoInexistente_DeveRetornarNull()
        {
            // Arrange
            var codigo = "SUB999";
            var agrupamentoId = Guid.NewGuid();

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByCodigoAsync(codigo, agrupamentoId))
                .ReturnsAsync((SubAgrupamento)null);

            // Act
            var resultado = await _subAgrupamentoService.GetByCodigoAsync(codigo, agrupamentoId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmpresaAsync_DeveRetornarSubAgrupamentosPorEmpresa()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var subAgrupamentos = new List<SubAgrupamento>
            {
                new SubAgrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "SubAgrupamento A", 
                    Ativo = true,
                    Agrupamento = new Agrupamento { EmpresaId = empresaId }
                },
                new SubAgrupamento 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "SubAgrupamento B", 
                    Ativo = true,
                    Agrupamento = new Agrupamento { EmpresaId = empresaId }
                }
            };

            var subAgrupamentoDtos = new List<SubAgrupamentoDto>
            {
                new SubAgrupamentoDto 
                { 
                    Id = subAgrupamentos[0].Id, 
                    Nome = "SubAgrupamento A", 
                    Ativo = true 
                },
                new SubAgrupamentoDto 
                { 
                    Id = subAgrupamentos[1].Id, 
                    Nome = "SubAgrupamento B", 
                    Ativo = true 
                }
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByEmpresaAsync(empresaId))
                .ReturnsAsync(subAgrupamentos);
            _mapperMock.Setup(x => x.Map<IEnumerable<SubAgrupamentoDto>>(subAgrupamentos))
                .Returns(subAgrupamentoDtos);

            // Act
            var resultado = await _subAgrupamentoService.GetByEmpresaAsync(empresaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
        }
    }
}