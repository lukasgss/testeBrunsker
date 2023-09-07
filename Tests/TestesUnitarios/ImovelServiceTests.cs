using System.Collections.Generic;
using System.Net.Http;
using Application.Common.Exceptions;
using Application.Common.Interfaces.ApisExternas.ViaCep;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Services.Entidades.Imoveis;
using Domain.Entidades;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Tests.TesteUtils.Constantes;
using Tests.TesteUtils.GeradoresEntidades;

namespace Tests.TestesUnitarios;

public class ImovelServiceTests
{
    private readonly IImovelRepository _imovelRepositoryMock;
    private readonly IUsuarioRepository _usuarioRepositoryMock;
    private readonly IViaCepClient _viaCepClientMock;
    private readonly IImovelService _sut;

    private readonly Imovel _imovel = GeradorImovel.GerarImovel();
    private readonly List<Imovel> _listaImoveis = GeradorImovel.GerarListaImoveis();
    private readonly CriarImovelRequest _criarImovelRequest = GeradorImovel.GerarCriarImovelRequest();
    private readonly EditarImovelRequest _editarImovelRequest = GeradorImovel.GerarEditarImovelRequest();
    private readonly RespostaImovel _respostaImovel = GeradorImovel.GerarRespostaImovel();
    private readonly RespostaViaCep _respostaViaCep = GeradorCep.GerarRespostaViaCep();
    private readonly Usuario _usuario = GeradorUsuario.GerarUsuario();

    public ImovelServiceTests()
    {
        _imovelRepositoryMock = Substitute.For<IImovelRepository>();
        _usuarioRepositoryMock = Substitute.For<IUsuarioRepository>();
        _viaCepClientMock = Substitute.For<IViaCepClient>();

        _sut = new ImovelService(_imovelRepositoryMock, _usuarioRepositoryMock, _viaCepClientMock);
    }

