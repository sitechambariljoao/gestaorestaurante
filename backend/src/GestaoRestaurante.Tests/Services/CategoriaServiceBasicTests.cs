using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Services;
using GestaoRestaurante.Application.Validators;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GestaoRestaurante.Tests.Services
{
    public class CategoriaServiceBasicTests
    {
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly Mock<ICentroCustoRepository> _centroCustoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<CreateCategoriaDbValidator> _createDbValidatorMock;
        private readonly Mock<UpdateCategoriaDbValidator> _updateDbValidatorMock;
        private readonly Mock<ILogger<CategoriaService>> _loggerMock;
        private readonly CategoriaService _categoriaService;

        public CategoriaServiceBasicTests()
        {
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _centroCustoRepositoryMock = new Mock<ICentroCustoRepository>();
            _mapperMock = new Mock<IMapper>();
            _createDbValidatorMock = new Mock<CreateCategoriaDbValidator>(_categoriaRepositoryMock.Object);
            _updateDbValidatorMock = new Mock<UpdateCategoriaDbValidator>(_categoriaRepositoryMock.Object);
            _loggerMock = new Mock<ILogger<CategoriaService>>();
            _categoriaService = new CategoriaService(
                _categoriaRepositoryMock.Object,
                _centroCustoRepositoryMock.Object,
                _mapperMock.Object,
                _createDbValidatorMock.Object,
                _updateDbValidatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarResultadoComSuccess()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = Guid.NewGuid(), Nome = "Categoria 1", Nivel = 1, Ativa = true },
                new Categoria { Id = Guid.NewGuid(), Nome = "Categoria 2", Nivel = 1, Ativa = true }
            };

            var categoriasDto = new List<CategoriaDto>
            {
                new CategoriaDto { Id = categorias[0].Id, Nome = "Categoria 1", Nivel = 1 },
                new CategoriaDto { Id = categorias[1].Id, Nome = "Categoria 2", Nivel = 1 }
            };

            _categoriaRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(categorias);

            _mapperMock.Setup(x => x.Map<List<CategoriaDto>>(categorias))
                .Returns(categoriasDto);

            // Act
            var result = await _categoriaService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarSuccessComCategoria()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var categoria = new Categoria 
            { 
                Id = categoriaId, 
                Nome = "Categoria Teste", 
                Codigo = "CAT001",
                Nivel = 1, 
                Ativa = true 
            };

            var categoriaDto = new CategoriaDto
            {
                Id = categoriaId,
                Nome = "Categoria Teste",
                Codigo = "CAT001",
                Nivel = 1
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);

            _mapperMock.Setup(x => x.Map<CategoriaDto>(categoria))
                .Returns(categoriaDto);

            // Act
            var result = await _categoriaService.GetByIdAsync(categoriaId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Nome.Should().Be("Categoria Teste");
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarError()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync((Categoria?)null);

            // Act
            var result = await _categoriaService.GetByIdAsync(categoriaId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Categoria não encontrada");
        }

        [Fact]
        public void CategoriaService_DeveSerInstanciado()
        {
            // Assert
            _categoriaService.Should().NotBeNull();
            _categoriaService.Should().BeOfType<CategoriaService>();
        }

        [Fact]
        public void Mock_Setup_DeveEstarConfigurado()
        {
            // Assert
            _categoriaRepositoryMock.Should().NotBeNull();
            _centroCustoRepositoryMock.Should().NotBeNull();
            _loggerMock.Should().NotBeNull();
        }
    }
}