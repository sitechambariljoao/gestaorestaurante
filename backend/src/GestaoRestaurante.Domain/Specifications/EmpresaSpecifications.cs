using System.Linq.Expressions;
using GestaoRestaurante.Domain.Entities;

namespace GestaoRestaurante.Domain.Specifications;

/// <summary>
/// Especificação para filtrar empresas ativas
/// </summary>
public class EmpresaAtivaSpecification : Specification<Empresa>
{
    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.Ativa;
    }
}

/// <summary>
/// Especificação para filtrar empresas por CNPJ
/// </summary>
public class EmpresaPorCnpjSpecification : Specification<Empresa>
{
    private readonly string _cnpj;

    public EmpresaPorCnpjSpecification(string cnpj)
    {
        _cnpj = cnpj;
    }

    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.Cnpj == _cnpj;
    }
}

/// <summary>
/// Especificação para filtrar empresas por email
/// </summary>
public class EmpresaPorEmailSpecification : Specification<Empresa>
{
    private readonly string _email;

    public EmpresaPorEmailSpecification(string email)
    {
        _email = email.ToLowerInvariant();
    }

    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.Email.ToLower() == _email;
    }
}

/// <summary>
/// Especificação para filtrar empresas por nome fantasia (busca parcial)
/// </summary>
public class EmpresaPorNomeFantasiaSpecification : Specification<Empresa>
{
    private readonly string _nomeFantasia;

    public EmpresaPorNomeFantasiaSpecification(string nomeFantasia)
    {
        _nomeFantasia = nomeFantasia.ToLowerInvariant();
    }

    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.NomeFantasia.ToLower().Contains(_nomeFantasia);
    }
}

/// <summary>
/// Especificação para filtrar empresas que possuem filiais ativas
/// </summary>
public class EmpresaComFiliaisAtivasSpecification : Specification<Empresa>
{
    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.Filiais.Any(f => f.Ativa);
    }
}

/// <summary>
/// Especificação para filtrar empresas por estado
/// </summary>
public class EmpresaPorEstadoSpecification : Specification<Empresa>
{
    private readonly string _estado;

    public EmpresaPorEstadoSpecification(string estado)
    {
        _estado = estado.ToUpperInvariant();
    }

    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.Endereco.Estado.ToUpper() == _estado;
    }
}

/// <summary>
/// Especificação para filtrar empresas por cidade
/// </summary>
public class EmpresaPorCidadeSpecification : Specification<Empresa>
{
    private readonly string _cidade;

    public EmpresaPorCidadeSpecification(string cidade)
    {
        _cidade = cidade.ToLowerInvariant();
    }

    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.Endereco.Cidade.ToLower().Contains(_cidade);
    }
}

/// <summary>
/// Especificação para filtrar empresas criadas em um período
/// </summary>
public class EmpresaCriadaEntreDatasSpecification : Specification<Empresa>
{
    private readonly DateTime _dataInicio;
    private readonly DateTime _dataFim;

    public EmpresaCriadaEntreDatasSpecification(DateTime dataInicio, DateTime dataFim)
    {
        _dataInicio = dataInicio.Date;
        _dataFim = dataFim.Date.AddDays(1).AddTicks(-1); // Inclui o dia todo
    }

    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.DataCriacao >= _dataInicio && empresa.DataCriacao <= _dataFim;
    }
}

/// <summary>
/// Especificação para filtrar empresas com assinatura ativa
/// </summary>
public class EmpresaComAssinaturaAtivaSpecification : Specification<Empresa>
{
    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.AssinaturaAtiva != null && 
                         empresa.AssinaturaAtiva.Ativa &&
                         empresa.AssinaturaAtiva.DataVencimento > DateTime.UtcNow;
    }
}

/// <summary>
/// Especificação para filtrar empresas por plano de assinatura
/// </summary>
public class EmpresaPorPlanoAssinaturaSpecification : Specification<Empresa>
{
    private readonly string _nomePlano;

    public EmpresaPorPlanoAssinaturaSpecification(string nomePlano)
    {
        _nomePlano = nomePlano;
    }

    public override Expression<Func<Empresa, bool>> ToExpression()
    {
        return empresa => empresa.AssinaturaAtiva != null && 
                         empresa.AssinaturaAtiva.Plano.Nome == _nomePlano;
    }
}