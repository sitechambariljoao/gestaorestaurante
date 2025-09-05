using System.Diagnostics.CodeAnalysis;

namespace GestaoRestaurante.Domain.Common;

/// <summary>
/// Result pattern implementation for better error handling and flow control
/// </summary>
public class Result
{
    protected Result(bool isSuccess, IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? Array.Empty<string>();
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<string> Errors { get; }
    public string ErrorMessage => string.Join("; ", Errors);

    public static Result Success() => new(true, Array.Empty<string>());
    
    public static Result Failure(string error) => new(false, new[] { error });
    
    public static Result Failure(IEnumerable<string> errors) => new(false, errors.ToArray());

    public static implicit operator Result(string error) => Failure(error);
}

/// <summary>
/// Generic result with data payload
/// </summary>
public class Result<T> : Result
{
    private readonly T? _value;

    protected Result(T? value, bool isSuccess, IReadOnlyList<string> errors) 
        : base(isSuccess, errors)
    {
        _value = value;
    }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue => IsSuccess && _value is not null;

    public T? Value => IsSuccess ? _value : default;

    public static Result<T> Success(T value) => new(value, true, Array.Empty<string>());
    
    public static new Result<T> Failure(string error) => new(default, false, new[] { error });
    
    public static new Result<T> Failure(IEnumerable<string> errors) => new(default, false, errors.ToArray());

    public static implicit operator Result<T>(T value) => Success(value);
    
    public static implicit operator Result<T>(string error) => Failure(error);

    /// <summary>
    /// Transforms the result value if successful, otherwise returns failure
    /// </summary>
    public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
    {
        return IsSuccess && HasValue 
            ? Result<TOut>.Success(mapper(Value)) 
            : Result<TOut>.Failure(Errors);
    }

    /// <summary>
    /// Binds to another result-returning operation if successful
    /// </summary>
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
    {
        return IsSuccess && HasValue 
            ? binder(Value) 
            : Result<TOut>.Failure(Errors);
    }

    /// <summary>
    /// Executes action if successful, returns original result
    /// </summary>
    public Result<T> Tap(Action<T> action)
    {
        if (IsSuccess && HasValue)
        {
            action(Value);
        }
        return this;
    }

    /// <summary>
    /// Returns value if successful, otherwise returns provided fallback
    /// </summary>
    public T GetValueOrDefault(T fallback = default!) => IsSuccess && HasValue ? Value : fallback;

    /// <summary>
    /// Matches success and failure cases
    /// </summary>
    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<IReadOnlyList<string>, TOut> onFailure)
    {
        return IsSuccess && HasValue ? onSuccess(Value) : onFailure(Errors);
    }
}

/// <summary>
/// Result for operations without return value (like void methods)
/// </summary>
public sealed class OperationResult : Result
{
    private OperationResult(bool isSuccess, IReadOnlyList<string> errors) : base(isSuccess, errors) { }

    public static new OperationResult Success() => new(true, Array.Empty<string>());
    public static new OperationResult Failure(string error) => new(false, new[] { error });
    public static new OperationResult Failure(IEnumerable<string> errors) => new(false, errors.ToArray());

    public static implicit operator OperationResult(string error) => Failure(error);
    
    public static OperationResult FromResult(Result result) => 
        result.IsSuccess ? Success() : Failure(result.Errors);
}

/// <summary>
/// Extension methods for Result operations
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Combines multiple results into a single result
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToArray();
        
        if (failures.Length == 0)
            return Result.Success();
            
        var allErrors = failures.SelectMany(r => r.Errors).ToArray();
        return Result.Failure(allErrors);
    }

    /// <summary>
    /// Async version of Map
    /// </summary>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Result<T> result, Func<T, Task<TOut>> mapper)
    {
        if (result.IsFailure || !result.HasValue)
            return Result<TOut>.Failure(result.Errors);

        try
        {
            var mapped = await mapper(result.Value);
            return Result<TOut>.Success(mapped);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Async version of Bind
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Result<T> result, Func<T, Task<Result<TOut>>> binder)
    {
        if (result.IsFailure || !result.HasValue)
            return Result<TOut>.Failure(result.Errors);

        try
        {
            return await binder(result.Value);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Async version of Tap
    /// </summary>
    public static async Task<Result<T>> TapAsync<T>(this Result<T> result, Func<T, Task> action)
    {
        if (result.IsSuccess && result.HasValue)
        {
            await action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Converts Result to OperationResult
    /// </summary>
    public static OperationResult ToOperationResult(this Result result) =>
        OperationResult.FromResult(result);

    /// <summary>
    /// Converts Result<T> to OperationResult
    /// </summary>
    public static OperationResult ToOperationResult<T>(this Result<T> result) =>
        OperationResult.FromResult(result);

    /// <summary>
    /// Converts Task<Result> to Task<OperationResult>
    /// </summary>
    public static async Task<OperationResult> ToOperationResultAsync(this Task<Result> resultTask)
    {
        var result = await resultTask;
        return result.ToOperationResult();
    }

    /// <summary>
    /// Converts Task<Result<T>> to Task<OperationResult>
    /// </summary>
    public static async Task<OperationResult> ToOperationResultAsync<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        return result.ToOperationResult();
    }

    /// <summary>
    /// Ensures operation succeeds or throws an exception with combined error messages
    /// </summary>
    public static Result EnsureSuccess(this Result result)
    {
        if (result.IsFailure)
            throw new InvalidOperationException(result.ErrorMessage);
        return result;
    }

    /// <summary>
    /// Ensures operation succeeds or throws an exception with combined error messages
    /// </summary>
    public static Result<T> EnsureSuccess<T>(this Result<T> result)
    {
        if (result.IsFailure)
            throw new InvalidOperationException(result.ErrorMessage);
        return result;
    }
}