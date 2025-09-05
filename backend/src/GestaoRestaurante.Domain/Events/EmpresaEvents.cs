namespace GestaoRestaurante.Domain.Events;

/// <summary>
/// Evento disparado quando uma nova empresa é criada
/// </summary>
public class EmpresaCriadaEvent : DomainEvent
{
    public Guid EmpresaId { get; }
    public string RazaoSocial { get; }
    public string NomeFantasia { get; }
    public string Cnpj { get; }
    public string Email { get; }

    public EmpresaCriadaEvent(Guid empresaId, string razaoSocial, string nomeFantasia, string cnpj, string email)
    {
        EmpresaId = empresaId;
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Cnpj = cnpj;
        Email = email;
    }
}

/// <summary>
/// Evento disparado quando uma empresa é atualizada
/// </summary>
public class EmpresaAtualizadaEvent : DomainEvent
{
    public Guid EmpresaId { get; }
    public string RazaoSocial { get; }
    public string NomeFantasia { get; }
    public Dictionary<string, object> ChangedFields { get; }

    public EmpresaAtualizadaEvent(Guid empresaId, string razaoSocial, string nomeFantasia, Dictionary<string, object> changedFields)
    {
        EmpresaId = empresaId;
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        ChangedFields = changedFields;
    }
}

/// <summary>
/// Evento disparado quando uma empresa é inativada
/// </summary>
public class EmpresaInativadaEvent : DomainEvent
{
    public Guid EmpresaId { get; }
    public string RazaoSocial { get; }
    public string Motivo { get; }

    public EmpresaInativadaEvent(Guid empresaId, string razaoSocial, string motivo = "")
    {
        EmpresaId = empresaId;
        RazaoSocial = razaoSocial;
        Motivo = motivo;
    }
}

/// <summary>
/// Evento disparado quando uma empresa é reativada
/// </summary>
public class EmpresaReativadaEvent : DomainEvent
{
    public Guid EmpresaId { get; }
    public string RazaoSocial { get; }

    public EmpresaReativadaEvent(Guid empresaId, string razaoSocial)
    {
        EmpresaId = empresaId;
        RazaoSocial = razaoSocial;
    }
}