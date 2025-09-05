using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Repositories;
using AutoMapper;
using FluentValidation;

namespace GestaoRestaurante.Application.Features.Categorias.Commands.UpdateCategoria;

public class UpdateCategoriaCommandHandler : ICommandHandler<UpdateCategoriaCommand, CategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateCategoriaDto> _validator;

    public UpdateCategoriaCommandHandler(
        ICategoriaRepository categoriaRepository,
        IMapper mapper,
        IValidator<UpdateCategoriaDto> validator)
    {
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<CategoriaDto>> Handle(UpdateCategoriaCommand request, CancellationToken cancellationToken)
    {
        // Create DTO for validation
        var updateDto = new UpdateCategoriaDto
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
            return Result<CategoriaDto>.Failure(errors);
        }

        try
        {
            // Find existing category
            var categoria = await _categoriaRepository.GetByIdAsync(request.Id);
            if (categoria == null)
            {
                return Result<CategoriaDto>.Failure(BusinessRuleMessages.CATEGORIA_NAO_ENCONTRADA);
            }

            // Check uniqueness (excluding current category)
            var existingByCodigo = await _categoriaRepository.GetByCodigoAsync(request.Codigo);
            if (existingByCodigo?.Any(c => c.CentroCustoId == categoria.CentroCustoId && c.Id != request.Id) == true)
            {
                return Result<CategoriaDto>.Failure($"Já existe uma categoria com código '{request.Codigo}' neste centro de custo");
            }

            var existingByNome = await _categoriaRepository.GetByNomeAsync(request.Nome);
            if (existingByNome?.Any(c => c.CentroCustoId == categoria.CentroCustoId && c.Id != request.Id) == true)
            {
                return Result<CategoriaDto>.Failure($"Já existe uma categoria com nome '{request.Nome}' neste centro de custo");
            }

            // Update entity
            categoria.Codigo = request.Codigo;
            categoria.Nome = request.Nome;
            categoria.Descricao = request.Descricao;

            await _categoriaRepository.UpdateAsync(categoria);

            var result = await _categoriaRepository.GetByIdAsync(categoria.Id);
            var categoriaDto = _mapper.Map<CategoriaDto>(result);

            return Result<CategoriaDto>.Success(categoriaDto);
        }
        catch (Exception ex)
        {
            return Result<CategoriaDto>.Failure($"Erro interno: {ex.Message}");
        }
    }
}