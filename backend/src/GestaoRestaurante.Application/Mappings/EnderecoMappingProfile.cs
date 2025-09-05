using AutoMapper;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.ValueObjects;

namespace GestaoRestaurante.Application.Mappings;

public class EnderecoMappingProfile : Profile
{
    public EnderecoMappingProfile()
    {
        // Endereco -> EnderecoDto
        CreateMap<Endereco, EnderecoDto>();

        // EnderecoDto -> Endereco
        CreateMap<EnderecoDto, Endereco>();
    }
}