    [Fact]
    public async Task Obter_Imovel_Nao_Existente_Por_Id_Retorna_NotFoundException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).ReturnsNull();

        async Task Result() => await _sut.ObterPorIdAsync(_imovel.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Imóvel com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Obter_Imovel_Por_Id_Retorna_Imovel()
    {
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);

        RespostaImovel respostaImovel = await _sut.ObterPorIdAsync(_imovel.Id);

        Assert.Equivalent(_respostaImovel, respostaImovel);
    }

    [Fact]
    public async Task Cadastrar_Imovel_Ja_Cadastrado_Retorna_ConflictException()
    {
        _imovelRepositoryMock.ObterPorCep(_imovel.Cep).Returns(_listaImoveis);

        async Task Result() => await _sut.CadastrarAsync(_criarImovelRequest, Constants.DadosUsuario.Id);

        var excecao = await Assert.ThrowsAsync<ConflictException>(Result);
        Assert.Equal("Imóvel já foi cadastrado.", excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Imovel_Com_Cep_Invalido_Retorna_BadRequestException()
    {
        List<Imovel> imoveisJaExistentes = new List<Imovel>()
        {
            GeradorImovel.GerarImovelComComplementoENumero(complemento: "222", numero: 70)
        };
        _imovelRepositoryMock.ObterPorCep(_imovel.Cep).Returns(imoveisJaExistentes);
        RespostaViaCep respostaViaCepInvalido = GeradorCep.GerarRespostaViaCepInvalida();
        _viaCepClientMock.ObterEnderecoPorCep(_criarImovelRequest.Cep).Returns(respostaViaCepInvalido);

        async Task Result() => await _sut.CadastrarAsync(_criarImovelRequest, Constants.DadosUsuario.Id);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("O cep informado não é válido.", excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Imovel_Com_Formato_Do_Cep_Invalido_Retorna_BadRequestException()
    {
        List<Imovel> imoveisJaExistentes = new List<Imovel>()
        {
            GeradorImovel.GerarImovelComComplementoENumero(complemento: "222", numero: 70)
        };
        _imovelRepositoryMock.ObterPorCep(_imovel.Cep).Returns(imoveisJaExistentes);
        _viaCepClientMock.ObterEnderecoPorCep(_criarImovelRequest.Cep).ThrowsAsync<HttpRequestException>();

        async Task Result() => await _sut.CadastrarAsync(_criarImovelRequest, Constants.DadosUsuario.Id);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não foi possível obter os dados do CEP, tente novamente mais tarde.", excecao.Message);
    }

    [Fact]
    public async Task Cadastrar_Imovel_Retorna_Resposta_Imovel()
    {
        List<Imovel> imoveisJaExistentes = new List<Imovel>()
        {
            GeradorImovel.GerarImovelComComplementoENumero(complemento: "222", numero: 70)
        };
        _imovelRepositoryMock.ObterPorCep(_imovel.Cep).Returns(imoveisJaExistentes);
        _usuarioRepositoryMock.ObterPorIdAsync(Constants.DadosUsuario.Id).Returns(_usuario);
        _viaCepClientMock.ObterEnderecoPorCep(_respostaViaCep.Cep!).Returns(_respostaViaCep);

        RespostaImovel respostaImovel = await _sut.CadastrarAsync(_criarImovelRequest, Constants.DadosUsuario.Id);
        respostaImovel.Id = Constants.DadosImovel.Id;

        Assert.Equivalent(_respostaImovel, respostaImovel);
    }

    [Fact]
    public async Task Editar_Imovel_Com_Id_Diferente_Do_Especificado_Na_Rota_Retorna_BadRequestException()
    {
        async Task Result() => await _sut.EditarAsync(
            _editarImovelRequest,
            idUsuario: Constants.DadosImovel.Id,
            idRota: 99);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Id da rota não coincide com o id especificado.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Imovel_Nao_Existente_Retorna_NotFoundException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(_editarImovelRequest.Id).ReturnsNull();

        async Task Result() => await _sut.EditarAsync(
            _editarImovelRequest,
            idUsuario: Constants.DadosUsuario.Id,
            idRota: _editarImovelRequest.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Imóvel com o id especificado não foi encontrado.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Imovel_Sem_Ser_O_Dono_Retorna_UnauthorizedException()
    {
        const int idUsuarioNaoExistente = 999;
        _imovelRepositoryMock.ObterPorIdAsync(_editarImovelRequest.Id).Returns(_imovel);

        async Task Result() => await _sut.EditarAsync(
            _editarImovelRequest,
            idUsuario: idUsuarioNaoExistente,
            idRota: _editarImovelRequest.Id);

        var excecao = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível editar imóveis no qual não é dono.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Imovel_Para_Imovel_Ja_Existente_Retorna_ConflictException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(_editarImovelRequest.Id).Returns(_imovel);
        _imovelRepositoryMock.ObterPorCep(_editarImovelRequest.Cep).Returns(_listaImoveis);

        async Task Result() => await _sut.EditarAsync(
            _editarImovelRequest,
            idUsuario: Constants.DadosUsuario.Id,
            idRota: _editarImovelRequest.Id);

        var excecao = await Assert.ThrowsAsync<ConflictException>(Result);
        Assert.Equal("Imóvel com esses dados já foi cadastrado.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Imovel_Com_Cep_Invalido_Retorna_BadRequestException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(_editarImovelRequest.Id).Returns(_imovel);
        List<Imovel> imoveisJaExistentes = new List<Imovel>()
        {
            GeradorImovel.GerarImovelComComplementoENumero(complemento: "222", numero: 70)
        };
        _imovelRepositoryMock.ObterPorCep(_editarImovelRequest.Cep).Returns(imoveisJaExistentes);
        RespostaViaCep respostaViaCepInvalido = GeradorCep.GerarRespostaViaCepInvalida();
        _viaCepClientMock.ObterEnderecoPorCep(_criarImovelRequest.Cep).Returns(respostaViaCepInvalido);

        async Task Result() => await _sut.EditarAsync(
            _editarImovelRequest,
            idUsuario: Constants.DadosUsuario.Id,
            idRota: _editarImovelRequest.Id);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("O cep informado não é válido.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Imovel_Com_Formato_Do_Cep_Invalido_Retorna_BadRequestException()
    {
        List<Imovel> imoveisJaExistentes = new List<Imovel>()
        {
            GeradorImovel.GerarImovelComComplementoENumero(complemento: "222", numero: 70)
        };
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);
        _imovelRepositoryMock.ObterPorCep(_imovel.Cep).Returns(imoveisJaExistentes);
        _viaCepClientMock.ObterEnderecoPorCep(_editarImovelRequest.Cep).ThrowsAsync<HttpRequestException>();

        async Task Result() => await _sut.EditarAsync(
            _editarImovelRequest,
            idUsuario: Constants.DadosUsuario.Id,
            idRota: _editarImovelRequest.Id);

        var excecao = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não foi possível obter os dados do CEP, tente novamente mais tarde.", excecao.Message);
    }

    [Fact]
    public async Task Editar_Imovel_Retorna_Imovel_Editado()
    {
        List<Imovel> imoveisJaExistentes = new List<Imovel>()
        {
            GeradorImovel.GerarImovelComComplementoENumero(complemento: "222", numero: 70)
        };
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);
        _imovelRepositoryMock.ObterPorCep(_imovel.Cep).Returns(imoveisJaExistentes);
        _usuarioRepositoryMock.ObterPorIdAsync(Constants.DadosUsuario.Id).Returns(_usuario);
        _viaCepClientMock.ObterEnderecoPorCep(_respostaViaCep.Cep!).Returns(_respostaViaCep);

        RespostaImovel respostaImovel = await _sut.EditarAsync(
            _editarImovelRequest,
            idUsuario: Constants.DadosUsuario.Id,
            _editarImovelRequest.Id);

        Assert.Equivalent(_respostaImovel, respostaImovel);
    }

    [Fact]
    public async Task Deletar_Imovel_Nao_Existente_Retorna_NotFoundException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).ReturnsNull();

        async Task Result() =>
            await _sut.DeletarAsync(idUsuario: Constants.DadosUsuario.Id, idImovel: Constants.DadosImovel.Id);

        var excecao = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Imóvel com o id especificado não existe.", excecao.Message);
    }

    [Fact]
    public async Task Deletar_Imovel_Sem_Ser_O_Dono_Retorna_UnauthorizedException()
    {
        _imovelRepositoryMock.ObterPorIdAsync(_imovel.Id).Returns(_imovel);
        const int idUsuarioQueNaoEDono = 88;

        async Task Result() =>
            await _sut.DeletarAsync(idUsuario: idUsuarioQueNaoEDono, idImovel: Constants.DadosImovel.Id);

        var excecao = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível excluir imóveis no qual não é dono.", excecao.Message);
    }
}