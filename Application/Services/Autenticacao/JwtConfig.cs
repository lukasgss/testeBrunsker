namespace Application.Services.Autenticacao;

public class JwtConfig
{
    public const string NomeSecao = "JwtConfig";
    
    public string SecretKey { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public int TempoExpiracaoEmMin { get; init; }
}