using Application.Common.Exceptions;
using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Autenticacao;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Entidades.Usuarios.DTOs;
using Domain.Entidades;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Entidades.Usuarios;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IGeradorTokenJwt _geradorTokenJwt;

    public UsuarioService(IUsuarioRepository usuarioRepository, IGeradorTokenJwt geradorTokenJwt)
    {
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        _geradorTokenJwt = geradorTokenJwt ?? throw new ArgumentNullException(nameof(geradorTokenJwt));
    }

    public async Task<RespostaDadosUsuario> ObterUsuarioPorId(int idUsuario)
    {
        Usuario? usuario = await _usuarioRepository.ObterPorIdAsync(idUsuario);
        if (usuario is null)
        {
            throw new NotFoundException("Usuário com o id especificado não existe.");
        }

        return usuario.ToRespostaDadosUsuario();
    }

    public async Task<RespostaUsuario> RegistrarAsync(CriarUsuarioRequest criarUsuarioRequest)
    {
        Usuario usuarioParaCriar = new()
        {
            NomeCompleto = criarUsuarioRequest.NomeCompleto,
            UserName = criarUsuarioRequest.Email,
            PhoneNumber = criarUsuarioRequest.Telefone,
            Email = criarUsuarioRequest.Email,
            EmailConfirmed = true,
        };

        Usuario? usuarioJaExiste = await _usuarioRepository.ObterPorEmailAsync(criarUsuarioRequest.Email);
        if (usuarioJaExiste is not null)
        {
            throw new ConflictException("Usuário com o e-mail especificado já existe.");
        }

        IdentityResult resultadoCadastro =
            await _usuarioRepository.RegistrarAsync(usuarioParaCriar, criarUsuarioRequest.Senha);

        IdentityResult resultadoLockout =
            await _usuarioRepository.SetLockoutEnabledAsync(usuarioParaCriar, habilitado: false);

        if (!resultadoCadastro.Succeeded || !resultadoLockout.Succeeded)
        {
            throw new InternalServerErrorException();
        }

        string jwtToken = _geradorTokenJwt.GerarToken(usuarioParaCriar.Id, usuarioParaCriar.NomeCompleto);

        return usuarioParaCriar.ToRespostaUsuario(jwtToken);
    }

    public async Task<RespostaUsuario> LoginAsync(LoginRequest loginRequest)
    {
        Usuario? usuarioParaLogar = await _usuarioRepository.ObterPorEmailAsync(loginRequest.Email);
        if (usuarioParaLogar is null)
        {
            usuarioParaLogar = new Usuario()
            {
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        SignInResult signInResult =
            await _usuarioRepository.CheckCredentials(usuarioParaLogar, loginRequest.Senha);

        if (!signInResult.Succeeded || usuarioParaLogar is null)
        {
            if (signInResult.IsLockedOut)
            {
                throw new LockedException("Essa conta está bloqueada, aguarde e tente novamente.");
            }

            throw new UnauthorizedException("Credenciais inválidas.");
        }

        string jwtToken = _geradorTokenJwt.GerarToken(usuarioParaLogar.Id, usuarioParaLogar.NomeCompleto);

        return usuarioParaLogar.ToRespostaUsuario(jwtToken);
    }
}