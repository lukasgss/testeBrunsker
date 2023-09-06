namespace Application.Common.Interfaces.ApisExternas.ViaCep;

public class RespostaViaCep
{
    public string Cep { get; set; } = null!; 
    public string Logradouro { get; set; } = null!;
    public string Complemento { get; set; } = null!;
    public string Bairro { get; set; } = null!;
    public string Localidade { get; set; } = null!;
    public string Uf { get; set; } = null!;
    public string Ibge { get; set; } = null!;
    public string Gia { get; set; } = null!;
    public string Ddd { get; set; } = null!;
    public string Siafi { get; set; } = null!;
}