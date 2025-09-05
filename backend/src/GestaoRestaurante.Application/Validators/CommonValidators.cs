using FluentValidation;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Validators;

// SubAgrupamento Validators
public class CreateSubAgrupamentoDtoValidator : AbstractValidator<CreateSubAgrupamentoDto>
{
    public CreateSubAgrupamentoDtoValidator()
    {
        RuleFor(x => x.AgrupamentoId)
            .NotEmpty().WithMessage("ID do agrupamento é obrigatório");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}

public class UpdateSubAgrupamentoDtoValidator : AbstractValidator<UpdateSubAgrupamentoDto>
{
    public UpdateSubAgrupamentoDtoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}

// CentroCusto Validators
public class CreateCentroCustoDtoValidator : AbstractValidator<CreateCentroCustoDto>
{
    public CreateCentroCustoDtoValidator()
    {
        RuleFor(x => x.SubAgrupamentoId)
            .NotEmpty().WithMessage("ID do sub-agrupamento é obrigatório");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}

public class UpdateCentroCustoDtoValidator : AbstractValidator<UpdateCentroCustoDto>
{
    public UpdateCentroCustoDtoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}

// Categoria Validators
public class CreateCategoriaDtoValidator : AbstractValidator<CreateCategoriaDto>
{
    public CreateCategoriaDtoValidator()
    {
        RuleFor(x => x.CentroCustoId)
            .NotEmpty().WithMessage("ID do centro de custo é obrigatório");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Nivel)
            .InclusiveBetween(1, 3).WithMessage("Nível deve ser 1, 2 ou 3");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}

public class UpdateCategoriaDtoValidator : AbstractValidator<UpdateCategoriaDto>
{
    public UpdateCategoriaDtoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}

// Produto Validators
public class CreateProdutoDtoValidator : AbstractValidator<CreateProdutoDto>
{
    public CreateProdutoDtoValidator()
    {
        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("ID da categoria é obrigatório");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(50).WithMessage("Código deve ter no máximo 50 caracteres")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Código deve conter apenas letras maiúsculas, números e hífen");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero");

        RuleFor(x => x.UnidadeMedida)
            .NotEmpty().WithMessage("Unidade de medida é obrigatória")
            .MaximumLength(10).WithMessage("Unidade de medida deve ter no máximo 10 caracteres");
    }
}

public class UpdateProdutoDtoValidator : AbstractValidator<UpdateProdutoDto>
{
    public UpdateProdutoDtoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(50).WithMessage("Código deve ter no máximo 50 caracteres")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Código deve conter apenas letras maiúsculas, números e hífen");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero");

        // TODO: Implementar quando módulo financeiro for criado
        // RuleFor(x => x.PrecoVenda)
        //     .GreaterThan(0).WithMessage("Preço de venda deve ser maior que zero")
        //     .When(x => x.PrecoVenda.HasValue);
        //
        // RuleFor(x => x.PrecoCusto)
        //     .GreaterThan(0).WithMessage("Preço de custo deve ser maior que zero")
        //     .When(x => x.PrecoCusto.HasValue);

        RuleFor(x => x.UnidadeMedida)
            .NotEmpty().WithMessage("Unidade de medida é obrigatória")
            .MaximumLength(10).WithMessage("Unidade de medida deve ter no máximo 10 caracteres");
    }
}