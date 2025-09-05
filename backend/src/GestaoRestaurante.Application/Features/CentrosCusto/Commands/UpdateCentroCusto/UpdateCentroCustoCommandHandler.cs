using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using FluentValidation;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Commands.UpdateCentroCusto;

public class UpdateCentroCustoCommandHandler : ICommandHandler<UpdateCentroCustoCommand, CentroCustoDto>
{
    private readonly ICentroCustoRepository _centroCustoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateCentroCustoDto> _validator;

    public UpdateCentroCustoCommandHandler(
        ICentroCustoRepository centroCustoRepository,
        IMapper mapper,
        IValidator<UpdateCentroCustoDto> validator)
    {
        _centroCustoRepository = centroCustoRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<CentroCustoDto>> Handle(UpdateCentroCustoCommand request, CancellationToken cancellationToken)
    {
        // Create DTO for validation
        var updateDto = new UpdateCentroCustoDto
        {
            Codigo = request.Codigo,
            Nome = request.Nome,
            Descricao = request.Descricao
        };

        // FluentValidation
        var validationResult = await _validator.ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<CentroCustoDto>.Failure(errors);
        }

        try
        {
            // Find existing centro de custo
            var centroCusto = await _centroCustoRepository.GetByIdAsync(request.Id);
            if (centroCusto == null)
            {
                return Result<CentroCustoDto>.Failure(BusinessRuleMessages.CENTRO_CUSTO_NAO_ENCONTRADO);
            }

            // Check uniqueness by Codigo within the same SubAgrupamento (excluding current)
            var existingByCodigo = await _centroCustoRepository.GetByCodigoAsync(request.Codigo);
            if (existingByCodigo?.Any(cc => cc.SubAgrupamentoId == centroCusto.SubAgrupamentoId && cc.Id != request.Id) == true)
            {
                return Result<CentroCustoDto>.Failure($"Já existe um centro de custo com código '{request.Codigo}' neste sub-agrupamento");
            }

            // Check uniqueness by Nome within the same SubAgrupamento (excluding current)
            var existingByNome = await _centroCustoRepository.GetByNomeAsync(request.Nome);
            if (existingByNome?.Any(cc => cc.SubAgrupamentoId == centroCusto.SubAgrupamentoId && cc.Id != request.Id) == true)
            {
                return Result<CentroCustoDto>.Failure($"Já existe um centro de custo com nome '{request.Nome}' neste sub-agrupamento");
            }

            // Update entity
            centroCusto.Codigo = request.Codigo;
            centroCusto.Nome = request.Nome;
            centroCusto.Descricao = request.Descricao;

            await _centroCustoRepository.UpdateAsync(centroCusto);

            var result = await _centroCustoRepository.GetByIdAsync(centroCusto.Id);
            var centroCustoDto = _mapper.Map<CentroCustoDto>(result);

            return Result<CentroCustoDto>.Success(centroCustoDto);
        }
        catch (Exception ex)
        {
            return Result<CentroCustoDto>.Failure($"Erro interno: {ex.Message}");
        }
    }
}