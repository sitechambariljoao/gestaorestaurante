using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Application.Common.Commands;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Domain.Common;
using GestaoRestaurante.Domain.Constants;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;

namespace GestaoRestaurante.Application.Features.Categorias.Commands.CreateCategoria;

public class CreateCategoriaCommandHandler : ICommandHandler<CreateCategoriaCommand, CategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ICentroCustoRepository _centroCustoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCategoriaDto> _validator;

    public CreateCategoriaCommandHandler(
        ICategoriaRepository categoriaRepository,
        ICentroCustoRepository centroCustoRepository,
        IMapper mapper,
        IValidator<CreateCategoriaDto> validator)
    {
        _categoriaRepository = categoriaRepository;
        _centroCustoRepository = centroCustoRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<CategoriaDto>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken)
    {
        // Create DTO for validation
        var createDto = new CreateCategoriaDto
        {
            CentroCustoId = request.CentroCustoId,
            CategoriaPaiId = request.CategoriaPaiId,
            Codigo = request.Codigo,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Nivel = request.Nivel
        };

        // FluentValidation
        var validationResult = await _validator.ValidateAsync(createDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result<CategoriaDto>.Failure(errors);
        }

        try
        {
            // Verify CentroCusto exists
            var centroCusto = await _centroCustoRepository.GetByIdAsync(request.CentroCustoId);
            if (centroCusto == null)
            {
                return Result<CategoriaDto>.Failure(BusinessRuleMessages.CENTRO_CUSTO_NAO_ENCONTRADO);
            }

            // Verify CategoriaPai if provided
            if (request.CategoriaPaiId.HasValue)
            {
                var categoriaPai = await _categoriaRepository.GetByIdAsync(request.CategoriaPaiId.Value);
                if (categoriaPai == null)
                {
                    return Result<CategoriaDto>.Failure("Categoria pai não encontrada");
                }

                if (categoriaPai.Nivel >= 3)
                {
                    return Result<CategoriaDto>.Failure("Categoria pai deve ter nível máximo de 2 para aceitar subcategorias");
                }
            }

            // Check uniqueness
            var existingByCodigo = await _categoriaRepository.GetByCodigoAsync(request.Codigo);
            if (existingByCodigo?.Any(c => c.CentroCustoId == request.CentroCustoId) == true)
            {
                return Result<CategoriaDto>.Failure($"Já existe uma categoria com código '{request.Codigo}' neste centro de custo");
            }

            var existingByNome = await _categoriaRepository.GetByNomeAsync(request.Nome);
            if (existingByNome?.Any(c => c.CentroCustoId == request.CentroCustoId) == true)
            {
                return Result<CategoriaDto>.Failure($"Já existe uma categoria com nome '{request.Nome}' neste centro de custo");
            }

            // Create entity
            var categoria = new Categoria
            {
                Id = Guid.NewGuid(),
                CentroCustoId = request.CentroCustoId,
                CategoriaPaiId = request.CategoriaPaiId,
                Codigo = request.Codigo,
                Nome = request.Nome,
                Descricao = request.Descricao,
                Nivel = request.Nivel,
                Ativa = true,
                DataCriacao = DateTime.Now
            };

            await _categoriaRepository.AddAsync(categoria);

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