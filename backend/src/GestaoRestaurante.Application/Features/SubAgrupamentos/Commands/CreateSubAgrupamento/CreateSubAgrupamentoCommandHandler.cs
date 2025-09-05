using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using FluentValidation;

namespace GestaoRestaurante.Application.Features.SubAgrupamentos.Commands.CreateSubAgrupamento;

public class CreateSubAgrupamentoCommandHandler : ICommandHandler<CreateSubAgrupamentoCommand, SubAgrupamentoDto>
{
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;
    private readonly IAgrupamentoRepository _agrupamentoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateSubAgrupamentoDto> _validator;

    public CreateSubAgrupamentoCommandHandler(
        ISubAgrupamentoRepository subAgrupamentoRepository,
        IAgrupamentoRepository agrupamentoRepository,
        IMapper mapper,
        IValidator<CreateSubAgrupamentoDto> validator)
    {
        _subAgrupamentoRepository = subAgrupamentoRepository;
        _agrupamentoRepository = agrupamentoRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<SubAgrupamentoDto>> Handle(CreateSubAgrupamentoCommand request, CancellationToken cancellationToken)
    {
        // Create DTO for validation
        var createDto = new CreateSubAgrupamentoDto
        {
            AgrupamentoId = request.AgrupamentoId,
            Codigo = request.Codigo,
            Nome = request.Nome,
            Descricao = request.Descricao
        };

        // FluentValidation
        var validationResult = await _validator.ValidateAsync(createDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<SubAgrupamentoDto>.Failure(errors);
        }

        try
        {
            // Verify Agrupamento exists
            var agrupamento = await _agrupamentoRepository.GetByIdAsync(request.AgrupamentoId);
            if (agrupamento == null)
            {
                return Result<SubAgrupamentoDto>.Failure(BusinessRuleMessages.AGRUPAMENTO_NAO_ENCONTRADO);
            }

            // Check uniqueness by Codigo within the same Agrupamento
            var existingByCodigo = await _subAgrupamentoRepository.GetByCodigoAsync(request.Codigo);
            if (existingByCodigo?.Any(s => s.AgrupamentoId == request.AgrupamentoId) == true)
            {
                return Result<SubAgrupamentoDto>.Failure($"Já existe um sub-agrupamento com código '{request.Codigo}' neste agrupamento");
            }

            // Check uniqueness by Nome within the same Agrupamento
            var existingByNome = await _subAgrupamentoRepository.GetByNomeAsync(request.Nome);
            if (existingByNome?.Any(s => s.AgrupamentoId == request.AgrupamentoId) == true)
            {
                return Result<SubAgrupamentoDto>.Failure($"Já existe um sub-agrupamento com nome '{request.Nome}' neste agrupamento");
            }

            // Create entity
            var subAgrupamento = new SubAgrupamento
            {
                Id = Guid.NewGuid(),
                AgrupamentoId = request.AgrupamentoId,
                Codigo = request.Codigo,
                Nome = request.Nome,
                Descricao = request.Descricao,
                Ativa = true,
                DataCriacao = DateTime.Now
            };

            await _subAgrupamentoRepository.AddAsync(subAgrupamento);

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