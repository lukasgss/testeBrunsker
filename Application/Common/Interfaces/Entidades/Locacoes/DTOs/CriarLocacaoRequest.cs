namespace Application.Common.Interfaces.Entidades.Locacoes.DTOs;

public class CriarLocacaoRequest
{
    public int IdImovel { get; set; }
    public int IdLocatario { get; set; }
    public DateOnly DataVencimento { get; set; }
    public decimal ValorMensal { get; set; }
}