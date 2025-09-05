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
    public class CategoriaServiceTests
    {
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly Mock<ICentroCustoRepository> _centroCustoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CategoriaService _categoriaService;

        public CategoriaServiceTests()
        {
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _centroCustoRepositoryMock = new Mock<ICentroCustoRepository>();
            _mapperMock = new Mock<IMapper>();
            _categoriaService = new CategoriaService(
                _categoriaRepositoryMock.Object,
                _centroCustoRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodasCategorias()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = Guid.NewGuid(), Nome = "Categoria 1", Nivel = 1, Ativo = true },
                new Categoria { Id = Guid.NewGuid(), Nome = "Categoria 2", Nivel = 1, Ativo = true }
            };

            var categoriaDtos = new List<CategoriaDto>
            {
                new CategoriaDto { Id = categorias[0].Id, Nome = "Categoria 1", Nivel = 1, Ativo = true },
                new CategoriaDto { Id = categorias[1].Id, Nome = "Categoria 2", Nivel = 1, Ativo = true }
            };

            _categoriaRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(categorias);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoriaDto>>(categorias))
                .Returns(categoriaDtos);

            // Act
            var resultado = await _categoriaService.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(categoriaDtos);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarCategoria()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var categoria = new Categoria 
            { 
                Id = categoriaId, 
                Nome = "Categoria Teste", 
                Nivel = 1, 
                Ativo = true 
            };
            var categoriaDto = new CategoriaDto 
            { 
                Id = categoriaId, 
                Nome = "Categoria Teste", 
                Nivel = 1, 
                Ativo = true 
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);
            _mapperMock.Setup(x => x.Map<CategoriaDto>(categoria))
                .Returns(categoriaDto);

            // Act
            var resultado = await _categoriaService.GetByIdAsync(categoriaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(categoriaDto);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync((Categoria)null);

            // Act
            var resultado = await _categoriaService.GetByIdAsync(categoriaId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarCategoria()
        {
            // Arrange
            var centroCustoId = Guid.NewGuid();
            var centroCusto = new CentroCusto 
            { 
                Id = centroCustoId, 
                Nome = "Centro Custo Teste", 
                Ativo = true 
            };

            var categoriaDto = new CategoriaDto
            {
                Nome = "Nova Categoria",
                Codigo = "CAT001",
                CentroCustoId = centroCustoId,
                Nivel = 1,
                Ativo = true
            };

            var categoria = new Categoria
            {
                Id = Guid.NewGuid(),
                Nome = "Nova Categoria",
                Codigo = "CAT001",
                CentroCustoId = centroCustoId,
                Nivel = 1,
                Ativo = true
            };

            var categoriaCriada = new Categoria
            {
                Id = categoria.Id,
                Nome = "Nova Categoria",
                Codigo = "CAT001",
                CentroCustoId = centroCustoId,
                Nivel = 1,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync(centroCusto);
            _mapperMock.Setup(x => x.Map<Categoria>(categoriaDto))
                .Returns(categoria);
            _categoriaRepositoryMock.Setup(x => x.AddAsync(categoria))
                .ReturnsAsync(categoriaCriada);
            _mapperMock.Setup(x => x.Map<CategoriaDto>(categoriaCriada))
                .Returns(new CategoriaDto 
                { 
                    Id = categoriaCriada.Id,
                    Nome = categoriaCriada.Nome,
                    Codigo = categoriaCriada.Codigo,
                    CentroCustoId = categoriaCriada.CentroCustoId,
                    Nivel = categoriaCriada.Nivel,
                    Ativo = categoriaCriada.Ativo
                });

            // Act
            var resultado = await _categoriaService.CreateAsync(categoriaDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(categoriaCriada.Id);
            resultado.Nome.Should().Be("Nova Categoria");
            _categoriaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Categoria>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ComCentroCustoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var centroCustoId = Guid.NewGuid();
            var categoriaDto = new CategoriaDto
            {
                Nome = "Nova Categoria",
                CentroCustoId = centroCustoId,
                Nivel = 1
            };

            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync((CentroCusto)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _categoriaService.CreateAsync(categoriaDto));
            
            exception.Message.Should().Contain("Centro de custo não encontrado");
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarCategoria()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var centroCustoId = Guid.NewGuid();
            
            var categoriaExistente = new Categoria
            {
                Id = categoriaId,
                Nome = "Categoria Antiga",
                CentroCustoId = centroCustoId,
                Nivel = 1,
                Ativo = true
            };

            var centroCusto = new CentroCusto 
            { 
                Id = centroCustoId, 
                Nome = "Centro Custo Teste", 
                Ativo = true 
            };

            var categoriaDto = new CategoriaDto
            {
                Id = categoriaId,
                Nome = "Categoria Atualizada",
                CentroCustoId = centroCustoId,
                Nivel = 1,
                Ativo = true
            };

            var categoriaAtualizada = new Categoria
            {
                Id = categoriaId,
                Nome = "Categoria Atualizada",
                CentroCustoId = centroCustoId,
                Nivel = 1,
                Ativo = true
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoriaExistente);
            _centroCustoRepositoryMock.Setup(x => x.GetByIdAsync(centroCustoId))
                .ReturnsAsync(centroCusto);
            _categoriaRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Categoria>()))
                .ReturnsAsync(categoriaAtualizada);
            _mapperMock.Setup(x => x.Map<CategoriaDto>(categoriaAtualizada))
                .Returns(categoriaDto);

            // Act
            var resultado = await _categoriaService.UpdateAsync(categoriaId, categoriaDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Categoria Atualizada");
            _categoriaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Categoria>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComCategoriaInexistente_DeveRetornarNull()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var categoriaDto = new CategoriaDto { Id = categoriaId, Nome = "Teste" };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync((Categoria)null);

            // Act
            var resultado = await _categoriaService.UpdateAsync(categoriaId, categoriaDto);

            // Assert
            resultado.Should().BeNull();
            _categoriaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ComCategoriaExistente_DeveExcluirCategoria()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var categoria = new Categoria 
            { 
                Id = categoriaId, 
                Nome = "Categoria para Excluir", 
                Ativo = true 
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);
            _categoriaRepositoryMock.Setup(x => x.DeleteAsync(categoriaId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _categoriaService.DeleteAsync(categoriaId);

            // Assert
            resultado.Should().BeTrue();
            _categoriaRepositoryMock.Verify(x => x.DeleteAsync(categoriaId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ComCategoriaInexistente_DeveRetornarFalse()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync((Categoria)null);

            // Act
            var resultado = await _categoriaService.DeleteAsync(categoriaId);

            // Assert
            resultado.Should().BeFalse();
            _categoriaRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetByNivelAsync_DeveRetornarCategoriasPorNivel()
        {
            // Arrange
            var nivel = 1;
            var categorias = new List<Categoria>
            {
                new Categoria { Id = Guid.NewGuid(), Nome = "Categoria Nível 1 - A", Nivel = 1, Ativo = true },
                new Categoria { Id = Guid.NewGuid(), Nome = "Categoria Nível 1 - B", Nivel = 1, Ativo = true }
            };

            var categoriaDtos = new List<CategoriaDto>
            {
                new CategoriaDto { Id = categorias[0].Id, Nome = "Categoria Nível 1 - A", Nivel = 1, Ativo = true },
                new CategoriaDto { Id = categorias[1].Id, Nome = "Categoria Nível 1 - B", Nivel = 1, Ativo = true }
            };

            _categoriaRepositoryMock.Setup(x => x.GetByNivelAsync(nivel))
                .ReturnsAsync(categorias);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoriaDto>>(categorias))
                .Returns(categoriaDtos);

            // Act
            var resultado = await _categoriaService.GetByNivelAsync(nivel);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.All(c => c.Nivel == nivel).Should().BeTrue();
        }

        [Fact]
        public async Task GetFilhasAsync_DeveRetornarCategoriasFilhas()
        {
            // Arrange
            var categoriaPaiId = Guid.NewGuid();
            var categoriasFilhas = new List<Categoria>
            {
                new Categoria 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Subcategoria A", 
                    CategoriaPaiId = categoriaPaiId,
                    Nivel = 2, 
                    Ativo = true 
                },
                new Categoria 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Subcategoria B", 
                    CategoriaPaiId = categoriaPaiId,
                    Nivel = 2, 
                    Ativo = true 
                }
            };

            var categoriaFilhaDtos = new List<CategoriaDto>
            {
                new CategoriaDto 
                { 
                    Id = categoriasFilhas[0].Id, 
                    Nome = "Subcategoria A", 
                    CategoriaPaiId = categoriaPaiId,
                    Nivel = 2, 
                    Ativo = true 
                },
                new CategoriaDto 
                { 
                    Id = categoriasFilhas[1].Id, 
                    Nome = "Subcategoria B", 
                    CategoriaPaiId = categoriaPaiId,
                    Nivel = 2, 
                    Ativo = true 
                }
            };

            _categoriaRepositoryMock.Setup(x => x.GetFilhasAsync(categoriaPaiId))
                .ReturnsAsync(categoriasFilhas);
            _mapperMock.Setup(x => x.Map<IEnumerable<CategoriaDto>>(categoriasFilhas))
                .Returns(categoriaFilhaDtos);

            // Act
            var resultado = await _categoriaService.GetFilhasAsync(categoriaPaiId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.All(c => c.CategoriaPaiId == categoriaPaiId).Should().BeTrue();
            resultado.All(c => c.Nivel == 2).Should().BeTrue();
        }
    }
}