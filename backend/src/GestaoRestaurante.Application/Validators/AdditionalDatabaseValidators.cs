using GestaoRestaurante.Domain.Repositories;
using FluentValidation;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Repositories;

namespace GestaoRestaurante.Application.Validators;

/// <summary>
/// Validadores customizados adicionais que fazem verificações no banco de dados
/// </summary>
public class CreateSubAgrupamentoDbValidator : AbstractValidator<CreateSubAgrupamentoDto>
{
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;

    public CreateSubAgrupamentoDbValidator(ISubAgrupamentoRepository subAgrupamentoRepository)
    {
        _subAgrupamentoRepository = subAgrupamentoRepository;

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _subAgrupamentoRepository.ExistsByCodigoInAgrupamentoAsync(dto.AgrupamentoId, dto.Codigo))
            .WithMessage("Já existe um sub-agrupamento com este código neste agrupamento")
            .WithName("Codigo");

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _subAgrupamentoRepository.ExistsByNomeInAgrupamentoAsync(dto.AgrupamentoId, dto.Nome))
            .WithMessage("Já existe um sub-agrupamento com este nome neste agrupamento")
            .WithName("Nome");
    }
}

public class UpdateSubAgrupamentoDbValidator : AbstractValidator<UpdateSubAgrupamentoDto>
{
    private readonly ISubAgrupamentoRepository _subAgrupamentoRepository;
    private Guid _excludeId;
    private Guid _agrupamentoId;

    public UpdateSubAgrupamentoDbValidator(ISubAgrupamentoRepository subAgrupamentoRepository)
    {
        _subAgrupamentoRepository = subAgrupamentoRepository;

        RuleFor(x => x.Codigo)
            .MustAsync(async (codigo, cancellation) => !await _subAgrupamentoRepository.ExistsByCodigoInAgrupamentoAsync(_agrupamentoId, codigo, _excludeId))
            .WithMessage("Já existe outro sub-agrupamento com este código neste agrupamento");

        RuleFor(x => x.Nome)
            .MustAsync(async (nome, cancellation) => !await _subAgrupamentoRepository.ExistsByNomeInAgrupamentoAsync(_agrupamentoId, nome, _excludeId))
            .WithMessage("Já existe outro sub-agrupamento com este nome neste agrupamento");
    }

    public void SetContext(Guid excludeId, Guid agrupamentoId)
    {
        _excludeId = excludeId;
        _agrupamentoId = agrupamentoId;
    }
}

public class CreateCentroCustoDbValidator : AbstractValidator<CreateCentroCustoDto>
{
    private readonly ICentroCustoRepository _centroCustoRepository;

    public CreateCentroCustoDbValidator(ICentroCustoRepository centroCustoRepository)
    {
        _centroCustoRepository = centroCustoRepository;

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _centroCustoRepository.ExistsByCodigoInSubAgrupamentoAsync(dto.SubAgrupamentoId, dto.Codigo))
            .WithMessage("Já existe um centro de custo com este código neste sub-agrupamento")
            .WithName("Codigo");

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _centroCustoRepository.ExistsByNomeInSubAgrupamentoAsync(dto.SubAgrupamentoId, dto.Nome))
            .WithMessage("Já existe um centro de custo com este nome neste sub-agrupamento")
            .WithName("Nome");
    }
}

public class UpdateCentroCustoDbValidator : AbstractValidator<UpdateCentroCustoDto>
{
    private readonly ICentroCustoRepository _centroCustoRepository;
    private Guid _excludeId;
    private Guid _subAgrupamentoId;

    public UpdateCentroCustoDbValidator(ICentroCustoRepository centroCustoRepository)
    {
        _centroCustoRepository = centroCustoRepository;

        RuleFor(x => x.Codigo)
            .MustAsync(async (codigo, cancellation) => !await _centroCustoRepository.ExistsByCodigoInSubAgrupamentoAsync(_subAgrupamentoId, codigo, _excludeId))
            .WithMessage("Já existe outro centro de custo com este código neste sub-agrupamento");

        RuleFor(x => x.Nome)
            .MustAsync(async (nome, cancellation) => !await _centroCustoRepository.ExistsByNomeInSubAgrupamentoAsync(_subAgrupamentoId, nome, _excludeId))
            .WithMessage("Já existe outro centro de custo com este nome neste sub-agrupamento");
    }

    public void SetContext(Guid excludeId, Guid subAgrupamentoId)
    {
        _excludeId = excludeId;
        _subAgrupamentoId = subAgrupamentoId;
    }
}

public class CreateCategoriaDbValidator : AbstractValidator<CreateCategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CreateCategoriaDbValidator(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _categoriaRepository.ExistsByCodigoInCentroCustoAsync(dto.CentroCustoId, dto.Codigo))
            .WithMessage("Já existe uma categoria com este código neste centro de custo")
            .WithName("Codigo");

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _categoriaRepository.ExistsByNomeInCentroCustoAsync(dto.CentroCustoId, dto.Nome))
            .WithMessage("Já existe uma categoria com este nome neste centro de custo")
            .WithName("Nome");
    }
}

public class UpdateCategoriaDbValidator : AbstractValidator<UpdateCategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private Guid _excludeId;
    private Guid _centroCustoId;

    public UpdateCategoriaDbValidator(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;

        RuleFor(x => x.Codigo)
            .MustAsync(async (codigo, cancellation) => !await _categoriaRepository.ExistsByCodigoInCentroCustoAsync(_centroCustoId, codigo, _excludeId))
            .WithMessage("Já existe outra categoria com este código neste centro de custo");

        RuleFor(x => x.Nome)
            .MustAsync(async (nome, cancellation) => !await _categoriaRepository.ExistsByNomeInCentroCustoAsync(_centroCustoId, nome, _excludeId))
            .WithMessage("Já existe outra categoria com este nome neste centro de custo");
    }

    public void SetContext(Guid excludeId, Guid centroCustoId)
    {
        _excludeId = excludeId;
        _centroCustoId = centroCustoId;
    }
}

public class CreateProdutoDbValidator : AbstractValidator<CreateProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;

    public CreateProdutoDbValidator(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;

        RuleFor(x => x.Codigo)
            .MustAsync(async (codigo, cancellation) => !await _produtoRepository.ExistsByCodigoAsync(codigo))
            .WithMessage("Já existe um produto com este código");

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _produtoRepository.ExistsByNomeInCategoriaAsync(dto.CategoriaId, dto.Nome))
            .WithMessage("Já existe um produto com este nome nesta categoria")
            .WithName("Nome");
    }
}

public class UpdateProdutoDbValidator : AbstractValidator<UpdateProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private Guid _excludeId;
    private Guid _categoriaId;

    public UpdateProdutoDbValidator(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;

        RuleFor(x => x.Codigo)
            .MustAsync(async (codigo, cancellation) => !await _produtoRepository.ExistsByCodigoAsync(codigo, _excludeId))
            .WithMessage("Já existe outro produto com este código");

        RuleFor(x => x.Nome)
            .MustAsync(async (nome, cancellation) => !await _produtoRepository.ExistsByNomeInCategoriaAsync(_categoriaId, nome, _excludeId))
            .WithMessage("Já existe outro produto com este nome nesta categoria");
    }

    public void SetContext(Guid excludeId, Guid categoriaId)
    {
        _excludeId = excludeId;
        _categoriaId = categoriaId;
    }
}