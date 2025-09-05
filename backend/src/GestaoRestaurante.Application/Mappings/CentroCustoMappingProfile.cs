using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Mappings;

public class CentroCustoMappingProfile : Profile
{
    public CentroCustoMappingProfile()
    {
        // CentroCusto -> CentroCustoDto
        CreateMap<CentroCusto, CentroCustoDto>()
            .ForMember(dest => dest.SubAgrupamentoNome, opt => opt.MapFrom(src => src.SubAgrupamento.Nome))
            .ForMember(dest => dest.AgrupamentoNome, opt => opt.MapFrom(src => src.SubAgrupamento.Agrupamento.Nome))
            .ForMember(dest => dest.EmpresaNome, opt => opt.MapFrom(src => src.SubAgrupamento.Agrupamento.Filial.Empresa.RazaoSocial))
            .ForMember(dest => dest.TotalCategorias, opt => opt.MapFrom(src => src.Categorias.Count))
            .ForMember(dest => dest.TotalFiliais, opt => opt.MapFrom(src => 1)); // Assuming 1 filial per centro custo

        // CreateCentroCustoDto -> CentroCusto
        CreateMap<CreateCentroCustoDto, CentroCusto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.Ignore())
            .ForMember(dest => dest.SubAgrupamento, opt => opt.Ignore())
            .ForMember(dest => dest.Categorias, opt => opt.Ignore());

        // UpdateCentroCustoDto -> CentroCusto
        CreateMap<UpdateCentroCustoDto, CentroCusto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SubAgrupamentoId, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.SubAgrupamento, opt => opt.Ignore())
            .ForMember(dest => dest.Categorias, opt => opt.Ignore());
    }
}