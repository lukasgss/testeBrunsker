using Application.Common.Interfaces.GenericRepository;
using Domain.Entidades;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces.Entidades.Usuarios;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<IdentityResult> RegistrarAsync(Usuario usuario, string senha);
    Task<SignInResult> CheckCredentials(Usuario usuario, string senha);
    Task<IdentityResult> SetLockoutEnabledAsync(Usuario usuario, bool habilitado);
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
}