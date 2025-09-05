using System.ComponentModel.DataAnnotations;

namespace GestaoRestaurante.Application.DTOs;

/// <summary>
/// DTO para representar endereços completos
/// </summary>
public class EnderecoDto
{
    [Required(ErrorMessage = "Logradouro é obrigatório")]
    [StringLength(200, ErrorMessage = "Logradouro deve ter no máximo 200 caracteres")]
    public string Logradouro { get; set; } = string.Empty;

    [Required(ErrorMessage = "Número é obrigatório")]
    [StringLength(20, ErrorMessage = "Número deve ter no máximo 20 caracteres")]
    public string Numero { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
    public string? Complemento { get; set; }

    [Required(ErrorMessage = "CEP é obrigatório")]
    [StringLength(10, ErrorMessage = "CEP deve ter no máximo 10 caracteres")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP deve conter exatamente 8 dígitos numéricos")]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bairro é obrigatório")]
    [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
    public string Bairro { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cidade é obrigatória")]
    [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "Estado é obrigatório")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter exatamente 2 caracteres")]
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// Retorna o endereço completo formatado
    /// </summary>
    public string EnderecoCompleto =>
        $"{Logradouro}, {Numero}" +
        (!string.IsNullOrWhiteSpace(Complemento) ? $", {Complemento}" : "") +
        $", {Bairro}, {Cidade} - {Estado}, CEP: {CepFormatado}";

    /// <summary>
    /// Retorna o CEP formatado (12345-678)
    /// </summary>
    public string CepFormatado =>
        Cep.Length == 8 ? $"{Cep.Substring(0, 5)}-{Cep.Substring(5)}" : Cep;
}