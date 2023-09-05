using System.Security.Claims;

namespace Application.Common.Interfaces.Authorization;

public interface IUsuarioAuthService
{
    int? ObterIdPorTokenJwt(ClaimsPrincipal user);
}