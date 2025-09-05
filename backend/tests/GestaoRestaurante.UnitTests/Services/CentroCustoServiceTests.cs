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
    public class CentroCustoServiceTests
    {
        private readonly Mock<ICentroCustoRepository> _centroCustoRepositoryMock;
        private readonly Mock<ISubAgrupamentoRepository> _subAgrupamentoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CentroCustoService _centroCustoService;

        public CentroCustoServiceTests()
        {
            _centroCustoRepositoryMock = new Mock<ICentroCustoRepository>();
            _subAgrupamentoRepositoryMock = new Mock<ISubAgrupamentoRepository>();
            _mapperMock = new Mock<IMapper>();
            _centroCustoService = new CentroCustoService(
                _centroCustoRepositoryMock.Object,
                _subAgrupamentoRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodosCentrosCusto()
        {
            // Arrange
            var centrosCusto = new List<CentroCusto>
            {
                new CentroCusto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Centro Custo 1", 
                    Codigo = "CC001",
                    Ativo = true 
                },
                new CentroCusto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Centro Custo 2", 
                    Codigo = "CC002",
                    Ativo = true 
                }
            };

            var centroCustoDtos = new List<CentroCustoDto>
            {
                new CentroCustoDto 
                { 
                    Id = centrosCusto[0].Id, 
                    Nome = "Centro Custo 1", 
                    Codigo = "CC001",
                    Ativo = true 
                },
                new CentroCustoDto 
                { 
                    Id = centrosCusto[1].Id, 
                    Nome = "Centro Custo 2", 
                    Codigo = "CC002",
                    Ativo = true 
                }
            };

            _centroCustoRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(centrosCusto);
            _mapperMock.Setup(x => x.Map<IEnumerable<CentroCustoDto>>(centrosCusto))
                .Returns(centroCustoDtos);

            // Act
            var resultado = await _centroCustoService.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(centroCustoDtos);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarCentroCusto()
        {
            // Arrange
            var centroCustoId = Guid.NewGuid();
            var centroCusto = new CentroCusto 
            { 
                Id = centroCustoId, 
                Nome = "Centro Custo Teste", 
                Codigo = "CC001",
                Ativo = true 
            };
            var centroCustoDto = new CentroCustoDto 
            { 
                Id = centroCustoId, 
                Nome = "Centro Custo Teste", 
                Codigo = "CC001",
                Ativo = true 
            };

            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync(centroCusto);
            _mapperMock.Setup(x => x.Map<CentroCustoDto>(centroCusto))
                .Returns(centroCustoDto);

            // Act
            var resultado = await _centroCustoService.GetByIdAsync(centroCustoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(centroCustoDto);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Arrange
            var centroCustoId = Guid.NewGuid();

            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync((CentroCusto)null);

            // Act
            var resultado = await _centroCustoService.GetByIdAsync(centroCustoId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarCentroCusto()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var subAgrupamento = new SubAgrupamento 
            { 
                Id = subAgrupamentoId, 
                Nome = "SubAgrupamento Teste", 
                Ativo = true 
            };

            var centroCustoDto = new CentroCustoDto
            {
                Nome = "Novo Centro Custo",
                Codigo = "CC001",
                SubAgrupamentoId = subAgrupamentoId,
                Descricao = "Descrição do centro de custo",
                Ativo = true
            };

            var centroCusto = new CentroCusto
            {
                Id = Guid.NewGuid(),
                Nome = "Novo Centro Custo",
                Codigo = "CC001",
                SubAgrupamentoId = subAgrupamentoId,
                Descricao = "Descrição do centro de custo",
                Ativo = true
            };

            var centroCustoCriado = new CentroCusto
            {
                Id = centroCusto.Id,
                Nome = "Novo Centro Custo",
                Codigo = "CC001",
                SubAgrupamentoId = subAgrupamentoId,
                Descricao = "Descrição do centro de custo",
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync(subAgrupamento);
            _mapperMock.Setup(x => x.Map<CentroCusto>(centroCustoDto))
                .Returns(centroCusto);
            _centroCustoRepositoryMock.Setup(x => x.AddAsync(centroCusto))
                .ReturnsAsync(centroCustoCriado);
            _mapperMock.Setup(x => x.Map<CentroCustoDto>(centroCustoCriado))
                .Returns(new CentroCustoDto 
                { 
                    Id = centroCustoCriado.Id,
                    Nome = centroCustoCriado.Nome,
                    Codigo = centroCustoCriado.Codigo,
                    SubAgrupamentoId = centroCustoCriado.SubAgrupamentoId,
                    Descricao = centroCustoCriado.Descricao,
                    Ativo = centroCustoCriado.Ativo
                });

            // Act
            var resultado = await _centroCustoService.CreateAsync(centroCustoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(centroCustoCriado.Id);
            resultado.Nome.Should().Be("Novo Centro Custo");
            resultado.Codigo.Should().Be("CC001");
            _centroCustoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CentroCusto>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ComSubAgrupamentoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var centroCustoDto = new CentroCustoDto
            {
                Nome = "Novo Centro Custo",
                SubAgrupamentoId = subAgrupamentoId
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync((SubAgrupamento)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _centroCustoService.CreateAsync(centroCustoDto));
            
            exception.Message.Should().Contain("SubAgrupamento não encontrado");
        }

        [Fact]
        public async Task CreateAsync_ComSubAgrupamentoInativo_DeveLancarExcecao()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var subAgrupamento = new SubAgrupamento 
            { 
                Id = subAgrupamentoId, 
                Nome = "SubAgrupamento Inativo", 
                Ativo = false 
            };

            var centroCustoDto = new CentroCustoDto
            {
                Nome = "Novo Centro Custo",
                SubAgrupamentoId = subAgrupamentoId
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync(subAgrupamento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _centroCustoService.CreateAsync(centroCustoDto));
            
            exception.Message.Should().Contain("SubAgrupamento deve estar ativo");
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarCentroCusto()
        {
            // Arrange
            var centroCustoId = Guid.NewGuid();
            var subAgrupamentoId = Guid.NewGuid();
            
            var centroCustoExistente = new CentroCusto
            {
                Id = centroCustoId,
                Nome = "Centro Custo Antigo",
                SubAgrupamentoId = subAgrupamentoId,
                Ativo = true
            };

            var subAgrupamento = new SubAgrupamento 
            { 
                Id = subAgrupamentoId, 
                Nome = "SubAgrupamento Teste", 
                Ativo = true 
            };

            var centroCustoDto = new CentroCustoDto
            {
                Id = centroCustoId,
                Nome = "Centro Custo Atualizado",
                SubAgrupamentoId = subAgrupamentoId,
                Ativo = true
            };

            var centroCustoAtualizado = new CentroCusto
            {
                Id = centroCustoId,
                Nome = "Centro Custo Atualizado",
                SubAgrupamentoId = subAgrupamentoId,
                Ativo = true
            };

            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync(centroCustoExistente);
            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync(subAgrupamento);
            _centroCustoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CentroCusto>()))
                .ReturnsAsync(centroCustoAtualizado);
            _mapperMock.Setup(x => x.Map<CentroCustoDto>(centroCustoAtualizado))
                .Returns(centroCustoDto);

            // Act
            var resultado = await _centroCustoService.UpdateAsync(centroCustoId, centroCustoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Centro Custo Atualizado");
            _centroCustoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CentroCusto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComCentroCustoInexistente_DeveRetornarNull()
        {
            // Arrange
            var centroCustoId = Guid.NewGuid();
            var centroCustoDto = new CentroCustoDto { Id = centroCustoId, Nome = "Teste" };

            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync((CentroCusto)null);

            // Act
            var resultado = await _centroCustoService.UpdateAsync(centroCustoId, centroCustoDto);

            // Assert
            resultado.Should().BeNull();
            _centroCustoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CentroCusto>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ComCentroCustoExistente_DeveExcluirCentroCusto()
        {
            // Arrange
            var centroCustoId = Guid.NewGuid();
            var centroCusto = new CentroCusto 
            { 
                Id = centroCustoId, 
                Nome = "Centro Custo para Excluir", 
                Ativo = true 
            };

            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync(centroCusto);
            _centroCustoRepositoryMock.Setup(x => x.DeleteAsync(centroCustoId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _centroCustoService.DeleteAsync(centroCustoId);

            // Assert
            resultado.Should().BeTrue();
            _centroCustoRepositoryMock.Verify(x => x.DeleteAsync(centroCustoId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ComCentroCustoInexistente_DeveRetornarFalse()
        {
            // Arrange
            var centroCustoId = Guid.NewGuid();

            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync((CentroCusto)null);

            // Act
            var resultado = await _centroCustoService.DeleteAsync(centroCustoId);

            // Assert
            resultado.Should().BeFalse();
            _centroCustoRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetBySubAgrupamentoAsync_DeveRetornarCentrosCustoPorSubAgrupamento()
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var centrosCusto = new List<CentroCusto>
            {
                new CentroCusto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Centro Custo A", 
                    SubAgrupamentoId = subAgrupamentoId, 
                    Ativo = true 
                },
                new CentroCusto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Centro Custo B", 
                    SubAgrupamentoId = subAgrupamentoId, 
                    Ativo = true 
                }
            };

            var centroCustoDtos = new List<CentroCustoDto>
            {
                new CentroCustoDto 
                { 
                    Id = centrosCusto[0].Id, 
                    Nome = "Centro Custo A", 
                    SubAgrupamentoId = subAgrupamentoId, 
                    Ativo = true 
                },
                new CentroCustoDto 
                { 
                    Id = centrosCusto[1].Id, 
                    Nome = "Centro Custo B", 
                    SubAgrupamentoId = subAgrupamentoId, 
                    Ativo = true 
                }
            };

            _centroCustoRepositoryMock.Setup(x => x.GetBySubAgrupamentoAsync(subAgrupamentoId))
                .ReturnsAsync(centrosCusto);
            _mapperMock.Setup(x => x.Map<IEnumerable<CentroCustoDto>>(centrosCusto))
                .Returns(centroCustoDtos);

            // Act
            var resultado = await _centroCustoService.GetBySubAgrupamentoAsync(subAgrupamentoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(c => c.SubAgrupamentoId == subAgrupamentoId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var subAgrupamento = new SubAgrupamento 
            { 
                Id = subAgrupamentoId, 
                Nome = "SubAgrupamento Teste", 
                Ativo = true 
            };

            var centroCustoDto = new CentroCustoDto
            {
                Nome = nomeInvalido,
                SubAgrupamentoId = subAgrupamentoId
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync(subAgrupamento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _centroCustoService.CreateAsync(centroCustoDto));
            
            exception.Message.Should().Contain("Nome é obrigatório");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_ComCodigoInvalido_DeveLancarExcecao(string codigoInvalido)
        {
            // Arrange
            var subAgrupamentoId = Guid.NewGuid();
            var subAgrupamento = new SubAgrupamento 
            { 
                Id = subAgrupamentoId, 
                Nome = "SubAgrupamento Teste", 
                Ativo = true 
            };

            var centroCustoDto = new CentroCustoDto
            {
                Nome = "Centro Custo Teste",
                Codigo = codigoInvalido,
                SubAgrupamentoId = subAgrupamentoId
            };

            _subAgrupamentoRepositoryMock.Setup(x => x.GetByIdAsync(subAgrupamentoId))
                .ReturnsAsync(subAgrupamento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _centroCustoService.CreateAsync(centroCustoDto));
            
            exception.Message.Should().Contain("Código é obrigatório");
        }

        [Fact]
        public async Task GetByCodigoAsync_ComCodigoValido_DeveRetornarCentroCusto()
        {
            // Arrange
            var codigo = "CC001";
            var subAgrupamentoId = Guid.NewGuid();
            var centroCusto = new CentroCusto 
            { 
                Id = Guid.NewGuid(), 
                Codigo = codigo,
                Nome = "Centro Custo Teste", 
                SubAgrupamentoId = subAgrupamentoId,
                Ativo = true 
            };
            var centroCustoDto = new CentroCustoDto 
            { 
                Id = centroCusto.Id, 
                Codigo = codigo,
                Nome = "Centro Custo Teste", 
                SubAgrupamentoId = subAgrupamentoId,
                Ativo = true 
            };

            _centroCustoRepositoryMock.Setup(x => x.GetByCodigoAsync(codigo, subAgrupamentoId))
                .ReturnsAsync(centroCusto);
            _mapperMock.Setup(x => x.Map<CentroCustoDto>(centroCusto))
                .Returns(centroCustoDto);

            // Act
            var resultado = await _centroCustoService.GetByCodigoAsync(codigo, subAgrupamentoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Codigo.Should().Be(codigo);
            resultado.SubAgrupamentoId.Should().Be(subAgrupamentoId);
        }

        [Fact]
        public async Task GetByCodigoAsync_ComCodigoInexistente_DeveRetornarNull()
        {
            // Arrange
            var codigo = "CC999";
            var subAgrupamentoId = Guid.NewGuid();

            _centroCustoRepositoryMock.Setup(x => x.GetByCodigoAsync(codigo, subAgrupamentoId))
                .ReturnsAsync((CentroCusto)null);

            // Act
            var resultado = await _centroCustoService.GetByCodigoAsync(codigo, subAgrupamentoId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmpresaAsync_DeveRetornarCentrosCustoPorEmpresa()
        {
            // Arrange
            var empresaId = Guid.NewGuid();
            var centrosCusto = new List<CentroCusto>
            {
                new CentroCusto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Centro Custo A", 
                    Ativo = true,
                    SubAgrupamento = new SubAgrupamento 
                    { 
                        Agrupamento = new Agrupamento { EmpresaId = empresaId } 
                    }
                },
                new CentroCusto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Centro Custo B", 
                    Ativo = true,
                    SubAgrupamento = new SubAgrupamento 
                    { 
                        Agrupamento = new Agrupamento { EmpresaId = empresaId } 
                    }
                }
            };

            var centroCustoDtos = new List<CentroCustoDto>
            {
                new CentroCustoDto 
                { 
                    Id = centrosCusto[0].Id, 
                    Nome = "Centro Custo A", 
                    Ativo = true 
                },
                new CentroCustoDto 
                { 
                    Id = centrosCusto[1].Id, 
                    Nome = "Centro Custo B", 
                    Ativo = true 
                }
            };

            _centroCustoRepositoryMock.Setup(x => x.GetByEmpresaAsync(empresaId))
                .ReturnsAsync(centrosCusto);
            _mapperMock.Setup(x => x.Map<IEnumerable<CentroCustoDto>>(centrosCusto))
                .Returns(centroCustoDtos);

            // Act
            var resultado = await _centroCustoService.GetByEmpresaAsync(empresaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByFilialAsync_DeveRetornarCentrosCustoPorFilial()
        {
            // Arrange
            var filialId = Guid.NewGuid();
            var centrosCusto = new List<CentroCusto>
            {
                new CentroCusto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Centro Custo A", 
                    Ativo = true 
                },
                new CentroCusto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Centro Custo B", 
                    Ativo = true 
                }
            };

            var centroCustoDtos = new List<CentroCustoDto>
            {
                new CentroCustoDto 
                { 
                    Id = centrosCusto[0].Id, 
                    Nome = "Centro Custo A", 
                    Ativo = true 
                },
                new CentroCustoDto 
                { 
                    Id = centrosCusto[1].Id, 
                    Nome = "Centro Custo B", 
                    Ativo = true 
                }
            };

            _centroCustoRepositoryMock.Setup(x => x.GetByFilialAsync(filialId))
                .ReturnsAsync(centrosCusto);
            _mapperMock.Setup(x => x.Map<IEnumerable<CentroCustoDto>>(centrosCusto))
                .Returns(centroCustoDtos);

            // Act
            var resultado = await _centroCustoService.GetByFilialAsync(filialId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
        }
    }
}