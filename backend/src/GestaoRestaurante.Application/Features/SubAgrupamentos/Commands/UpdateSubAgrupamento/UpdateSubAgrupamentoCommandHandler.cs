using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using FluentValidation;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.UpdateSubAgrupamento;

public class UpdateSubAgrupamentoCommandHandler : ICommandHandler<UpdateSubAgrupamentoCommand, SubAgrupamentoDto>
{
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateSubAgrupamentoDto> _validator;

    public UpdateSubAgrupamentoCommandHandler(
        ISubAgrupamentoRepository subAgrupamentoRepository,
        IMapper mapper,
        IValidator<UpdateSubAgrupamentoDto> validator)
    {
        _subAgrupamentoRepository = subAgrupamentoRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<SubAgrupamentoDto>> Handle(UpdateSubAgrupamentoCommand request, CancellationToken cancellationToken)
    {
        // Create DTO for validation
        var updateDto = new UpdateSubAgrupamentoDto
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
            return Result<SubAgrupamentoDto>.Failure(errors);
        }

        try
        {
            // Find existing sub-agrupamento
            var subAgrupamento = await _subAgrupamentoRepository.GetByIdAsync(request.Id);
            if (subAgrupamento == null)
            {
                return Result<SubAgrupamentoDto>.Failure(BusinessRuleMessages.SUBAGRUPAMENTO_NAO_ENCONTRADO);
            }

            // Check uniqueness by Codigo within the same Agrupamento (excluding current)
            var existingByCodigo = await _subAgrupamentoRepository.GetByCodigoAsync(request.Codigo);
            if (existingByCodigo?.Any(s => s.AgrupamentoId == subAgrupamento.AgrupamentoId && s.Id != request.Id) == true)
            {
                return Result<SubAgrupamentoDto>.Failure($"Já existe um sub-agrupamento com código '{request.Codigo}' neste agrupamento");
            }

            // Check uniqueness by Nome within the same Agrupamento (excluding current)
            var existingByNome = await _subAgrupamentoRepository.GetByNomeAsync(request.Nome);
            if (existingByNome?.Any(s => s.AgrupamentoId == subAgrupamento.AgrupamentoId && s.Id != request.Id) == true)
            {
                return Result<SubAgrupamentoDto>.Failure($"Já existe um sub-agrupamento com nome '{request.Nome}' neste agrupamento");
            }

            // Update entity
            subAgrupamento.Codigo = request.Codigo;
            subAgrupamento.Nome = request.Nome;
            subAgrupamento.Descricao = request.Descricao;

            await _subAgrupamentoRepository.UpdateAsync(subAgrupamento);

            var result = await _subAgrupamentoRepository.GetByIdAsync(subAgrupamento.Id);
            var subAgrupamentoDto = _mapper.Map<SubAgrupamentoDto>(result);

            return Result<SubAgrupamentoDto>.Success(subAgrupamentoDto);
        }
        catch (Exception ex)
        {
            return Result<SubAgrupamentoDto>.Failure($"Erro interno: {ex.Message}");
        }
    }
}