using GestaoRestaurante.Domain.Repositories;
using FluentValidation;
using GestaoRestaurante.Application.DTOs;
using GestaoRestaurante.Domain.Repositories;

namespace GestaoRestaurante.Application.Validators;

/// <summary>
/// Validadores customizados que fazem verificações no banco de dados
/// </summary>
public class CreateEmpresaDbValidator : AbstractValidator<CreateEmpresaDto>
{
    private readonly IEmpresaRepository _empresaRepository;

    public CreateEmpresaDbValidator(IEmpresaRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;

        RuleFor(x => x.Cnpj)
            .MustAsync(async (cnpj, cancellation) => !await _empresaRepository.ExistsByCnpjAsync(cnpj))
            .WithMessage("Já existe uma empresa com este CNPJ");

        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellation) => !await _empresaRepository.ExistsByEmailAsync(email))
            .WithMessage("Já existe uma empresa com este email");
    }
}

public class UpdateEmpresaDbValidator : AbstractValidator<UpdateEmpresaDto>
{
    private readonly IEmpresaRepository _empresaRepository;
    private Guid _excludeId;

    public UpdateEmpresaDbValidator(IEmpresaRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;

        RuleFor(x => x.Cnpj)
            .MustAsync(async (cnpj, cancellation) => !await _empresaRepository.ExistsByCnpjAsync(cnpj, _excludeId))
            .WithMessage("Já existe outra empresa com este CNPJ");

        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellation) => !await _empresaRepository.ExistsByEmailAsync(email, _excludeId))
            .WithMessage("Já existe outra empresa com este email");
    }

    public void SetExcludeId(Guid excludeId)
    {
        _excludeId = excludeId;
    }
}

public class CreateAgrupamentoDbValidator : AbstractValidator<CreateAgrupamentoDto>
{
    private readonly IAgrupamentoRepository _agrupamentoRepository;

    public CreateAgrupamentoDbValidator(IAgrupamentoRepository agrupamentoRepository)
    {
        _agrupamentoRepository = agrupamentoRepository;

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _agrupamentoRepository.ExistsByCodigoInEmpresaAsync(dto.FilialId, dto.Codigo))
            .WithMessage("Já existe um agrupamento com este código nesta filial")
            .WithName("Codigo");

        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) => !await _agrupamentoRepository.ExistsByNomeInEmpresaAsync(dto.FilialId, dto.Nome))
            .WithMessage("Já existe um agrupamento com este nome nesta filial")
            .WithName("Nome");
    }
}

public class UpdateAgrupamentoDbValidator : AbstractValidator<UpdateAgrupamentoDto>
{
    private readonly IAgrupamentoRepository _agrupamentoRepository;
    private Guid _excludeId;
    private Guid _filialId;

    public UpdateAgrupamentoDbValidator(IAgrupamentoRepository agrupamentoRepository)
    {
        _agrupamentoRepository = agrupamentoRepository;

        RuleFor(x => x.Codigo)
            .MustAsync(async (codigo, cancellation) => !await _agrupamentoRepository.ExistsByCodigoInEmpresaAsync(_filialId, codigo, _excludeId))
            .WithMessage("Já existe outro agrupamento com este código nesta filial");

        RuleFor(x => x.Nome)
            .MustAsync(async (nome, cancellation) => !await _agrupamentoRepository.ExistsByNomeInEmpresaAsync(_filialId, nome, _excludeId))
            .WithMessage("Já existe outro agrupamento com este nome nesta filial");
    }

    public void SetContext(Guid excludeId, Guid filialId)
    {
        _excludeId = excludeId;
        _filialId = filialId;
    }
}

public class CreateFilialDbValidator : AbstractValidator<CreateFilialDto>
{
    private readonly IFilialRepository _filialRepository;

    public CreateFilialDbValidator(IFilialRepository filialRepository)
    {
        _filialRepository = filialRepository;

        RuleFor(x => x.Cnpj)
            .MustAsync(async (cnpj, cancellation) => !await _filialRepository.ExistsByCnpjAsync(cnpj))
            .WithMessage("Já existe uma filial com este CNPJ")
            .When(x => !string.IsNullOrEmpty(x.Cnpj));

        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellation) => !await _filialRepository.ExistsByEmailAsync(email))
            .WithMessage("Já existe uma filial com este email")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class UpdateFilialDbValidator : AbstractValidator<UpdateFilialDto>
{
    private readonly IFilialRepository _filialRepository;
    private Guid _excludeId;

    public UpdateFilialDbValidator(IFilialRepository filialRepository)
    {
        _filialRepository = filialRepository;

        RuleFor(x => x.Cnpj)
            .MustAsync(async (cnpj, cancellation) => !await _filialRepository.ExistsByCnpjAsync(cnpj, _excludeId))
            .WithMessage("Já existe outra filial com este CNPJ")
            .When(x => !string.IsNullOrEmpty(x.Cnpj));

        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellation) => !await _filialRepository.ExistsByEmailAsync(email, _excludeId))
            .WithMessage("Já existe outra filial com este email")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }

    public void SetExcludeId(Guid excludeId)
    {
        _excludeId = excludeId;
    }
}