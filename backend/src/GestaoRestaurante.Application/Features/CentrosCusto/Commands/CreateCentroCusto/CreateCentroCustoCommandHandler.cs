using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using FluentValidation;

namespace GestaoRestaurante.Application.Features.CentrosCusto.Commands.CreateCentroCusto;

public class CreateCentroCustoCommandHandler : ICommandHandler<CreateCentroCustoCommand, CentroCustoDto>
{
    private readonly ICentroCustoRepository _centroCustoRepository;
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCentroCustoDto> _validator;

    public CreateCentroCustoCommandHandler(
        ICentroCustoRepository centroCustoRepository,
        ISubAgrupamentoRepository subAgrupamentoRepository,
        IMapper mapper,
        IValidator<CreateCentroCustoDto> validator)
    {
        _centroCustoRepository = centroCustoRepository;
        _subAgrupamentoRepository = subAgrupamentoRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<CentroCustoDto>> Handle(CreateCentroCustoCommand request, CancellationToken cancellationToken)
    {
        // Create DTO for validation
        var createDto = new CreateCentroCustoDto
        {
            SubAgrupamentoId = request.SubAgrupamentoId,
            Codigo = request.Codigo,
            Nome = request.Nome,
            Descricao = request.Descricao
        };

        // FluentValidation
        var validationResult = await _validator.ValidateAsync(createDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<CentroCustoDto>.Failure(errors);
        }

        try
        {
            // Verify SubAgrupamento exists
            var subAgrupamento = await _subAgrupamentoRepository.GetByIdAsync(request.SubAgrupamentoId);
            if (subAgrupamento == null)
            {
                return Result<CentroCustoDto>.Failure(BusinessRuleMessages.SUBAGRUPAMENTO_NAO_ENCONTRADO);
            }

            // Check uniqueness by Codigo within the same SubAgrupamento
            var existingByCodigo = await _centroCustoRepository.GetByCodigoAsync(request.Codigo);
            if (existingByCodigo?.Any(cc => cc.SubAgrupamentoId == request.SubAgrupamentoId) == true)
            {
                return Result<CentroCustoDto>.Failure($"Já existe um centro de custo com código '{request.Codigo}' neste sub-agrupamento");
            }

            // Check uniqueness by Nome within the same SubAgrupamento
            var existingByNome = await _centroCustoRepository.GetByNomeAsync(request.Nome);
            if (existingByNome?.Any(cc => cc.SubAgrupamentoId == request.SubAgrupamentoId) == true)
            {
                return Result<CentroCustoDto>.Failure($"Já existe um centro de custo com nome '{request.Nome}' neste sub-agrupamento");
            }

            // Create entity
            var centroCusto = new CentroCusto
            {
                Id = Guid.NewGuid(),
                SubAgrupamentoId = request.SubAgrupamentoId,
                Codigo = request.Codigo,
                Nome = request.Nome,
                Descricao = request.Descricao,
                Ativa = true,
                DataCriacao = DateTime.Now
            };

            await _centroCustoRepository.AddAsync(centroCusto);

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