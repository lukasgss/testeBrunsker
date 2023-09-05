using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Providers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Application.Services.Autenticacao;

public class GeradorTokenJwt : IGeradorTokenJwt
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly JwtConfig _jwtConfig;

    public GeradorTokenJwt(IDateTimeProvider dateTimeProvider, IOptions<JwtConfig> opcoesJwt)
    {
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _jwtConfig = opcoesJwt.Value;
    }

    public string GerarToken(int idUsuario, string fullName)
    {
        SigningCredentials signingCredentials = new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, fullName)
        };

        JwtSecurityToken jwtSecurityToken = new(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: _dateTimeProvider.UtcNow().AddMinutes(_jwtConfig.TempoExpiracaoEmMin),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}