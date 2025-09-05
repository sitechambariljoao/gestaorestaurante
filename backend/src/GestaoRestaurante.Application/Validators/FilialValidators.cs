using FluentValidation;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Validators;

public class CreateFilialDtoValidator : AbstractValidator<CreateFilialDto>
{
    public CreateFilialDtoValidator()
    {
        RuleFor(x => x.EmpresaId)
            .NotEmpty().WithMessage("ID da empresa é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres");

        RuleFor(x => x.Cnpj)
            .Length(14, 18).WithMessage("CNPJ deve ter entre 14 e 18 caracteres")
            .Matches(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$").WithMessage("CNPJ deve ter formato válido")
            .When(x => !string.IsNullOrEmpty(x.Cnpj));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefone));

        RuleFor(x => x.Endereco)
            .SetValidator(new EnderecoValidator())
            .When(x => x.Endereco != null);
    }
}

public class UpdateFilialDtoValidator : AbstractValidator<UpdateFilialDto>
{
    public UpdateFilialDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres");

        RuleFor(x => x.Cnpj)
            .Length(14, 18).WithMessage("CNPJ deve ter entre 14 e 18 caracteres")
            .Matches(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$").WithMessage("CNPJ deve ter formato válido")
            .When(x => !string.IsNullOrEmpty(x.Cnpj));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefone));

        RuleFor(x => x.Endereco)
            .SetValidator(new EnderecoValidator())
            .When(x => x.Endereco != null);
    }
}