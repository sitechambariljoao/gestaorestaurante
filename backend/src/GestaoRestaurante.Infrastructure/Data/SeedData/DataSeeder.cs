using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.ValueObjects;
using GestaoRestaurante.Infrastructure.Data.Context;

namespace GestaoRestaurante.Infrastructure.Data.SeedData;

public static class DataSeeder
{
    public static async Task SeedAsync(GestaoRestauranteContext context, UserManager<Usuario> userManager)
    {
        // Verificar se planos de assinatura existem
        if (!await context.PlanosAssinatura.AnyAsync())
        {
            await SeedPlanosAssinatura(context);
        }

        // Verificar se empresa demo existe
        if (!await context.Empresas.AnyAsync())
        {
            await SeedEmpresaDemo(context);
        }

        // Verificar se usuário admin existe
        var adminUser = await userManager.FindByEmailAsync("admin@restaurantedemo.com.br");
        if (adminUser == null)
        {
            await SeedUsuarioAdmin(context, userManager);
        }

        // Verificar se estrutura demo existe
        if (!await context.Agrupamentos.AnyAsync())
        {
            await SeedEstruturaDemo(context);
        }
        
        await context.SaveChangesAsync();
    }

    private static async Task SeedPlanosAssinatura(GestaoRestauranteContext context)
    {
        var planos = new[]
        {
            new PlanoAssinatura
            {
                Id = Guid.NewGuid(),
                Nome = "Básico",
                Valor = 99.90m,
                QuantidadeFiliais = 1,
                QuantidadeUsuarios = 3,
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                Modulos = new List<ModuloPlano>
                {
                    new() { Id = Guid.NewGuid(), NomeModulo = "EMPRESAS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "FILIAIS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "CENTRO_CUSTO", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "CATEGORIAS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "PRODUTOS", Liberado = true }
                }
            },
            new PlanoAssinatura
            {
                Id = Guid.NewGuid(),
                Nome = "Profissional",
                Valor = 199.90m,
                QuantidadeFiliais = 3,
                QuantidadeUsuarios = 10,
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                Modulos = new List<ModuloPlano>
                {
                    new() { Id = Guid.NewGuid(), NomeModulo = "EMPRESAS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "FILIAIS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "CENTRO_CUSTO", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "CATEGORIAS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "PRODUTOS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "CARDAPIO", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "ESTOQUE", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "PEDIDOS", Liberado = true }
                }
            },
            new PlanoAssinatura
            {
                Id = Guid.NewGuid(),
                Nome = "Enterprise",
                Valor = 399.90m,
                QuantidadeFiliais = 0, // Ilimitado
                QuantidadeUsuarios = 0, // Ilimitado
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                Modulos =
                [
                    new() { Id = Guid.NewGuid(), NomeModulo = "EMPRESAS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "FILIAIS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "CENTRO_CUSTO", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "CATEGORIAS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "PRODUTOS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "CARDAPIO", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "ESTOQUE", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "PEDIDOS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "FINANCEIRO", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "FUNCIONARIOS", Liberado = true },
                    new() { Id = Guid.NewGuid(), NomeModulo = "RELATORIOS", Liberado = true }
                ]
            }
        };

        await context.PlanosAssinatura.AddRangeAsync(planos);
    }

    private static async Task SeedEmpresaDemo(GestaoRestauranteContext context)
    {
        var empresaId = Guid.NewGuid();
        var empresa = new Empresa
        {
            Id = empresaId,
            RazaoSocial = "Restaurante Demo Ltda",
            NomeFantasia = "Restaurante Demo",
            Cnpj = "12.345.678/0001-90",
            Email = "contato@restaurantedemo.com.br",
            Telefone = "(11) 99999-9999",
            Endereco = new Endereco("Rua Demo", "123", null, "01234567", "Centro", "São Paulo", "SP"),
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        await context.Empresas.AddAsync(empresa);

        // Filial principal
        var filial = new Filial
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Nome = "Matriz",
            CnpjFilial = "12.345.678/0001-90",
            Email = "matriz@restaurantedemo.com.br",
            Telefone = "(11) 99999-9999",
            EnderecoFilial = new Endereco("Rua Demo", "123", null, "01234567", "Centro", "São Paulo", "SP"),
            Ativa = true,
            DataCriacao = DateTime.UtcNow,
            Matriz = true
        };

        await context.Filiais.AddAsync(filial);

        // Assinatura ativa - Plano Profissional
        var planoProfissional = await context.PlanosAssinatura
            .FirstOrDefaultAsync(p => p.Nome == "Profissional");

        if (planoProfissional != null)
        {
            var assinatura = new AssinaturaEmpresa
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                PlanoId = planoProfissional.Id,
                DataInicio = DateTime.UtcNow,
                DataVencimento = DateTime.UtcNow.AddYears(1),
                Ativa = true,
                DataCriacao = DateTime.UtcNow
            };

            await context.AssinaturasEmpresa.AddAsync(assinatura);
        }
    }

    private static async Task SeedUsuarioAdmin(GestaoRestauranteContext context, UserManager<Usuario> userManager)
    {
        var empresa = await context.Empresas.FirstOrDefaultAsync();
        if (empresa == null) return;

        var adminUser = new Usuario
        {
            Id = Guid.NewGuid(),
            UserName = "admin@restaurantedemo.com.br",
            Email = "admin@restaurantedemo.com.br",
            Nome = "Administrador Demo",
            EmpresaId = empresa.Id,
            Cpf = "123.456.789-00",
            Perfil = "Administrador",
            DataCriacao = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");

        if (result.Succeeded)
        {
            // Vincular usuário à filial
            var filial = await context.Filiais.FirstOrDefaultAsync(f => f.EmpresaId == empresa.Id);
            if (filial != null)
            {
                var usuarioFilial = new UsuarioFilial
                {
                    UsuarioId = adminUser.Id,
                    FilialId = filial.Id,
                    DataVinculo = DateTime.UtcNow,
                    Ativo = true
                };

                await context.UsuarioFiliais.AddAsync(usuarioFilial);
            }
        }
    }

    private static async Task SeedEstruturaDemo(GestaoRestauranteContext context)
    {
        var filial = await context.Filiais.FirstOrDefaultAsync();
        if (filial == null) return;

        // Agrupamento
        var agrupamento = new Agrupamento
        {
            Id = Guid.NewGuid(),
            FilialId = filial.Id,
            Codigo = "01",
            Nome = "Operacional",
            Descricao = "Atividades operacionais do restaurante",
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        await context.Agrupamentos.AddAsync(agrupamento);

        // SubAgrupamento
        var subAgrupamento = new SubAgrupamento
        {
            Id = Guid.NewGuid(),
            AgrupamentoId = agrupamento.Id,
            Codigo = "01",
            Nome = "Cozinha",
            Descricao = "Atividades da cozinha",
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        await context.SubAgrupamentos.AddAsync(subAgrupamento);

        // Centro de Custo
        var centroCusto = new CentroCusto
        {
            Id = Guid.NewGuid(),
            SubAgrupamentoId = subAgrupamento.Id,
            Codigo = "01",
            Nome = "Preparo de Alimentos",
            Descricao = "Centro de custo para preparo de alimentos",
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        await context.CentrosCusto.AddAsync(centroCusto);

        // Categorias
        var categoriaLevel1 = new Categoria
        {
            Id = Guid.NewGuid(),
            CentroCustoId = centroCusto.Id,
            CategoriaPaiId = null,
            Codigo = "01",
            Nome = "Pratos Principais",
            Descricao = "Categoria de pratos principais",
            Nivel = 1,
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        await context.Categorias.AddAsync(categoriaLevel1);

        var categoriaLevel2 = new Categoria
        {
            Id = Guid.NewGuid(),
            CentroCustoId = centroCusto.Id,
            CategoriaPaiId = categoriaLevel1.Id,
            Codigo = "01",
            Nome = "Carnes",
            Descricao = "Pratos com carnes",
            Nivel = 2,
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        await context.Categorias.AddAsync(categoriaLevel2);

        var categoriaLevel3 = new Categoria
        {
            Id = Guid.NewGuid(),
            CentroCustoId = centroCusto.Id,
            CategoriaPaiId = categoriaLevel2.Id,
            Codigo = "01",
            Nome = "Bovinos",
            Descricao = "Pratos com carne bovina",
            Nivel = 3,
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        await context.Categorias.AddAsync(categoriaLevel3);

        // Produtos Demo
        var produtos = new[]
        {
            new Produto
            {
                Id = Guid.NewGuid(),
                CategoriaId = categoriaLevel3.Id,
                Codigo = "001",
                Nome = "Filé Mignon Grelhado",
                Descricao = "Filé mignon grelhado com temperos especiais",
                Preco = 45.90m,
                UnidadeMedida = "Unidade",
                Ativa = true,
                ProdutoVenda = true,
                ProdutoEstoque = false,
                DataCriacao = DateTime.UtcNow
            },
            new Produto
            {
                Id = Guid.NewGuid(),
                CategoriaId = categoriaLevel3.Id,
                Codigo = "002",
                Nome = "Picanha na Brasa",
                Descricao = "Picanha grelhada na brasa com acompanhamentos",
                Preco = 38.50m,
                UnidadeMedida = "Unidade",
                Ativa = true,
                ProdutoVenda = true,
                ProdutoEstoque = false,
                DataCriacao = DateTime.UtcNow
            }
        };

        await context.Produtos.AddRangeAsync(produtos);
    }
}