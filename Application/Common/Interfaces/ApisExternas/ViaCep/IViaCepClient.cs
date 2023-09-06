namespace Application.Common.Interfaces.ApisExternas.ViaCep;

public interface IViaCepClient
{
    Task<RespostaViaCep?> ObterEnderecoPorCep(string cep);
}