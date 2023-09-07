using System.Collections.Generic;
using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Domain.Entidades;
using Tests.TesteUtils.Constantes;

namespace Tests.TesteUtils.GeradoresEntidades;

public static class GeradorImovel
{
    public static Imovel GerarImovel()
    {
        return new Imovel()
        {
            Id = Constants.DadosImovel.Id,
            Endereco = Constants.DadosImovel.Endereco,
            Cep = Constants.DadosImovel.Cep,
            Numero = Constants.DadosImovel.Numero,
            Complemento = Constants.DadosImovel.Complemento,
            Dono = Constants.DadosImovel.Dono
        };
    }

    public static Imovel GerarImovelComComplementoENumero(string complemento, int numero)
    {
        return new Imovel()
        {
            Id = Constants.DadosImovel.Id,
            Endereco = Constants.DadosImovel.Endereco,
            Cep = Constants.DadosImovel.Cep,
            Numero = numero,
            Complemento = complemento,
            Dono = Constants.DadosImovel.Dono
        };
    }

    public static List<Imovel> GerarListaImoveis()
    {
        return new List<Imovel>()
        {
            GerarImovel()
        };
    }

    public static CriarImovelRequest GerarCriarImovelRequest()
    {
        return new CriarImovelRequest()
        {
            Cep = Constants.DadosImovel.Cep,
            Numero = Constants.DadosImovel.Numero,
            Complemento = Constants.DadosImovel.Complemento
        };
    }

    public static EditarImovelRequest GerarEditarImovelRequest()
    {
        return new EditarImovelRequest()
        {
            Id = Constants.DadosImovel.Id,
            Cep = Constants.DadosImovel.Cep,
            Numero = Constants.DadosImovel.Numero,
            Complemento = Constants.DadosImovel.Complemento
        };
    }

    public static RespostaImovel GerarRespostaImovel()
    {
        return new RespostaImovel()
        {
            Id = Constants.DadosImovel.Id,
            Cep = Constants.DadosImovel.Cep,
            Endereco = Constants.DadosImovel.Endereco,
            Numero = Constants.DadosImovel.Numero,
            Complemento = Constants.DadosImovel.Complemento,
            Dono = Constants.DadosImovel.Dono.ToRespostaDadosUsuario()
        };
    }
}