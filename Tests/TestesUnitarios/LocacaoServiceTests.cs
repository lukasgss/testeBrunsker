using Application.Common.Exceptions;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Locacoes;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Providers;
using Application.Services.Entidades.Locacoes;
using Domain.Entidades;
using NSubstitute.ReturnsExtensions;
using Tests.TesteUtils.Constantes;
using Tests.TesteUtils.GeradoresEntidades;

namespace Tests.TestesUnitarios;

public class LocacaoServiceTests
{
    private readonly ILocacaoRepository _locacaoRepositoryMock;
    private readonly IImovelRepository _imovelRepositoryMock;
    private readonly IUsuarioRepository _usuarioRepositoryMock;
    private readonly IDateTimeProvider _dateTimeProviderMock;
    private readonly ILocacaoService _sut;

    private readonly Locacao _locacao = GeradorLocacao.GerarLocacao();
    private readonly Locacao _locacaoJaAssinada = GeradorLocacao.GerarLocacaoJaAssinada();
    private readonly CriarLocacaoRequest _criarLocacaoRequest = GeradorLocacao.GerarCriarLocacaoRequest();
    private readonly EditarLocacaoRequest _editarLocacaoRequest = GeradorLocacao.GerarEditarLocacaoRequest();
    private readonly RespostaLocacao _respostaLocacaoEsperada = GeradorLocacao.GerarRespostaLocacao();
    private readonly Imovel _imovel = GeradorImovel.GerarImovel();
    private readonly Usuario _usuario = GeradorUsuario.GerarUsuario();
    private readonly Usuario _locatario = GeradorUsuario.GerarUsuarioComId(Constants.DadosLocacao.LocatarioId);

    public LocacaoServiceTests()
    {
        _locacaoRepositoryMock = Substitute.For<ILocacaoRepository>();
        _imovelRepositoryMock = Substitute.For<IImovelRepository>();
        _usuarioRepositoryMock = Substitute.For<IUsuarioRepository>();
        _dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

        _sut = new LocacaoService(
            _locacaoRepositoryMock,
            _imovelRepositoryMock,
            _usuarioRepositoryMock,
            _dateTimeProviderMock);
    }

    [Fact]
    public async Task Obter_Locacao_Nao_Existente_Por_Id_retorna_NotFoundException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(Constants.DadosLocacao.Id).ReturnsNull();

        async Task Result() => await _sut.ObterPorIdAsync(Constants.DadosLocacao.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Locação com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Obter_Locacao_Por_Id_Retorna_Locacao()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacao);

        RespostaLocacao respostaLocacao = await _sut.ObterPorIdAsync(_locacao.Id);

