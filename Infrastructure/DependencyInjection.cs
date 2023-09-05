using Application.Common.Interfaces.Entidades.Usuarios;
using Infrastructure.Persistencia.Repositorios;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigurarInfrastrutura(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        
        return services;
    }
}