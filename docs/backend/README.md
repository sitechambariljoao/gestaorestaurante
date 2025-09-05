# 🏗️ Backend - Sistema ERP Restaurantes

Documentação técnica do backend desenvolvido em .NET 9.0 com Clean Architecture, Domain-Driven Design e CQRS Pattern.

## 📐 **Arquitetura Clean Architecture + DDD + CQRS**

### **Visão Geral da Arquitetura**
```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                   │
│  ┌─────────────────────────────────────────────────────┤
│  │               GestaoRestaurante.API                 │
│  │  • Controllers (REST Endpoints)                     │
│  │  • Filters (Validation, Logging, Performance)      │
│  │  • Middlewares (Exception Handling)                │
│  │  • Authorization (JWT + Module-based)              │
│  │  • Swagger Configuration                            │
│  └─────────────────────────────────────────────────────┤
├─────────────────────────────────────────────────────────┤
│                   Application Layer                     │
│  ┌─────────────────────────────────────────────────────┤
│  │            GestaoRestaurante.Application            │
│  │  • CQRS (Commands & Queries + Handlers)            │
│  │  • DTOs (Data Transfer Objects)                     │
│  │  • Services (Business Logic Coordination)          │
│  │  • Validators (FluentValidation)                   │
│  │  • Mappings (AutoMapper Profiles)                  │
│  │  • Domain Events Handlers                          │
│  └─────────────────────────────────────────────────────┤
├─────────────────────────────────────────────────────────┤
│                     Domain Layer                        │
│  ┌─────────────────────────────────────────────────────┤
│  │              GestaoRestaurante.Domain               │
│  │  • Entities (Aggregate Roots)                      │
│  │  • Value Objects (Email, CNPJ, etc.)               │
│  │  • Domain Services (Complex Business Rules)        │
│  │  • Domain Events (EmpresaCriada, etc.)             │
│  │  • Specifications (Query Logic)                    │
│  │  • Repository Interfaces                           │
│  └─────────────────────────────────────────────────────┤
├─────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                   │
│  ┌─────────────────────────────────────────────────────┤
│  │           GestaoRestaurante.Infrastructure          │
│  │  • EF Core DbContext + Configurations              │
│  │  • Repository Implementations                      │
│  │  • External Services (Email, Storage)              │
│  │  • Cache Services (Memory, Redis)                  │
│  │  • Performance Profiling                           │
│  └─────────────────────────────────────────────────────┤
└─────────────────────────────────────────────────────────┘
```

## 🎯 **Projetos e Responsabilidades**

### **GestaoRestaurante.API** (Presentation Layer)
**Responsabilidade**: Interface de entrada, controllers REST, configurações

**Componentes Principais**:
- **Controllers**: Endpoints REST com documentação Swagger
- **Filters**: 
  - `ValidationActionFilter` - Validação automática
  - `LoggingActionFilter` - Logging estruturado  
  - `PerformanceActionFilter` - Monitoramento performance
  - `ResponseWrapperFilter` - Padronização respostas
- **Middlewares**:
  - `GlobalExceptionHandlerMiddleware` - Tratamento global exceções
- **Authorization**:
  - `ModuleAuthorization` - Autorização por módulos
  - JWT Bearer Authentication
- **Health Checks**: Database, cache, system, external services

### **GestaoRestaurante.Application** (Application Layer)
**Responsabilidade**: Casos de uso, orquestração, coordenação

**CQRS Implementation**:
```csharp
// Commands (Write Operations)
public record CreateEmpresaCommand(CreateEmpresaDto EmpresaDto) : ICommand<Result<EmpresaDto>>;
public class CreateEmpresaCommandHandler : ICommandHandler<CreateEmpresaCommand, Result<EmpresaDto>>

// Queries (Read Operations)  
public record GetAllEmpresasQuery : IQuery<Result<IEnumerable<EmpresaDto>>>;
public class GetAllEmpresasQueryHandler : IQueryHandler<GetAllEmpresasQuery, Result<IEnumerable<EmpresaDto>>>
```

