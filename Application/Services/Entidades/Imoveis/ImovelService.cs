using System.Net;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.ApisExternas.ViaCep;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios;
using Domain.Entidades;
using Domain.Formatadores;

namespace Application.Services.Entidades.Imoveis;

public class ImovelService : IImovelService
{
    private readonly IImovelRepository _imovelRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IViaCepClient _viaCepClient;

    public ImovelService(IImovelRepository imovelRepository,
        IUsuarioRepository usuarioRepository,
        IViaCepClient viaCepClient)
    {
        _imovelRepository = imovelRepository ?? throw new ArgumentNullException(nameof(imovelRepository));
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        _viaCepClient = viaCepClient ?? throw new ArgumentNullException(nameof(viaCepClient));
    }

    public async Task<RespostaImovel> ObterPorIdAsync(int imovelId)
    {
        Imovel? imovel = await _imovelRepository.ObterPorId(imovelId);
        if (imovel is null)
        {
            throw new NotFoundException("Imóvel com o id especificado não existe.");
        }

        return imovel.ToRespostaImovel();
    }

    public async Task<RespostaImovel> CadastrarAsync(CriarImovelRequest criarImovelRequest, int idUsuario)
    {
        string cepFormatado = FormatadorCep.Formatar(criarImovelRequest.Cep);
        await ValidarImovelParaCadastroAsync(criarImovelRequest, cepFormatado);

        var usuarioDono = await _usuarioRepository.ObterPorIdAsync(idUsuario);

        try
        {
            RespostaViaCep? dadosCep = await _viaCepClient.ObterEnderecoPorCep(criarImovelRequest.Cep);

            if (dadosCep?.Cep is null)
            {
                throw new BadRequestException("O cep informado não é válido.");
            }

            Imovel imovelParaCadastrar = new()
            {
                Cep = cepFormatado,
                Endereco = dadosCep.Logradouro!,
                Numero = criarImovelRequest.Numero,
                Complemento = criarImovelRequest.Complemento,
                Dono = usuarioDono!,
            };

            _imovelRepository.Add(imovelParaCadastrar);
            await _imovelRepository.CommitAsync();

            return imovelParaCadastrar.ToRespostaImovel();
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new BadRequestException("Formato do cep inválido, insira um cep válido e tente novamente.");
            }

            throw new BadRequestException(
                "Não foi possível obter os dados do CEP, tente novamente mais tarde.",
                exception);
        }
    }

    public async Task<RespostaImovel> EditarAsync(EditarImovelRequest editarImovelRequest, int idUsuario, int idRota)
    {
        if (editarImovelRequest.Id != idRota)
        {
            throw new BadRequestException("Id da rota não coincide com o id especificado.");
        }

        Imovel? imovelDb = await _imovelRepository.ObterPorId(editarImovelRequest.Id);
        if (imovelDb is null)
        {
            throw new NotFoundException("Imóvel com o id especificado não foi encontrado.");
        }

        if (imovelDb.Dono.Id != idUsuario)
        {
            throw new UnauthorizedException("Não é possível editar imóveis no qual não é dono.");
        }

        string cepFormatado = FormatadorCep.Formatar(editarImovelRequest.Cep);
        await ValidarImovelParaEdicaoAsync(editarImovelRequest, cepFormatado);

        try
        {
            RespostaViaCep? dadosCep = await _viaCepClient.ObterEnderecoPorCep(editarImovelRequest.Cep);

            if (dadosCep?.Cep is null)
            {
                throw new BadRequestException("O cep informado não é válido.");
            }

            imovelDb.Cep = cepFormatado;
            imovelDb.Endereco = dadosCep.Logradouro!;
            imovelDb.Numero = editarImovelRequest.Numero;
            imovelDb.Complemento = editarImovelRequest.Complemento;

            await _imovelRepository.CommitAsync();

            return imovelDb.ToRespostaImovel();
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new BadRequestException("Cep inválido, insira um cep válido e tente novamente.");
            }

            throw new BadRequestException(
                "Não foi possível obter os dados do CEP, tente novamente mais tarde.",
                exception);
        }
    }

    public async Task DeletarAsync(int idUsuario, int idImovel)
    {
        Imovel? imovelDb = await _imovelRepository.ObterPorId(idImovel);
        if (imovelDb is null)
        {
            throw new NotFoundException("Imóvel com o id especificado não existe.");
        }

        if (imovelDb.Dono.Id != idUsuario)
        {
            throw new UnauthorizedException("Não é possível excluir imóveis no qual não é dono.");
        }

        _imovelRepository.Delete(imovelDb);
        await _imovelRepository.CommitAsync();
    }

    private async Task ValidarImovelParaCadastroAsync(CriarImovelRequest criarImovelRequest, string cepFormatado)
    {
        List<Imovel> imoveis = await _imovelRepository.ObterPorCep(cepFormatado);

        if (ImovelJaExiste(imoveis, criarImovelRequest.Numero, criarImovelRequest?.Complemento))
        {
            throw new ConflictException("Imóvel já foi cadastrado.");
        }
    }

    private async Task ValidarImovelParaEdicaoAsync(EditarImovelRequest editarImovelRequest, string cepFormatado)
    {
        List<Imovel> imoveis = await _imovelRepository.ObterPorCep(cepFormatado);

        if (ImovelJaExiste(imoveis, editarImovelRequest.Numero, editarImovelRequest?.Complemento))
        {
            throw new ConflictException("Imóvel com esses dados já foi cadastrado.");
        }
    }

    private bool ImovelJaExiste(List<Imovel> imoveis, int numero, string? complemento)
    {
        return imoveis.Any(imovel =>
            imovel.Numero == numero &&
            imovel.Complemento == complemento);
    }
}