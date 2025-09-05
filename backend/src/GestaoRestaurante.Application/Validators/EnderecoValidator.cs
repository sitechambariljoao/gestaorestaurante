using FluentValidation;
using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Validators;

public class EnderecoValidator : AbstractValidator<EnderecoDto>
{
    public EnderecoValidator()
    {
        RuleFor(x => x.Logradouro)
            .NotEmpty().WithMessage("Logradouro é obrigatório")
            .MaximumLength(255).WithMessage("Logradouro deve ter no máximo 255 caracteres");

        RuleFor(x => x.Numero)
            .NotEmpty().WithMessage("Número é obrigatório")
            .MaximumLength(10).WithMessage("Número deve ter no máximo 10 caracteres");

        RuleFor(x => x.Bairro)
            .NotEmpty().WithMessage("Bairro é obrigatório")
            .MaximumLength(100).WithMessage("Bairro deve ter no máximo 100 caracteres");

        RuleFor(x => x.Cidade)
            .NotEmpty().WithMessage("Cidade é obrigatória")
            .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("Estado é obrigatório")
            .Length(2).WithMessage("Estado deve ter exatamente 2 caracteres")
            .Matches(@"^[A-Z]{2}$").WithMessage("Estado deve estar em maiúsculas (ex: SP, RJ)");

        RuleFor(x => x.Cep)
            .NotEmpty().WithMessage("CEP é obrigatório")
            .Matches(@"^\d{5}-?\d{3}$").WithMessage("CEP deve ter formato válido (xxxxx-xxx)");

        RuleFor(x => x.Complemento)
            .MaximumLength(255).WithMessage("Complemento deve ter no máximo 255 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Complemento));
    }
}