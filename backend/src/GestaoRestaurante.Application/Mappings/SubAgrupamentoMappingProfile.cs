using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Mappings;

public class SubAgrupamentoMappingProfile : Profile
{
    public SubAgrupamentoMappingProfile()
    {
        // SubAgrupamento -> SubAgrupamentoDto
        CreateMap<SubAgrupamento, SubAgrupamentoDto>()
            .ForMember(dest => dest.AgrupamentoNome, opt => opt.MapFrom(src => src.Agrupamento.Nome))
            .ForMember(dest => dest.EmpresaNome, opt => opt.MapFrom(src => src.Agrupamento.Filial.Empresa.RazaoSocial))
            .ForMember(dest => dest.TotalCentrosCusto, opt => opt.MapFrom(src => src.CentrosCusto.Count));

        // CreateSubAgrupamentoDto -> SubAgrupamento
        CreateMap<CreateSubAgrupamentoDto, SubAgrupamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.Ignore())
            .ForMember(dest => dest.Agrupamento, opt => opt.Ignore())
            .ForMember(dest => dest.CentrosCusto, opt => opt.Ignore());

        // UpdateSubAgrupamentoDto -> SubAgrupamento
        CreateMap<UpdateSubAgrupamentoDto, SubAgrupamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AgrupamentoId, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Agrupamento, opt => opt.Ignore())
            .ForMember(dest => dest.CentrosCusto, opt => opt.Ignore());
    }
}