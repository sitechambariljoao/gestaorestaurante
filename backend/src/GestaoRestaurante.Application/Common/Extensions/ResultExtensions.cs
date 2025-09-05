using GestaoRestaurante.Domain.Common;

namespace GestaoRestaurante.Application.Common.Extensions;

/// <summary>
/// Extensions para trabalhar com Result pattern de forma fluente
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converte um Result simples em Result tipado com valor padrão
    /// </summary>
    public static Result<T> ToResult<T>(this Result result, T value)
    {
        return result.IsSuccess 
            ? Result<T>.Success(value)
            : Result<T>.Failure(result.Errors);
    }

    /// <summary>
    /// Converte um Result tipado em Result simples
    /// </summary>
    public static Result ToResult<T>(this Result<T> result)
    {
        return result.IsSuccess 
            ? Result.Success()
            : Result.Failure(result.Errors);
    }

    /// <summary>
    /// Executa uma ação se o resultado for sucesso
    /// </summary>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess && result.Value != null)
        {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Executa uma ação se o resultado for falha
    /// </summary>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<IEnumerable<string>> action)
    {
        if (!result.IsSuccess)
        {
            action(result.Errors);
        }
        return result;
    }

    /// <summary>
    /// Transforma o valor do resultado se for sucesso
    /// </summary>
    public static Result<TOutput> Map<TInput, TOutput>(this Result<TInput> result, Func<TInput, TOutput> mapper)
    {
        return result.IsSuccess && result.Value != null
            ? Result<TOutput>.Success(mapper(result.Value))
            : Result<TOutput>.Failure(result.Errors);
    }

    /// <summary>
    /// Combina múltiplos resultados em um só
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        var allErrors = results
            .Where(r => !r.IsSuccess)
            .SelectMany(r => r.Errors)
            .ToList();

        return allErrors.Count == 0
            ? Result.Success()
            : Result.Failure(allErrors);
    }

    /// <summary>
    /// Combina múltiplos resultados tipados em um só
    /// </summary>
    public static Result<T[]> Combine<T>(params Result<T>[] results)
    {
        var allErrors = results
            .Where(r => !r.IsSuccess)
            .SelectMany(r => r.Errors)
            .ToList();

        if (allErrors.Count > 0)
        {
            return Result<T[]>.Failure(allErrors);
        }

        var values = results.Select(r => r.Value!).ToArray();
        return Result<T[]>.Success(values);
    }
}