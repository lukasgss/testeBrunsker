using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Domain.Entidades;

namespace Application.Common.Extensions.Mapeamento;

public static class MapeamentosLocacao
{
    public static RespostaLocacao ToRespostaLocacao(this Locacao locacao)
    {
        return new RespostaLocacao()
        {
            Id = locacao.Id,
            Imovel = locacao.Imovel.ToRespostaImovel(),
            Locador = locacao.Locador.ToRespostaDadosUsuario(),
            Locatario = locacao.Locatario.ToRespostaDadosUsuario(),
            LocadorAssinou = locacao.LocadorAssinou,
            LocatarioAssinou = locacao.LocatarioAssinou,
            DataFechamento = locacao.DataFechamento,
            DataVencimento = locacao.DataVencimento,
            ValorMensal = locacao.ValorMensal
        };
    }
}