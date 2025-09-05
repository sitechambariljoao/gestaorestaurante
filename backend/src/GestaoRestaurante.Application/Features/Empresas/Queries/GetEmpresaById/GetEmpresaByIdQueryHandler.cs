using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;

namespace GestaoRestaurante.Application.Features.Empresas.Queries.GetEmpresaById;

/// <summary>
/// Handler para a query de obter empresa por ID
/// </summary>
public sealed class GetEmpresaByIdQueryHandler : IQueryHandler<GetEmpresaByIdQuery, EmpresaDto>
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IMapper _mapper;

    public GetEmpresaByIdQueryHandler(IEmpresaRepository empresaRepository, IMapper mapper)
    {
        _empresaRepository = empresaRepository;
        _mapper = mapper;
    }

    public async Task<Result<EmpresaDto>> Handle(GetEmpresaByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.Id);
            
            if (empresa == null)
                return Result<EmpresaDto>.Failure("Empresa não encontrada");

            if (!empresa.Ativa)
                return Result<EmpresaDto>.Failure("Empresa está inativa");

            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return Result<EmpresaDto>.Success(empresaDto);
        }
        catch (Exception ex)
        {
            return Result<EmpresaDto>.Failure($"Erro ao buscar empresa: {ex.Message}");
        }
    }
}