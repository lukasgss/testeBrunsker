using Application.Common.Exceptions;
using Application.Common.Interfaces.Autenticacao;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Entidades.Usuarios.DTOs;
using Application.Services.Entidades.Usuarios;
using Domain.Entidades;
using Microsoft.AspNetCore.Identity;
using NSubstitute.ReturnsExtensions;
using Tests.TesteUtils.Constantes;
using Tests.TesteUtils.Fakes;
using Tests.TesteUtils.GeradoresEntidades;

namespace Tests.TestesUnitarios;

public class UsuarioServiceTests
{
    private readonly IUsuarioRepository _usuarioRepositoryMock;
    private readonly IGeradorTokenJwt _geradorTokenJwtMock;
    private readonly IUsuarioService _sut;

    private readonly Usuario _usuario = GeradorUsuario.GerarUsuario();
    private readonly RespostaDadosUsuario _respostaDadosUsuario = GeradorUsuario.GerarRespostaDadosUsuario();
    private readonly RespostaUsuario _respostaUsuario = GeradorUsuario.GerarRespostaUsuario(); 
    private readonly CriarUsuarioRequest _criarUsuarioRequest = GeradorUsuario.GerarCriarUsuarioRequest();
    private readonly LoginRequest _loginRequest = GeradorUsuario.GerarLoginRequest();

    public UsuarioServiceTests()
    {
        _usuarioRepositoryMock = Substitute.For<IUsuarioRepository>();
        _geradorTokenJwtMock = Substitute.For<IGeradorTokenJwt>();

        _sut = new UsuarioService(_usuarioRepositoryMock, _geradorTokenJwtMock);
    }

    [Fact]
    public async Task Obter_Usuario_Nao_Existente_Por_Id_Retorna_NotFoundException()
    {
        _usuarioRepositoryMock.ObterPorIdAsync(Constants.DadosUsuario.Id).ReturnsNull();

        async Task Result() => await _sut.ObterUsuarioPorId(Constants.DadosUsuario.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Usuário com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Obter_Usuario_Por_Id_Retorna_Usuario()
    {
        _usuarioRepositoryMock.ObterPorIdAsync(_usuario.Id).Returns(_usuario);

        RespostaDadosUsuario usuarioRetornado = await _sut.ObterUsuarioPorId(_usuario.Id);

        Assert.Equivalent(_respostaDadosUsuario, usuarioRetornado);
    }

    [Fact]
    public async Task Cadastrar_Usuario_Com_Email_Ja_Existente_Retorna_ConflictException()
    {
        _usuarioRepositoryMock.ObterPorEmailAsync(_usuario.Email).Returns(_usuario);

        async Task Result() => await _sut.RegistrarAsync(_criarUsuarioRequest);

        var excecao = await Assert.ThrowsAsync<ConflictException>(Result);
        Assert.Equal("Usuário com o e-mail especificado já existe.", excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Usuario_Com_Algum_Erro_De_Registro_Retorna_InternalServerErrorException()
    {
        _usuarioRepositoryMock.ObterPorEmailAsync(_usuario.Email).ReturnsNull();
        IdentityResult identityResultEsperada = new FakeIdentityResult(succeeded: false);
        _usuarioRepositoryMock.RegistrarAsync(Arg.Any<Usuario>(), _criarUsuarioRequest.Senha)
            .Returns(identityResultEsperada);
        _usuarioRepositoryMock.SetLockoutEnabledAsync(Arg.Any<Usuario>(), false).Returns(identityResultEsperada);

        async Task Result() => await _sut.RegistrarAsync(_criarUsuarioRequest);

        await Assert.ThrowsAsync<InternalServerErrorException>(Result);
    }

    [Fact]
    public async Task Cadastrar_Usuario_Retorna_Resposta_Usuario()
    {
        _usuarioRepositoryMock.ObterPorEmailAsync(_usuario.Email).ReturnsNull();
        IdentityResult identityResultEsperada = new FakeIdentityResult(succeeded: true);
        _usuarioRepositoryMock.RegistrarAsync(Arg.Any<Usuario>(), _criarUsuarioRequest.Senha)
            .Returns(identityResultEsperada);
        _usuarioRepositoryMock.SetLockoutEnabledAsync(Arg.Any<Usuario>(), false).Returns(identityResultEsperada);
        _geradorTokenJwtMock.GerarToken(Arg.Any<int>(), _usuario.NomeCompleto).Returns(Constants.DadosUsuario.JwtToken);

        RespostaUsuario respostaUsuario = await _sut.RegistrarAsync(_criarUsuarioRequest);
        respostaUsuario.Id = Constants.DadosUsuario.Id;
        
        Assert.Equivalent(_respostaUsuario, respostaUsuario);
    }

    [Fact]
    public async Task Login_Com_Conta_Bloqueada_Retorna_LockedException()
    {
        _usuarioRepositoryMock.ObterPorEmailAsync(_usuario.Email).Returns(_usuario);
        SignInResult fakeSignInResult = new FakeSignInResult(succeeded: false, isLockedOut: true);
        _usuarioRepositoryMock.CheckCredentials(_usuario, _loginRequest.Senha).Returns(fakeSignInResult);

        async Task Result() => await _sut.LoginAsync(_loginRequest);

        var excecao = await Assert.ThrowsAsync<LockedException>(Result);
        Assert.Equal("Essa conta está bloqueada, aguarde e tente novamente.", excecao.Message);
    }

    [Fact]
    public async Task Login_Com_Credenciais_Invalidas_Retorna_UnauthorizedException()
    {
        _usuarioRepositoryMock.ObterPorEmailAsync(_usuario.Email).Returns(_usuario);
        SignInResult fakeSignInResult = new FakeSignInResult(succeeded: false, isLockedOut: false);
        _usuarioRepositoryMock.CheckCredentials(_usuario, _loginRequest.Senha).Returns(fakeSignInResult);

        async Task Result() => await _sut.LoginAsync(_loginRequest);

        var excecao = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Credenciais inválidas.", excecao.Message);
    }

    [Fact]
    public async Task Login_Retorna_RespostaUsuario()
    {
        _usuarioRepositoryMock.ObterPorEmailAsync(_usuario.Email).Returns(_usuario);
        SignInResult fakeSignInResult = new FakeSignInResult(succeeded: true, isLockedOut: false);
        _usuarioRepositoryMock.CheckCredentials(_usuario, _loginRequest.Senha).Returns(fakeSignInResult);
        _geradorTokenJwtMock.GerarToken(Arg.Any<int>(), _usuario.NomeCompleto).Returns(Constants.DadosUsuario.JwtToken);

        RespostaUsuario respostaUsuario = await _sut.LoginAsync(_loginRequest);
        
        Assert.Equivalent(_respostaUsuario, respostaUsuario);
    }
}