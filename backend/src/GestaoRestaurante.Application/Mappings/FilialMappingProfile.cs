using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Application.Mappings;

public class FilialMappingProfile : Profile
{
    public FilialMappingProfile()
    {
        // Filial -> FilialDto
        CreateMap<Filial, FilialDto>()
            .ForMember(dest => dest.EmpresaNome, opt => opt.MapFrom(src => src.Empresa.RazaoSocial))
            .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.Cnpj))
            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco));

        // CreateFilialDto -> Filial
        CreateMap<CreateFilialDto, Filial>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CnpjFilial, opt => opt.MapFrom(src => src.Cnpj))
            .ForMember(dest => dest.EnderecoFilial, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Ativa, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.Ignore())
            .ForMember(dest => dest.Empresa, opt => opt.Ignore())
            .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
            .ForMember(dest => dest.Agrupamentos, opt => opt.Ignore());

        // UpdateFilialDto -> Filial
        CreateMap<UpdateFilialDto, Filial>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
            .ForMember(dest => dest.CnpjFilial, opt => opt.MapFrom(src => src.Cnpj))
            .ForMember(dest => dest.EnderecoFilial, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Ativa, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataUltimaAlteracao, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Empresa, opt => opt.Ignore())
            .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
            .ForMember(dest => dest.Agrupamentos, opt => opt.Ignore());
    }
}