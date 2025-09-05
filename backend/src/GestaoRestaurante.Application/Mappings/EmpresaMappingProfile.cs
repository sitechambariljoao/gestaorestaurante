using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Mappings;

public class EmpresaMappingProfile : Profile
{
    public EmpresaMappingProfile()
    {
        // Empresa -> EmpresaDto
        CreateMap<Empresa, EmpresaDto>()
            .ForMember(dest => dest.Filiais, opt => opt.MapFrom(src => src.Filiais));

        // CreateEmpresaDto -> Empresa
        CreateMap<CreateEmpresaDto, Empresa>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.Ignore())
            .ForMember(dest => dest.Filiais, opt => opt.Ignore());

        // UpdateEmpresaDto -> Empresa (for updating existing entity)
        CreateMap<UpdateEmpresaDto, Empresa>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Ativa, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Filiais, opt => opt.Ignore());
    }
}