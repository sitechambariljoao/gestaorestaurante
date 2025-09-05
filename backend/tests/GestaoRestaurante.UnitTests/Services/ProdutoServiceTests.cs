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
    public class ProdutoServiceTests
    {
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProdutoService _produtoService;

        public ProdutoServiceTests()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _mapperMock = new Mock<IMapper>();
            _produtoService = new ProdutoService(
                _produtoRepositoryMock.Object,
                _categoriaRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodosProdutos()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new Produto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Produto 1", 
                    Preco = 10.50m, 
                    Ativo = true 
                },
                new Produto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Produto 2", 
                    Preco = 25.00m, 
                    Ativo = true 
                }
            };

            var produtoDtos = new List<ProdutoDto>
            {
                new ProdutoDto 
                { 
                    Id = produtos[0].Id, 
                    Nome = "Produto 1", 
                    Preco = 10.50m, 
                    Ativo = true 
                },
                new ProdutoDto 
                { 
                    Id = produtos[1].Id, 
                    Nome = "Produto 2", 
                    Preco = 25.00m, 
                    Ativo = true 
                }
            };

            _produtoRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(produtos);
            _mapperMock.Setup(x => x.Map<IEnumerable<ProdutoDto>>(produtos))
                .Returns(produtoDtos);

            // Act
            var resultado = await _produtoService.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(produtoDtos);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarProduto()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto 
            { 
                Id = produtoId, 
                Nome = "Produto Teste", 
                Preco = 15.75m, 
                Ativo = true 
            };
            var produtoDto = new ProdutoDto 
            { 
                Id = produtoId, 
                Nome = "Produto Teste", 
                Preco = 15.75m, 
                Ativo = true 
            };

            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId))
                .ReturnsAsync(produto);
            _mapperMock.Setup(x => x.Map<ProdutoDto>(produto))
                .Returns(produtoDto);

            // Act
            var resultado = await _produtoService.GetByIdAsync(produtoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(produtoDto);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Arrange
            var produtoId = Guid.NewGuid();

            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId))
                .ReturnsAsync((Produto)null);

            // Act
            var resultado = await _produtoService.GetByIdAsync(produtoId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarProduto()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var categoria = new Categoria 
            { 
                Id = categoriaId, 
                Nome = "Categoria Teste", 
                Nivel = 3, 
                Ativo = true 
            };

            var produtoDto = new ProdutoDto
            {
                Nome = "Novo Produto",
                Codigo = "PROD001",
                CategoriaId = categoriaId,
                Preco = 20.00m,
                UnidadeMedida = "UN",
                Ativo = true,
                ProdutoVenda = true,
                ProdutoEstoque = false
            };

            var produto = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Novo Produto",
                Codigo = "PROD001",
                CategoriaId = categoriaId,
                Preco = 20.00m,
                UnidadeMedida = "UN",
                Ativo = true,
                ProdutoVenda = true,
                ProdutoEstoque = false
            };

            var produtoCriado = new Produto
            {
                Id = produto.Id,
                Nome = "Novo Produto",
                Codigo = "PROD001",
                CategoriaId = categoriaId,
                Preco = 20.00m,
                UnidadeMedida = "UN",
                Ativo = true,
                ProdutoVenda = true,
                ProdutoEstoque = false,
                DataCriacao = DateTime.UtcNow
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);
            _mapperMock.Setup(x => x.Map<Produto>(produtoDto))
                .Returns(produto);
            _produtoRepositoryMock.Setup(x => x.AddAsync(produto))
                .ReturnsAsync(produtoCriado);
            _mapperMock.Setup(x => x.Map<ProdutoDto>(produtoCriado))
                .Returns(new ProdutoDto 
                { 
                    Id = produtoCriado.Id,
                    Nome = produtoCriado.Nome,
                    Codigo = produtoCriado.Codigo,
                    CategoriaId = produtoCriado.CategoriaId,
                    Preco = produtoCriado.Preco,
                    UnidadeMedida = produtoCriado.UnidadeMedida,
                    Ativo = produtoCriado.Ativo,
                    ProdutoVenda = produtoCriado.ProdutoVenda,
                    ProdutoEstoque = produtoCriado.ProdutoEstoque
                });

            // Act
            var resultado = await _produtoService.CreateAsync(produtoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(produtoCriado.Id);
            resultado.Nome.Should().Be("Novo Produto");
            resultado.Preco.Should().Be(20.00m);
            _produtoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Produto>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ComCategoriaInexistente_DeveLancarExcecao()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var produtoDto = new ProdutoDto
            {
                Nome = "Novo Produto",
                CategoriaId = categoriaId,
                Preco = 20.00m
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync((Categoria)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _produtoService.CreateAsync(produtoDto));
            
            exception.Message.Should().Contain("Categoria não encontrada");
        }

        [Fact]
        public async Task CreateAsync_ComCategoriaNivelInvalido_DeveLancarExcecao()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var categoria = new Categoria 
            { 
                Id = categoriaId, 
                Nome = "Categoria Nível 1", 
                Nivel = 1, 
                Ativo = true 
            };

            var produtoDto = new ProdutoDto
            {
                Nome = "Novo Produto",
                CategoriaId = categoriaId,
                Preco = 20.00m
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _produtoService.CreateAsync(produtoDto));
            
            exception.Message.Should().Contain("Produtos só podem ser vinculados a categorias de nível 3");
        }

        [Fact]
        public async Task CreateAsync_ComCategoriaInativa_DeveLancarExcecao()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var categoria = new Categoria 
            { 
                Id = categoriaId, 
                Nome = "Categoria Inativa", 
                Nivel = 3, 
                Ativo = false 
            };

            var produtoDto = new ProdutoDto
            {
                Nome = "Novo Produto",
                CategoriaId = categoriaId,
                Preco = 20.00m
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _produtoService.CreateAsync(produtoDto));
            
            exception.Message.Should().Contain("Categoria deve estar ativa");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5.50)]
        public async Task CreateAsync_ComPrecoInvalido_DeveLancarExcecao(decimal precoInvalido)
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var categoria = new Categoria 
            { 
                Id = categoriaId, 
                Nome = "Categoria Teste", 
                Nivel = 3, 
                Ativo = true 
            };

            var produtoDto = new ProdutoDto
            {
                Nome = "Novo Produto",
                CategoriaId = categoriaId,
                Preco = precoInvalido
            };

            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _produtoService.CreateAsync(produtoDto));
            
            exception.Message.Should().Contain("Preço deve ser maior que zero");
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarProduto()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var categoriaId = Guid.NewGuid();
            
            var produtoExistente = new Produto
            {
                Id = produtoId,
                Nome = "Produto Antigo",
                CategoriaId = categoriaId,
                Preco = 10.00m,
                Ativo = true
            };

            var categoria = new Categoria 
            { 
                Id = categoriaId, 
                Nome = "Categoria Teste", 
                Nivel = 3, 
                Ativo = true 
            };

            var produtoDto = new ProdutoDto
            {
                Id = produtoId,
                Nome = "Produto Atualizado",
                CategoriaId = categoriaId,
                Preco = 15.00m,
                Ativo = true
            };

            var produtoAtualizado = new Produto
            {
                Id = produtoId,
                Nome = "Produto Atualizado",
                CategoriaId = categoriaId,
                Preco = 15.00m,
                Ativo = true
            };

            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId))
                .ReturnsAsync(produtoExistente);
            _categoriaRepositoryMock.Setup(x => x.GetByIdAsync(categoriaId))
                .ReturnsAsync(categoria);
            _produtoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Produto>()))
                .ReturnsAsync(produtoAtualizado);
            _mapperMock.Setup(x => x.Map<ProdutoDto>(produtoAtualizado))
                .Returns(produtoDto);

            // Act
            var resultado = await _produtoService.UpdateAsync(produtoId, produtoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Produto Atualizado");
            resultado.Preco.Should().Be(15.00m);
            _produtoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Produto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComProdutoInexistente_DeveRetornarNull()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produtoDto = new ProdutoDto { Id = produtoId, Nome = "Teste" };

            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId))
                .ReturnsAsync((Produto)null);

            // Act
            var resultado = await _produtoService.UpdateAsync(produtoId, produtoDto);

            // Assert
            resultado.Should().BeNull();
            _produtoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Produto>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ComProdutoExistente_DeveExcluirProduto()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto 
            { 
                Id = produtoId, 
                Nome = "Produto para Excluir", 
                Ativo = true 
            };

            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId))
                .ReturnsAsync(produto);
            _produtoRepositoryMock.Setup(x => x.DeleteAsync(produtoId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _produtoService.DeleteAsync(produtoId);

            // Assert
            resultado.Should().BeTrue();
            _produtoRepositoryMock.Verify(x => x.DeleteAsync(produtoId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ComProdutoInexistente_DeveRetornarFalse()
        {
            // Arrange
            var produtoId = Guid.NewGuid();

            _produtoRepositoryMock.Setup(x => x.GetByIdAsync(produtoId))
                .ReturnsAsync((Produto)null);

            // Act
            var resultado = await _produtoService.DeleteAsync(produtoId);

            // Assert
            resultado.Should().BeFalse();
            _produtoRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetByCategoriaAsync_DeveRetornarProdutosPorCategoria()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var produtos = new List<Produto>
            {
                new Produto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Produto A", 
                    CategoriaId = categoriaId, 
                    Ativo = true 
                },
                new Produto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Produto B", 
                    CategoriaId = categoriaId, 
                    Ativo = true 
                }
            };

            var produtoDtos = new List<ProdutoDto>
            {
                new ProdutoDto 
                { 
                    Id = produtos[0].Id, 
                    Nome = "Produto A", 
                    CategoriaId = categoriaId, 
                    Ativo = true 
                },
                new ProdutoDto 
                { 
                    Id = produtos[1].Id, 
                    Nome = "Produto B", 
                    CategoriaId = categoriaId, 
                    Ativo = true 
                }
            };

            _produtoRepositoryMock.Setup(x => x.GetByCategoriaAsync(categoriaId))
                .ReturnsAsync(produtos);
            _mapperMock.Setup(x => x.Map<IEnumerable<ProdutoDto>>(produtos))
                .Returns(produtoDtos);

            // Act
            var resultado = await _produtoService.GetByCategoriaAsync(categoriaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.All(p => p.CategoriaId == categoriaId).Should().BeTrue();
        }

        [Fact]
        public async Task GetProdutosVendaAsync_DeveRetornarApenasProdutosDeVenda()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new Produto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Produto Venda 1", 
                    ProdutoVenda = true, 
                    Ativo = true 
                },
                new Produto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Produto Venda 2", 
                    ProdutoVenda = true, 
                    Ativo = true 
                }
            };

            var produtoDtos = new List<ProdutoDto>
            {
                new ProdutoDto 
                { 
                    Id = produtos[0].Id, 
                    Nome = "Produto Venda 1", 
                    ProdutoVenda = true, 
                    Ativo = true 
                },
                new ProdutoDto 
                { 
                    Id = produtos[1].Id, 
                    Nome = "Produto Venda 2", 
                    ProdutoVenda = true, 
                    Ativo = true 
                }
            };

            _produtoRepositoryMock.Setup(x => x.GetProdutosVendaAsync())
                .ReturnsAsync(produtos);
            _mapperMock.Setup(x => x.Map<IEnumerable<ProdutoDto>>(produtos))
                .Returns(produtoDtos);

            // Act
            var resultado = await _produtoService.GetProdutosVendaAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.All(p => p.ProdutoVenda == true).Should().BeTrue();
        }

        [Fact]
        public async Task GetProdutosEstoqueAsync_DeveRetornarApenasProdutosDeEstoque()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new Produto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Produto Estoque 1", 
                    ProdutoEstoque = true, 
                    Ativo = true 
                },
                new Produto 
                { 
                    Id = Guid.NewGuid(), 
                    Nome = "Produto Estoque 2", 
                    ProdutoEstoque = true, 
                    Ativo = true 
                }
            };

            var produtoDtos = new List<ProdutoDto>
            {
                new ProdutoDto 
                { 
                    Id = produtos[0].Id, 
                    Nome = "Produto Estoque 1", 
                    ProdutoEstoque = true, 
                    Ativo = true 
                },
                new ProdutoDto 
                { 
                    Id = produtos[1].Id, 
                    Nome = "Produto Estoque 2", 
                    ProdutoEstoque = true, 
                    Ativo = true 
                }
            };

            _produtoRepositoryMock.Setup(x => x.GetProdutosEstoqueAsync())
                .ReturnsAsync(produtos);
            _mapperMock.Setup(x => x.Map<IEnumerable<ProdutoDto>>(produtos))
                .Returns(produtoDtos);

            // Act
            var resultado = await _produtoService.GetProdutosEstoqueAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.All(p => p.ProdutoEstoque == true).Should().BeTrue();
        }
    }
}