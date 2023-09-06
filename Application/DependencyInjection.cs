using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Autorizacao;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Providers;
using Application.Common.Providers;
using Application.Services.Autenticacao;
using Application.Services.Autorizacao;
using Application.Services.Entidades.Imoveis;
using Application.Services.Entidades.Usuarios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigurarApplication(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IUsuarioAuthService, UsuarioAuthService>();
        services.AddScoped<IImovelService, ImovelService>();
        
        services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.NomeSecao));
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IGeradorTokenJwt, GeradorTokenJwt>();
        
        return services;
    }
}