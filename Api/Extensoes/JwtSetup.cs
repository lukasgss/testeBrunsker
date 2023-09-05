using System.Text;
using Application.Services.Autenticacao;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensoes;

public static class JwtSetup
{
    public static void ConfigurarJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                IConfigurationSection configJwt = configuration.GetSection(JwtConfig.NomeSecao);
                string secret = configJwt["SecretKey"]!;

                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configJwt["Issuer"],
                    ValidAudience = configJwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secret)
                    )
                };
            });
    }
}