using Moq;
using Microsoft.Extensions.Logging;
using GestaoRestaurante.Application.Common.Caching;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Application.Common.Interfaces;

namespace GestaoRestaurante.Tests.Helpers;

/// <summary>
/// Extensões para facilitar criação e configuração de mocks
/// </summary>
public static class MockExtensions
{
    /// <summary>
    /// Configura mock de logger para capturar e verificar logs
    /// </summary>
    public static Mock<ILogger<T>> SetupLogger<T>(this Mock<ILogger<T>> mockLogger)
    {
        mockLogger.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        return mockLogger;
    }

    /// <summary>
    /// Verifica se um log específico foi chamado
    /// </summary>
    public static void VerifyLogCalled<T>(this Mock<ILogger<T>> mockLogger, LogLevel level, string message)
    {
        mockLogger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Configura mock de cache service com comportamento padrão
    /// </summary>
    public static Mock<ICacheService> SetupCacheService(this Mock<ICacheService> mockCache)
    {
        // Configuração básica sem usar It.IsAnyType que causa problemas
        mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockCache.Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mockCache;
    }

    /// <summary>
    /// Configura mock de cache com dados pré-definidos
    /// </summary>
    public static Mock<ICacheService> WithCacheData<T>(this Mock<ICacheService> mockCache, string key, T data) where T : class
    {
        mockCache.Setup(x => x.GetAsync<T>(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        return mockCache;
    }

    /// <summary>
    /// Configura mock de repository com operações básicas
    /// </summary>
    public static Mock<TRepository> SetupRepository<TRepository, TEntity>(this Mock<TRepository> mockRepo, List<TEntity>? data = null)
        where TRepository : class
        where TEntity : BaseEntity
    {
        var entities = data ?? new List<TEntity>();

        // Setup comum para repositórios base
        if (mockRepo.Object is IBaseRepository<TEntity>)
        {
            var baseRepo = mockRepo.As<IBaseRepository<TEntity>>();
            
            baseRepo.Setup(x => x.GetAllAsync())
                .ReturnsAsync(entities);

            baseRepo.Setup(x => x.AddAsync(It.IsAny<TEntity>()))
                .ReturnsAsync((TEntity entity) => { entities.Add(entity); return entity; });

            baseRepo.Setup(x => x.UpdateAsync(It.IsAny<TEntity>()))
                .Returns(Task.CompletedTask);

            baseRepo.Setup(x => x.Delete(It.IsAny<TEntity>()))
                .Callback<TEntity>(entity => entities.Remove(entity));

            baseRepo.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);
        }

        return mockRepo;
    }

    /// <summary>
    /// Configura mock de performance profiler
    /// </summary>
    public static Mock<IPerformanceProfiler> SetupPerformanceProfiler(this Mock<IPerformanceProfiler> mockProfiler)
    {
        var mockMeasurement = new Mock<IDisposable>();
        
        mockProfiler.Setup(x => x.StartMeasurement(It.IsAny<string>()))
            .Returns(mockMeasurement.Object);

        mockProfiler.Setup(x => x.RecordMetric(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<string>()));

        mockProfiler.Setup(x => x.GetStatsAsync())
            .ReturnsAsync(new PerformanceStats());

        return mockProfiler;
    }

    /// <summary>
    /// Configura callback para capturar argumentos de chamadas
    /// </summary>
    public static Mock<T> CaptureArguments<T, TArg>(this Mock<T> mock, 
        System.Linq.Expressions.Expression<Action<T>> expression,
        List<TArg> capturedArgs) where T : class
    {
        mock.Setup(expression)
            .Callback<TArg>(arg => capturedArgs.Add(arg));

        return mock;
    }

    /// <summary>
    /// Configura mock para retornar diferentes valores em chamadas sequenciais
    /// </summary>
    public static Mock<T> SetupSequence<T, TResult>(this Mock<T> mock,
        System.Linq.Expressions.Expression<Func<T, TResult>> expression,
        params TResult[] results) where T : class
    {
        var setup = mock.SetupSequence(expression);
        
        foreach (var result in results)
        {
            setup = setup.Returns(result);
        }

        return mock;
    }

    /// <summary>
    /// Configura mock async para retornar diferentes valores em chamadas sequenciais
    /// </summary>
    public static Mock<T> SetupSequenceAsync<T, TResult>(this Mock<T> mock,
        System.Linq.Expressions.Expression<Func<T, Task<TResult>>> expression,
        params TResult[] results) where T : class
    {
        var setup = mock.SetupSequence(expression);
        
        foreach (var result in results)
        {
            setup = setup.ReturnsAsync(result);
        }

        return mock;
    }

    /// <summary>
    /// Verifica se uma chamada async foi feita com parâmetros específicos
    /// </summary>
    public static void VerifyAsync<T, TResult>(this Mock<T> mock,
        System.Linq.Expressions.Expression<Func<T, Task<TResult>>> expression,
        Times times) where T : class
    {
        mock.Verify(expression, times);
    }

    /// <summary>
    /// Configura mock para simular exceção
    /// </summary>
    public static Mock<T> ThrowsException<T, TException>(this Mock<T> mock,
        System.Linq.Expressions.Expression<Action<T>> expression,
        TException exception) 
        where T : class 
        where TException : Exception
    {
        mock.Setup(expression).Throws(exception);
        return mock;
    }

    /// <summary>
    /// Configura mock async para simular exceção
    /// </summary>
    public static Mock<T> ThrowsExceptionAsync<T, TResult, TException>(this Mock<T> mock,
        System.Linq.Expressions.Expression<Func<T, Task<TResult>>> expression,
        TException exception) 
        where T : class 
        where TException : Exception
    {
        mock.Setup(expression).ThrowsAsync(exception);
        return mock;
    }
}