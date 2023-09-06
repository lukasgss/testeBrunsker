using System.Security.Claims;

namespace Application.Common.Interfaces.Autorizacao;

public interface IUsuarioAuthService
{
    int ObterIdPorTokenJwt(ClaimsPrincipal user);
}