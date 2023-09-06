using System.Net.Http.Json;
using Application.Common.Interfaces.ApisExternas.ViaCep;

namespace Infrastructure.ApisExternas;

public class ViaCepClient : IViaCepClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ViaCepClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<RespostaViaCep?> ObterEnderecoPorCep(string cep)
    {
        HttpClient client = _httpClientFactory.CreateClient(ViaCepConfig.ChaveClient);

        return await client.GetFromJsonAsync<RespostaViaCep>($"{cep}/json");
    }
}