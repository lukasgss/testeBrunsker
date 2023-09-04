using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigurarApplication(this IServiceCollection services)
    {
        return services;
    }
}