using Application.Common.Interfaces.ApisExternas.ViaCep;
using Tests.TesteUtils.Constantes;

namespace Tests.TesteUtils.GeradoresEntidades;

public static class GeradorCep
{
    public static RespostaViaCep GerarRespostaViaCep()
    {
        return new RespostaViaCep()
        {
            Cep = Constants.DadosCep.Cep,
            Logradouro = Constants.DadosCep.Logradouro,
            Complemento = Constants.DadosCep.Complemento,
            Bairro = Constants.DadosCep.Bairro,
            Localidade = Constants.DadosCep.Localidade,
            Uf = Constants.DadosCep.Uf,
            Ibge = Constants.DadosCep.Ibge,
            Gia = Constants.DadosCep.Gia,
            Ddd = Constants.DadosCep.Ddd,
            Siafi = Constants.DadosCep.Siafi
        };
    }
    
    public static RespostaViaCep GerarRespostaViaCepInvalida()
    {
        return new RespostaViaCep()
        {
            Cep = null,
            Logradouro = null,
            Complemento = null,
            Bairro = null,
            Localidade = null,
            Uf = null,
            Ibge = null,
            Gia = null,
            Ddd = null,
            Siafi = null
        };
    }
}