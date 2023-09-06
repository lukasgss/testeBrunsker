namespace Application.Common.Interfaces.ApisExternas.ViaCep;

public static class ViaCepConfig
{
    public const string ChaveClient = "ViaCep";
    public static readonly Uri BaseUrl = new Uri("https://viacep.com.br/ws/"); 
}