**Componentes Principais**:
- **DTOs**: Data Transfer Objects para entrada/saída
- **Services**: Coordenação de casos de uso complexos
- **Validators**: FluentValidation para DTOs + Database validators
- **Mappings**: AutoMapper profiles para conversão entidade ↔ DTO
- **Events**: Handlers para Domain Events

### **GestaoRestaurante.Domain** (Domain Layer)
**Responsabilidade**: Lógica de negócio pura, regras de domínio

**Entities (Aggregate Roots)**:
```csharp
public class Empresa : BaseEntity, IAggregateRoot
{
    public CNPJ Cnpj { get; private set; }
    public Email Email { get; private set; }
    
    // Domain Methods
    public void Reativar() { /* business logic */ }
    public void AlterarEmail(Email novoEmail) { /* validation + events */ }
}
```

**Value Objects**:
- `Email` - Validação formato email
- `CNPJ` - Validação + formatação CNPJ
- `Telefone` - Máscara telefone brasileiro
- `Endereco` - Valor composto endereço
- `Moeda` - Operações monetárias precisas

**Domain Events**:
```csharp
public record EmpresaCriada(Guid EmpresaId, string RazaoSocial) : DomainEvent;
public record ProdutoPrecoAlterado(Guid ProdutoId, decimal PrecoAntigo, decimal PrecoNovo) : DomainEvent;
```

### **GestaoRestaurante.Infrastructure** (Infrastructure Layer)
**Responsabilidade**: Acesso dados, serviços externos, infraestrutura

**Entity Framework Setup**:
```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Filial> Filiais { get; set; }
    // ... outras entidades
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurações via Fluent API
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

## 🔧 **Stack Tecnológica Detalhada**

### **Framework Base**
- **.NET 9.0** - Framework base + performance improvements
- **ASP.NET Core 9.0** - Web API framework
- **C# 12** - Linguagem com record types, pattern matching

### **Database & ORM**  
- **Entity Framework Core 9.0** - ORM principal
- **SQL Server 2022** - Database engine
- **Migrations** - Controle versionamento schema

### **Patterns & Architecture**
- **MediatR 13.0** - CQRS + Mediator Pattern
- **AutoMapper 12.0** - Object-to-object mapping
- **FluentValidation 12.0** - Validation library
- **Repository Pattern** - Data access abstraction
- **Specification Pattern** - Query encapsulation

### **Authentication & Authorization**
- **ASP.NET Core Identity** - User management
- **JWT Bearer Tokens** - Stateless authentication
- **Custom Authorization** - Module-based permissions

### **Testing**
- **xUnit 2.9.2** - Test framework
- **Moq 4.20.72** - Mocking framework
- **FluentAssertions 8.6.0** - Assertion library
- **EntityFrameworkCore.InMemory** - In-memory testing

### **Observability & Performance**
- **Serilog** - Structured logging
- **Custom Metrics** - Application-specific metrics
- **Performance Profiling** - Thread-safe profiling
- **Health Checks** - Comprehensive health monitoring

## 📊 **Módulos do Sistema**

### **🎯 Core** (Sistema base)
**Entidades**: Usuario, Role, PlanoAssinatura, LogOperacao
**Responsabilidade**: Autenticação, autorização, auditoria

### **🏢 Empresas** (Gestão empresarial)
**Entidades**: Empresa
**Features**: CRUD empresas, gestão endereços
**Endpoints**: `/api/empresas`

### **🏪 Filiais** (Gestão filiais)
**Entidades**: Filial, UsuarioFilial
**Features**: CRUD filiais, vínculos usuários
**Endpoints**: `/api/filiais`

### **📊 CentroCusto** (Estrutura hierárquica)
**Entidades**: Agrupamento → SubAgrupamento → CentroCusto
**Features**: CRUD hierárquico, FilialAgrupamento
**Endpoints**: `/api/agrupamentos`, `/api/subagrupamentos`, `/api/centroscusto`

### **📂 Categorias** (Categorização produtos)
**Entidades**: Categoria (3 níveis hierárquicos)
**Features**: CRUD categorias, estrutura árvore
**Endpoints**: `/api/categorias`

### **🛍️ Produtos** (Gestão produtos)
**Entidades**: Produto, Ingrediente, ProdutoIngrediente
**Features**: CRUD produtos, ingredientes, busca avançada
**Endpoints**: `/api/produtos`

## 🛡️ **Sistema de Segurança**

### **Autenticação JWT**
```csharp
[HttpPost("login")]
public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
{
    var result = await _mediator.Send(new LoginCommand(loginDto));
    
    // Return JWT Token + User Info
    return Ok(new LoginResponseDto
    {
        Token = result.Token,
        Expiracao = result.Expiracao,
        Usuario = result.Usuario
    });
}
```

### **Autorização por Módulos**
```csharp
[ModuleAuthorization(ModuleNames.EMPRESAS)]
public class EmpresaController : ControllerBase
{
    // Apenas usuários com acesso ao módulo EMPRESAS
}
```

### **Validação Automática**
```csharp
public class CreateEmpresaValidator : AbstractValidator<CreateEmpresaDto>
{
    public CreateEmpresaValidator()
    {
        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ é obrigatório")
            .Must(CnpjValidator.IsValid).WithMessage("CNPJ inválido");
    }
}
```

## ⚡ **Performance & Observabilidade**

### **Metrics System**
```csharp
public interface IApplicationMetrics
{
    void IncrementCounter(string name, Dictionary<string, string> tags);
    void RecordValue(string name, double value, Dictionary<string, string> tags);
}

