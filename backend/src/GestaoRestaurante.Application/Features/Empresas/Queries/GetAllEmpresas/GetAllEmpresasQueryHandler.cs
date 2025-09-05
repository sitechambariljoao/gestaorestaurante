using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using GestaoRestaurante.Application.Common.Queries;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Specifications;

namespace GestaoRestaurante.Application.Features.Empresas.Queries.GetAllEmpresas;

/// <summary>
/// Handler para a query de obter todas as empresas
/// </summary>
public sealed class GetAllEmpresasQueryHandler : IQueryHandler<GetAllEmpresasQuery, IEnumerable<EmpresaDto>>
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IMapper _mapper;

    public GetAllEmpresasQueryHandler(IEmpresaRepository empresaRepository, IMapper mapper)
    {
        _empresaRepository = empresaRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<EmpresaDto>>> Handle(GetAllEmpresasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Usar specification para buscar apenas empresas ativas
            var activeCompaniesSpec = new EmpresaAtivaSpecification();
            
            var empresas = await _empresaRepository.GetAllAsync();
            var empresasAtivas = empresas.Where(activeCompaniesSpec.IsSatisfiedBy);

            var empresasDto = _mapper.Map<IEnumerable<EmpresaDto>>(empresasAtivas);
            
            return Result<IEnumerable<EmpresaDto>>.Success(empresasDto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EmpresaDto>>.Failure($"Erro ao buscar empresas: {ex.Message}");
        }
    }
}