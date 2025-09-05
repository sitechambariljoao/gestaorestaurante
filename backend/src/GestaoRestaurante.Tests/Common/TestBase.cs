using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using FluentValidation;
using GestaoRestaurante.Infrastructure.Data.Context;
using GestaoRestaurante.Application.Mappings;
using Xunit.Abstractions;

namespace GestaoRestaurante.Tests.Common;

/// <summary>
/// Classe base para todos os testes com configuração comum
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly ITestOutputHelper Output;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly GestaoRestauranteContext DbContext;
    protected readonly IMapper Mapper;
    protected readonly Mock<ILogger> MockLogger;

    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
        MockLogger = new Mock<ILogger>();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        DbContext = ServiceProvider.GetRequiredService<GestaoRestauranteContext>();
        Mapper = ServiceProvider.GetRequiredService<IMapper>();

        // Garantir que o banco está criado
        DbContext.Database.EnsureCreated();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // DbContext em memória
        services.AddDbContext<GestaoRestauranteContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        // AutoMapper
        services.AddAutoMapper(typeof(EmpresaMappingProfile));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<EmpresaMappingProfile>();

        // Logging mock
        services.AddSingleton(MockLogger.Object);
        services.AddLogging();
    }

    protected void LogTestOutput(string message)
    {
        Output.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
    }

    public virtual void Dispose()
    {
        DbContext?.Dispose();
        ServiceProvider?.GetService<IServiceScope>()?.Dispose();
    }
}

/// <summary>
/// Base class para testes de integração com seeding
/// </summary>
public abstract class IntegrationTestBase : TestBase
{
    protected IntegrationTestBase(ITestOutputHelper output) : base(output)
    {
        SeedTestData();
    }

    protected virtual void SeedTestData()
    {
        // Override em classes filhas para adicionar dados de teste específicos
    }

    protected async Task<T> AddEntityAsync<T>(T entity) where T : class
    {
        DbContext.Set<T>().Add(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    protected async Task<T[]> AddEntitiesAsync<T>(params T[] entities) where T : class
    {
        DbContext.Set<T>().AddRange(entities);
        await DbContext.SaveChangesAsync();
        return entities;
    }
}

/// <summary>
/// Base class para testes unitários com mocks
/// </summary>
public abstract class UnitTestBase : TestBase
{
    protected readonly MockRepository MockRepository;

    protected UnitTestBase(ITestOutputHelper output) : base(output)
    {
        MockRepository = new MockRepository(MockBehavior.Strict);
    }

    protected Mock<T> CreateMock<T>() where T : class
    {
        var mock = MockRepository.Create<T>();
        LogTestOutput($"Mock created for {typeof(T).Name}");
        return mock;
    }

    protected Mock<T> CreateLooseMock<T>() where T : class
    {
        var mock = new Mock<T>();
        LogTestOutput($"Loose mock created for {typeof(T).Name}");
        return mock;
    }

    public override void Dispose()
    {
        try
        {
            MockRepository.Verify();
        }
        catch (Exception ex)
        {
            LogTestOutput($"Mock verification failed: {ex.Message}");
            throw;
        }
        finally
        {
            base.Dispose();
        }
    }
}

/// <summary>
/// Base class para testes de performance
/// </summary>
public abstract class PerformanceTestBase : TestBase
{
    protected readonly System.Diagnostics.Stopwatch Stopwatch;

    protected PerformanceTestBase(ITestOutputHelper output) : base(output)
    {
        Stopwatch = new System.Diagnostics.Stopwatch();
    }

    protected void StartMeasurement()
    {
        Stopwatch.Restart();
    }

    protected TimeSpan StopMeasurement()
    {
        Stopwatch.Stop();
        var elapsed = Stopwatch.Elapsed;
        LogTestOutput($"Measurement completed in {elapsed.TotalMilliseconds:F2}ms");
        return elapsed;
    }

    protected async Task<TimeSpan> MeasureAsync(Func<Task> action)
    {
        StartMeasurement();
        await action();
        return StopMeasurement();
    }

    protected TimeSpan Measure(Action action)
    {
        StartMeasurement();
        action();
        return StopMeasurement();
    }

    protected void AssertPerformance(TimeSpan elapsed, TimeSpan expectedMax, string operationName)
    {
        LogTestOutput($"{operationName}: {elapsed.TotalMilliseconds:F2}ms (limit: {expectedMax.TotalMilliseconds:F2}ms)");
        
        if (elapsed > expectedMax)
        {
            throw new Exception($"{operationName} took {elapsed.TotalMilliseconds:F2}ms, expected under {expectedMax.TotalMilliseconds:F2}ms");
        }
    }
}