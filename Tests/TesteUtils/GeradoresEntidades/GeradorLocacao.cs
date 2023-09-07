using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Domain.Entidades;
using Tests.TesteUtils.Constantes;

namespace Tests.TesteUtils.GeradoresEntidades;

public static class GeradorLocacao
{
    public static Locacao GerarLocacao()
    {
        return new Locacao()
        {
            Id = Constants.DadosLocacao.Id,
            Imovel = Constants.DadosLocacao.Imovel,
            ImovelId = Constants.DadosLocacao.ImovelId,
            Locador = Constants.DadosLocacao.Locador,
            LocadorId = Constants.DadosLocacao.LocadorId,
            Locatario = Constants.DadosLocacao.Locatario,
            LocatarioId = Constants.DadosLocacao.LocatarioId,
            LocadorAssinou = Constants.DadosLocacao.LocadorAssinou,
            LocatarioAssinou = Constants.DadosLocacao.LocatarioAssinou,
            DataFechamento = Constants.DadosLocacao.DataFechamento,
            DataVencimento = Constants.DadosLocacao.DataVencimento,
            ValorMensal = Constants.DadosLocacao.ValorMensal
        };
    }

    public static CriarLocacaoRequest GerarCriarLocacaoRequest()
    {
        return new CriarLocacaoRequest()
        {
            IdImovel = Constants.DadosImovel.Id,
            IdLocatario = Constants.DadosLocacao.LocatarioId,
            DataVencimento = Constants.DadosLocacao.DataVencimento,
            ValorMensal = Constants.DadosLocacao.ValorMensal
        };
    }

    public static EditarLocacaoRequest GerarEditarLocacaoRequest()
    {
        return new EditarLocacaoRequest()
        {
            Id = Constants.DadosLocacao.Id,
            IdImovel = Constants.DadosImovel.Id,
            IdLocatario = Constants.DadosLocacao.LocatarioId,
            DataVencimento = Constants.DadosLocacao.DataVencimento,
            ValorMensal = Constants.DadosLocacao.ValorMensal
        };
    }

    public static RespostaLocacao GerarRespostaLocacao()
    {
        return new RespostaLocacao()
        {
            Id = Constants.DadosLocacao.Id,
            Imovel = Constants.DadosLocacao.Imovel.ToRespostaImovel(),
            Locador = Constants.DadosLocacao.Locador.ToRespostaDadosUsuario(),
            Locatario = Constants.DadosLocacao.Locatario.ToRespostaDadosUsuario(),
            LocadorAssinou = Constants.DadosLocacao.LocadorAssinou,
            LocatarioAssinou = Constants.DadosLocacao.LocatarioAssinou,
            DataFechamento = Constants.DadosLocacao.DataFechamento,
            DataVencimento = Constants.DadosLocacao.DataVencimento,
            ValorMensal = Constants.DadosLocacao.ValorMensal
        };
    }
}