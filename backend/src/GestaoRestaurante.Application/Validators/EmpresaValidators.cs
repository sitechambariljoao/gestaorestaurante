using FluentValidation;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Validators;

public class CreateEmpresaDtoValidator : AbstractValidator<CreateEmpresaDto>
{
    public CreateEmpresaDtoValidator()
    {
        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão social é obrigatória")
            .MaximumLength(255).WithMessage("Razão social deve ter no máximo 255 caracteres")
            .MinimumLength(2).WithMessage("Razão social deve ter pelo menos 2 caracteres");

        RuleFor(x => x.NomeFantasia)
            .NotEmpty().WithMessage("Nome fantasia é obrigatório")
            .MaximumLength(255).WithMessage("Nome fantasia deve ter no máximo 255 caracteres")
            .MinimumLength(2).WithMessage("Nome fantasia deve ter pelo menos 2 caracteres");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ é obrigatório")
            .Length(14, 18).WithMessage("CNPJ deve ter entre 14 e 18 caracteres")
            .Matches(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$").WithMessage("CNPJ deve ter formato válido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefone));

        RuleFor(x => x.Endereco)
            .SetValidator(new EnderecoValidator())
            .When(x => x.Endereco != null);
    }
}

public class UpdateEmpresaDtoValidator : AbstractValidator<UpdateEmpresaDto>
{
    public UpdateEmpresaDtoValidator()
    {
        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão social é obrigatória")
            .MaximumLength(255).WithMessage("Razão social deve ter no máximo 255 caracteres")
            .MinimumLength(2).WithMessage("Razão social deve ter pelo menos 2 caracteres");

        RuleFor(x => x.NomeFantasia)
            .NotEmpty().WithMessage("Nome fantasia é obrigatório")
            .MaximumLength(255).WithMessage("Nome fantasia deve ter no máximo 255 caracteres")
            .MinimumLength(2).WithMessage("Nome fantasia deve ter pelo menos 2 caracteres");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ é obrigatório")
            .Length(14, 18).WithMessage("CNPJ deve ter entre 14 e 18 caracteres")
            .Matches(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$").WithMessage("CNPJ deve ter formato válido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter formato válido")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres");

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefone));

        RuleFor(x => x.Endereco)
            .SetValidator(new EnderecoValidator())
            .When(x => x.Endereco != null);
    }
}