using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.ValueObjects;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Features.Empresas.Commands.CreateEmpresa;

namespace GestaoRestaurante.Tests.Helpers;

/// <summary>
/// Builder pattern para criação de dados de teste
/// </summary>
public static class TestDataBuilder
{
    public static EmpresaBuilder UmaEmpresa() => new();
    public static ProdutoBuilder UmProduto() => new();

    /// <summary>
    /// Builder para entidade Empresa
    /// </summary>
    public class EmpresaBuilder
    {
        private string _razaoSocial = "Empresa Teste Ltda";
        private string _nomeFantasia = "Empresa Teste";
        private string _cnpj = "12.345.678/0001-90";
        private string _email = "teste@empresa.com";
        private string? _telefone = "(11) 99999-9999";
        private Endereco? _endereco;
        private bool _ativa = true;

        public EmpresaBuilder ComRazaoSocial(string razaoSocial)
        {
            _razaoSocial = razaoSocial;
            return this;
        }

        public EmpresaBuilder ComNomeFantasia(string nomeFantasia)
        {
            _nomeFantasia = nomeFantasia;
            return this;
        }

        public EmpresaBuilder ComCnpj(string cnpj)
        {
            _cnpj = cnpj;
            return this;
        }

        public EmpresaBuilder ComEmail(string email)
        {
            _email = email;
            return this;
        }

        public EmpresaBuilder ComTelefone(string telefone)
        {
            _telefone = telefone;
            return this;
        }

        public EmpresaBuilder ComEndereco(string logradouro = "Rua Teste, 123",
                                        string cidade = "São Paulo", 
                                        string estado = "SP", 
                                        string cep = "01234-567")
        {
            _endereco = new Endereco(logradouro, "100", null, cep, "Centro", cidade, estado);
            return this;
        }

        public EmpresaBuilder Inativa()
        {
            _ativa = false;
            return this;
        }

        public Empresa Build()
        {
            var endereco = _endereco ?? new Endereco("Rua Default, 123", "100", null, "01234-567", "Centro", "São Paulo", "SP");
            
            // Criando uma empresa com construtor vazio e configurando propriedades
            var empresa = new Empresa
            {
                Id = Guid.NewGuid(),
                RazaoSocial = _razaoSocial,
                NomeFantasia = _nomeFantasia,
                Cnpj = _cnpj,
                Email = _email,
                Telefone = _telefone,
                Endereco = endereco,
                Ativa = _ativa,
                DataCriacao = DateTime.UtcNow
            };
            
            return empresa;
        }

        public CreateEmpresaCommand BuildCreateCommand()
        {
            var endereco = _endereco ?? new Endereco("Rua Default, 123", "100", null, "01234-567", "Centro", "São Paulo", "SP");
            
            return new CreateEmpresaCommand(
                _razaoSocial,
                _nomeFantasia,
                _cnpj,
                _email,
                _telefone,
                new EnderecoDto
                {
                    Logradouro = endereco.Logradouro,
                    Numero = endereco.Numero,
                    Complemento = endereco.Complemento,
                    Cep = endereco.Cep,
                    Bairro = endereco.Bairro,
                    Cidade = endereco.Cidade,
                    Estado = endereco.Estado
                }
            );
        }
    }

    /// <summary>
    /// Builder para entidade Produto
    /// </summary>
    public class ProdutoBuilder
    {
        private string _codigo = "PROD001";
        private string _nome = "Produto Teste";
        private string? _descricao = "Descrição do produto";
        private decimal _preco = 10.00m;
        private Guid? _categoriaId;
        private bool _ativo = true;

        public ProdutoBuilder ComNome(string nome)
        {
            _nome = nome;
            return this;
        }

        public ProdutoBuilder ComCodigo(string codigo)
        {
            _codigo = codigo;
            return this;
        }

        public ProdutoBuilder ComDescricao(string descricao)
        {
            _descricao = descricao;
            return this;
        }

        public ProdutoBuilder ComPreco(decimal preco)
        {
            _preco = preco;
            return this;
        }

        public ProdutoBuilder ComCategoria(Guid categoriaId)
        {
            _categoriaId = categoriaId;
            return this;
        }

        public ProdutoBuilder Inativo()
        {
            _ativo = false;
            return this;
        }

        public Produto Build()
        {
            return new Produto(
                _categoriaId ?? Guid.NewGuid(),
                _codigo,
                _nome,
                _preco,
                "UN",
                _descricao
            );
        }
    }
}