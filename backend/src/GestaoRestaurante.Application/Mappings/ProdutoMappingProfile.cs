using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Mappings;

public class ProdutoMappingProfile : Profile
{
    public ProdutoMappingProfile()
    {
        // Produto -> ProdutoDto
        CreateMap<Produto, ProdutoDto>()
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria.Nome))
            .ForMember(dest => dest.CentroCustoNome, opt => opt.MapFrom(src => src.Categoria.CentroCusto.Nome))
            .ForMember(dest => dest.SubAgrupamentoNome, opt => opt.MapFrom(src => src.Categoria.CentroCusto.SubAgrupamento.Nome))
            .ForMember(dest => dest.AgrupamentoNome, opt => opt.MapFrom(src => src.Categoria.CentroCusto.SubAgrupamento.Agrupamento.Nome))
            .ForMember(dest => dest.EmpresaNome, opt => opt.MapFrom(src => src.Categoria.CentroCusto.SubAgrupamento.Agrupamento.Filial.Empresa.RazaoSocial))
            .ForMember(dest => dest.TotalIngredientes, opt => opt.MapFrom(src => src.Ingredientes != null ? src.Ingredientes.Count : 0));

        // CreateProdutoDto -> Produto
        CreateMap<CreateProdutoDto, Produto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Ingredientes, opt => opt.Ignore());

        // UpdateProdutoDto -> Produto
        CreateMap<UpdateProdutoDto, Produto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CategoriaId, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Ingredientes, opt => opt.Ignore());
    }
}