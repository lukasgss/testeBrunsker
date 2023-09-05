namespace Application.Common.Interfaces.Authentication;

public interface IGeradorTokenJwt
{
    string GerarToken(int idUsuario, string fullName);
}