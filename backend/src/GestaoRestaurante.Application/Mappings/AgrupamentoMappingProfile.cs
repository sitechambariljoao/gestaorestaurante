using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Mappings;

public class AgrupamentoMappingProfile : Profile
{
    public AgrupamentoMappingProfile()
    {
        // Agrupamento -> AgrupamentoDto
        CreateMap<Agrupamento, AgrupamentoDto>()
            .ForMember(dest => dest.FilialNome, opt => opt.MapFrom(src => src.Filial.Nome))
            .ForMember(dest => dest.EmpresaNome, opt => opt.MapFrom(src => src.Filial.Empresa.RazaoSocial))
            .ForMember(dest => dest.TotalSubAgrupamentos, opt => opt.MapFrom(src => src.SubAgrupamentos.Count));

        // CreateAgrupamentoDto -> Agrupamento
        CreateMap<CreateAgrupamentoDto, Agrupamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.Ignore())
            .ForMember(dest => dest.Filial, opt => opt.Ignore())
            .ForMember(dest => dest.SubAgrupamentos, opt => opt.Ignore());

        // UpdateAgrupamentoDto -> Agrupamento
        CreateMap<UpdateAgrupamentoDto, Agrupamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FilialId, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Filial, opt => opt.Ignore())
            .ForMember(dest => dest.SubAgrupamentos, opt => opt.Ignore());
    }
}