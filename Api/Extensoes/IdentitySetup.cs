using Domain.Entidades;
using Infrastructure.Persistencia.DataContext;
using Microsoft.AspNetCore.Identity;

namespace Api.Extensoes;

public static class IdentitySetup
{
    public static void ConfigurarIdentity(this IServiceCollection services)
    {
        services.AddIdentity<Usuario, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }
}