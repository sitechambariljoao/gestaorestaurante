using GestaoRestaurante.Application.DTOs;

namespace GestaoRestaurante.Application.Interfaces;

public interface IBaseService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    Task<ServiceResult<IEnumerable<TDto>>> GetAllAsync();
    Task<ServiceResult<TDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<TDto>> CreateAsync(TCreateDto createDto);
    Task<ServiceResult<TDto>> UpdateAsync(Guid id, TUpdateDto updateDto);
    Task<ServiceResult<bool>> DeleteAsync(Guid id);
}

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();

    public static ServiceResult<T> SuccessResult(T data)
    {
        return new ServiceResult<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ServiceResult<T> ErrorResult(string errorMessage)
    {
        return new ServiceResult<T>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    public static ServiceResult<T> ValidationErrorResult(List<string> validationErrors)
    {
        return new ServiceResult<T>
        {
            Success = false,
            ValidationErrors = validationErrors
        };
    }
}