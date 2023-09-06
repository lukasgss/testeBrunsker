using Application.Common.Interfaces.Entidades.Usuarios.DTOs;

namespace Application.Common.Interfaces.Entidades.Usuarios;

public interface IUsuarioService
{
    Task<RespostaDadosUsuario> ObterUsuarioPorId(int idUsuario);
    Task<RespostaUsuario> RegistrarAsync(CriarUsuarioRequest criarUsuarioRequest);
    Task<RespostaUsuario> LoginAsync(LoginRequest loginRequest);
}