using Application.Common.Interfaces.ApisExternas.ViaCep;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Locacoes;
using Application.Common.Interfaces.Entidades.Usuarios;
using Infrastructure.ApisExternas;
using Infrastructure.Persistencia.DataContext;
using Infrastructure.Persistencia.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigurarInfrastrutura(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IImovelRepository, ImovelRepository>();
        services.AddScoped<IViaCepClient, ViaCepClient>();
        services.AddScoped<ILocacaoRepository, LocacaoRepository>();

        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection")!,
                ServerVersion.AutoDetect(connectionString));
        });

        services.AddHttpClient(ViaCepConfig.ChaveClient, cliente =>
        {
            cliente.BaseAddress = ViaCepConfig.BaseUrl;
        });

        return services;
    }
}