        Assert.Equivalent(_respostaLocacaoEsperada, respostaLocacao);
    }

    [Fact]
    public async Task Cadastrar_Locacao_Sendo_Locatario_E_Locador_Retorna_BadRequestException()
    {
        CriarLocacaoRequest locacaoRequestSendoLocatarioELocador = new() { IdLocatario = _usuario.Id };

        async Task Result() =>
            await _sut.CadastrarAsync(locacaoRequestSendoLocatarioELocador, _usuario.Id);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não é possível ser locatário e locador da locação, insira outro usuário como locatário.",
            excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Locacao_Com_Imovel_Inexistente_Retorna_NotFoundException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).ReturnsNull();

        async Task Result() => await _sut.CadastrarAsync(_criarLocacaoRequest, Constants.DadosLocacao.LocadorId);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Imóvel com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Locacao_Com_Imovel_Que_Ja_Possui_Locacao_Retorna_ConflictException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_imovel.Id).Returns(_locacao);

        async Task Result() => await _sut.CadastrarAsync(_criarLocacaoRequest, Constants.DadosLocacao.LocadorId);

        var excecao = await Assert.ThrowsAsync<ConflictException>(Result);
        Assert.Equal("Imóvel já possui uma locação cadastrada.", excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Locacao_Sem_Ser_Dono_Do_Imovel_Retorna_UnauthorizedException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_imovel.Id).ReturnsNull();
        const int idUsuarioQueNaoEDono = 99;

        async Task Result() => await _sut.CadastrarAsync(_criarLocacaoRequest, idUsuarioQueNaoEDono);

        var excecao = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível editar locações de imóveis que não é dono.", excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Imovel_Com_Locatario_Inexistente_Retorna_NotFoundException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_imovel.Id).ReturnsNull();
        _usuarioRepositoryMock.ObterPorIdAsync(_criarLocacaoRequest.IdLocatario).ReturnsNull();

        async Task Result() => await _sut.CadastrarAsync(_criarLocacaoRequest, Constants.DadosLocacao.LocadorId);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Usuário com o id especificado para locatário não existe.", excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Locacao_Retorna_Locacao_Cadastrada()
    {
        _imovelRepositoryMock.ObterPorIdAsync(Constants.DadosImovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_imovel.Id).ReturnsNull();
        _usuarioRepositoryMock.ObterPorIdAsync(_criarLocacaoRequest.IdLocatario).Returns(_locatario);
        _usuarioRepositoryMock.ObterPorIdAsync(Constants.DadosLocacao.LocadorId).Returns(_usuario);

        RespostaLocacao respostaLocacao =
            await _sut.CadastrarAsync(_criarLocacaoRequest, Constants.DadosLocacao.LocadorId);
        respostaLocacao.Id = Constants.DadosLocacao.Id;

        Assert.Equivalent(_respostaLocacaoEsperada, respostaLocacao);
    }

    [Fact]
    public async Task Editar_Locacao_Com_Id_Da_Rota_Diferente_Retorna_BadRequestException()
    {
        const int idRotaDiferente = 99;

        async Task Result() => await _sut.EditarAsync(_editarLocacaoRequest, _usuario.Id, idRotaDiferente);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Id da rota não coincide com o id especificado.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Locacao_Sendo_Locatario_E_Locador_Retorna_BadRequestException()
    {
        EditarLocacaoRequest locacaoRequestSendoLocatarioELocador = new() { IdLocatario = _usuario.Id };

        async Task Result() =>
            await _sut.EditarAsync(locacaoRequestSendoLocatarioELocador, _usuario.Id,
                locacaoRequestSendoLocatarioELocador.Id);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não é possível ser locatário e locador da locação, insira outro usuário como locatário.",
            excecao.Message);
    }

    [Fact]
    public async Task Editar_Locacao_Inexistente_Retorna_NotFoundException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).ReturnsNull();

        async Task Result() => await _sut.EditarAsync(_editarLocacaoRequest, _usuario.Id, _editarLocacaoRequest.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Locação com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Locacao_Com_Imovel_Inexistente_Retorna_NotFound_Exception()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacao);
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).ReturnsNull();

        async Task Result() => await _sut.EditarAsync(_editarLocacaoRequest, _usuario.Id, _editarLocacaoRequest.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Imóvel com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Locacao_Com_Imovel_Que_Ja_Possui_Locacao_Retorna_ConflictException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacao);
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_editarLocacaoRequest.IdImovel).Returns(new Locacao());

        async Task Result() => await _sut.EditarAsync(_editarLocacaoRequest, _usuario.Id, _editarLocacaoRequest.Id);

        var excecao = await Assert.ThrowsAsync<ConflictException>(Result);
        Assert.Equal("Imóvel já possui uma locação cadastrada.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Locacao_Em_Que_Nao_E_Locador_Retorna_UnauthorizedException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacao);
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_editarLocacaoRequest.IdImovel).ReturnsNull();
        const int idUsuarioQueNaoELocador = 99;

        async Task Result() =>
            await _sut.EditarAsync(_editarLocacaoRequest, idUsuarioQueNaoELocador, _editarLocacaoRequest.Id);

        var excecao = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível editar locações em que não é o locador.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Locacao_Em_Que_Nao_E_Dono_Retorna_UnauthorizedException()
    {
        const int idUsuarioQueNaoEDono = 99;
        Locacao locacao = new() { Locador = new() { Id = idUsuarioQueNaoEDono } };
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(locacao);
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_editarLocacaoRequest.IdImovel).ReturnsNull();

        async Task Result() =>
            await _sut.EditarAsync(_editarLocacaoRequest, idUsuarioQueNaoEDono, _editarLocacaoRequest.Id);

        var excecao = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível editar locações de imóveis que não é dono.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Locacao_Com_Locatario_Inexistente_Retorna_NotFoundException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacao);
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_editarLocacaoRequest.IdImovel).ReturnsNull();
        _usuarioRepositoryMock.ObterPorIdAsync(_locatario.Id).ReturnsNull();

        async Task Result() => await _sut.EditarAsync(_editarLocacaoRequest, _usuario.Id, _editarLocacaoRequest.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Usuário com o id especificado para locatário não existe.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Locacao_Retorna_Locacao_Editada()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacao);
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);
        _locacaoRepositoryMock.ObterPorIdDoImovelAsync(_editarLocacaoRequest.IdImovel).ReturnsNull();
        _usuarioRepositoryMock.ObterPorIdAsync(_locatario.Id).Returns(_locatario);

        RespostaLocacao respostaLocacao =
            await _sut.EditarAsync(_editarLocacaoRequest, _usuario.Id, _editarLocacaoRequest.Id);

        Assert.Equivalent(_respostaLocacaoEsperada, respostaLocacao);
    }

    [Fact]
    public async Task Excluir_Locacao_Inexistente_Retorna_NotFoundException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).ReturnsNull();

        async Task Result() => await _sut.DeletarAsync(_locacao.Id, _usuario.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Locação com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Excluir_Locacao_Em_Que_Nao_E_Dono_Retorna_UnauthorizedException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacao);
        const int idUsuarioQueNaoEDono = 99;

        async Task Result() => await _sut.DeletarAsync(_locacao.Id, idUsuarioQueNaoEDono);

        var excecao = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível excluir locações de imóveis que não é dono.", excecao.Message);
    }

    [Fact]
    public async Task Assinar_Locacao_Inexistente_Retorna_NotFoundException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).ReturnsNull();

        async Task Result() => await _sut.AssinarAsync(_locacao.Id, _usuario.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Locação com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Assinar_Locacao_Sendo_Locador_Ja_Tendo_Assinado_Retorna_BadRequestException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacaoJaAssinada);

        async Task Result() => await _sut.AssinarAsync(_locacaoJaAssinada.Id, _locacaoJaAssinada.Locador.Id);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Locação já foi assinada por você, não é possível assiná-la novamente.", excecao.Message);
    }

    [Fact]
    public async Task Assinar_Locacao_Sendo_Locatario_Ja_Tendo_Assinado_Retorna_BadRequestException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacaoJaAssinada);

        async Task Result() => await _sut.AssinarAsync(_locacaoJaAssinada.Id, _locacaoJaAssinada.Locatario.Id);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Locação já foi assinada por você, não é possível assiná-la novamente.", excecao.Message);
    }

    [Fact]
    public async Task Assinar_Locacao_Em_Que_Nao_E_Locador_Ou_Locatario_Retorna_UnauthorizedException()
    {
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(_locacao);
        const int idUsuarioNaoLocatarioOuLocador = 99;

        async Task Result() => await _sut.AssinarAsync(_locacaoJaAssinada.Id, idUsuarioNaoLocatarioOuLocador);

        var excecao = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível assinar locações em que não é locador ou locatário.", excecao.Message);
    }

    [Fact]
    public async Task Assinar_Locacao_Retorna_Locacao_Assinada()
    {
        Locacao locacaoJaAssinadaPeloLocador = GeradorLocacao.GerarLocacaoJaAssinadaPeloLocador();
        _locacaoRepositoryMock.ObterPorIdAsync(_locacao.Id).Returns(locacaoJaAssinadaPeloLocador);
        _dateTimeProviderMock.UtcNow().Returns(Constants.DadosLocacao.DataFechamentoLocacaoAssinada);
        RespostaLocacao respostaLocacaoEsperada = GeradorLocacao.GerarRespostaLocacaoAssinada();

        RespostaLocacao respostaLocacao = await _sut.AssinarAsync(_locacao.Id, _locatario.Id);

        Assert.Equivalent(respostaLocacaoEsperada, respostaLocacao);
    }
}