using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Autorizacao;

namespace Application.Services.Autorizacao;

public class UsuarioAuthService : IUsuarioAuthService
{
    public int ObterIdPorTokenJwt(ClaimsPrincipal user)
    {
        string? userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            throw new UnauthorizedException("Faça login para utilizar desse recurso.");
        }

        return int.Parse(userId);
    }
}