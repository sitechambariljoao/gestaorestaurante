using FluentValidation;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Validators;

public class CreateAgrupamentoDtoValidator : AbstractValidator<CreateAgrupamentoDto>
{
    public CreateAgrupamentoDtoValidator()
    {
        RuleFor(x => x.FilialId)
            .NotEmpty().WithMessage("ID da filial é obrigatório");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}

public class UpdateAgrupamentoDtoValidator : AbstractValidator<UpdateAgrupamentoDto>
{
    public UpdateAgrupamentoDtoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório")
            .MaximumLength(20).WithMessage("Código deve ter no máximo 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Código deve conter apenas letras maiúsculas e números");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}