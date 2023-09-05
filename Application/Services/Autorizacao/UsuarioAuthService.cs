using System.Security.Claims;
using Application.Common.Interfaces.Authorization;

namespace Application.Services.Autorizacao;

public class UsuarioAuthService : IUsuarioAuthService
{
    public int? ObterIdPorTokenJwt(ClaimsPrincipal user)
    {
        string? userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return null;
        }

        return int.Parse(userId);
    }
}