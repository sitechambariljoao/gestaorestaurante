using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Mappings;

public class CategoriaMappingProfile : Profile
{
    public CategoriaMappingProfile()
    {
        // Categoria -> CategoriaDto
        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.CentroCustoNome, opt => opt.MapFrom(src => src.CentroCusto.Nome))
            .ForMember(dest => dest.SubAgrupamentoNome, opt => opt.MapFrom(src => src.CentroCusto.SubAgrupamento.Nome))
            .ForMember(dest => dest.AgrupamentoNome, opt => opt.MapFrom(src => src.CentroCusto.SubAgrupamento.Agrupamento.Nome))
            .ForMember(dest => dest.EmpresaNome, opt => opt.MapFrom(src => src.CentroCusto.SubAgrupamento.Agrupamento.Filial.Empresa.RazaoSocial))
            .ForMember(dest => dest.CategoriaPaiNome, opt => opt.MapFrom(src => src.CategoriaPai != null ? src.CategoriaPai.Nome : null))
            .ForMember(dest => dest.TotalCategoriasFilhas, opt => opt.MapFrom(src => src.CategoriasFilhas.Count))
            .ForMember(dest => dest.TotalProdutos, opt => opt.MapFrom(src => src.Produtos.Count))
            .ForMember(dest => dest.CategoriasFilhas, opt => opt.MapFrom(src => src.CategoriasFilhas));

        // CreateCategoriaDto -> Categoria
        CreateMap<CreateCategoriaDto, Categoria>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.Ignore())
            .ForMember(dest => dest.CentroCusto, opt => opt.Ignore())
            .ForMember(dest => dest.CategoriaPai, opt => opt.Ignore())
            .ForMember(dest => dest.CategoriasFilhas, opt => opt.Ignore())
            .ForMember(dest => dest.Produtos, opt => opt.Ignore());

        // UpdateCategoriaDto -> Categoria
        CreateMap<UpdateCategoriaDto, Categoria>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CentroCustoId, opt => opt.Ignore())
            .ForMember(dest => dest.CategoriaPaiId, opt => opt.Ignore())
            .ForMember(dest => dest.Nivel, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CentroCusto, opt => opt.Ignore())
            .ForMember(dest => dest.CategoriaPai, opt => opt.Ignore())
            .ForMember(dest => dest.CategoriasFilhas, opt => opt.Ignore())
            .ForMember(dest => dest.Produtos, opt => opt.Ignore());
    }
}