using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios.DTOs;

namespace Application.Common.Interfaces.Entidades.Locacoes.DTOs;

public class RespostaLocacao
{
    public int Id { get; set; }
    public RespostaImovel Imovel { get; set; } = null!;
    public RespostaDadosUsuario Locador { get; set; } = null!;
    public RespostaDadosUsuario Locatario { get; set; } = null!;
    public bool LocadorAssinou { get; set; }
    public bool LocatarioAssinou { get; set; }
    public DateTime? DataFechamento { get; set; }
    public DateOnly DataVencimento { get; set; }
    public decimal ValorMensal { get; set; }
}