// Usage nos Controllers
_metrics.IncrementCounter("empresa.controller.requests", new Dictionary<string, string> 
{ 
    ["endpoint"] = "create",
    ["success"] = "true"
});
```

### **Health Checks**
```csharp
services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<CacheHealthCheck>("cache") 
    .AddCheck<SystemHealthCheck>("system");
    
// Endpoints:
// GET /api/health/live - Basic liveness
// GET /api/health/ready - Readiness check  
// GET /api/health/detailed - Full health report
```

### **Performance Profiling**
```csharp
public class PerformanceActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var executedContext = await next();
        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > 1000) // Log slow requests
        {
            _logger.LogWarning("Slow request detected: {Controller}.{Action} took {ElapsedMs}ms");
        }
    }
}
```

## 🧪 **Estratégia de Testes**

### **Cobertura Atual**
- **Controllers**: 42/46 testes (91% success rate)
- **Services**: Testes unitários básicos
- **Domain**: Value Objects testados
- **Integration**: Testes com InMemoryDatabase

### **Estrutura de Testes**
```csharp
public class EmpresaControllerTests
{
    [Fact]
    public async Task GetEmpresas_ReturnsOkResult_WhenEmpresasExist()
    {
        // Arrange
        var mockService = new Mock<IEmpresaService>();
        // ... setup

        // Act  
        var result = await controller.GetEmpresas();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
```

## 🔄 **Result Pattern Implementation**

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public IReadOnlyList<string> Errors { get; }
    
    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());
    public static Result<T> Failure(params string[] errors) => new(false, default, errors);
}

// Usage nos Services
public async Task<Result<EmpresaDto>> CreateAsync(CreateEmpresaDto dto)
{
    try
    {
        // Business logic
        var empresa = _mapper.Map<Empresa>(dto);
        await _repository.AddAsync(empresa);
        return Result<EmpresaDto>.Success(_mapper.Map<EmpresaDto>(empresa));
    }
    catch (ValidationException ex)
    {
        return Result<EmpresaDto>.Failure(ex.Message);
    }
}
```

## 📚 **Próximos Passos**

### **🚧 Em Desenvolvimento**
1. **Finalizar CQRS** - Migrar todos services para Commands/Queries
2. **Domain Events** - Implementar handlers completos
3. **Cache Distribuído** - Migrar para Redis
4. **Background Jobs** - Implementar Hangfire/Quartz

### **🔮 Roadmap Futuro**
1. **Módulos Adicionais** - Cardápio, Pedidos, Estoque, Financeiro
2. **Event Sourcing** - Para auditoria completa
3. **Microservices** - Decomposição por bounded contexts
4. **GraphQL** - API alternativa para queries complexas

---

**Arquitetura robusta, escalável e preparada para crescimento enterprise.**