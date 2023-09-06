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
        await ValidarImovelAsync(criarImovelRequest, cepFormatado);

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
                Endereco = dadosCep.Logradouro,
                Numero = criarImovelRequest.Numero,
                Complemento= criarImovelRequest.Complemento,
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
                throw new BadRequestException("Cep inválido, insira um cep válido e tente novamente.");
            }
            
            throw new BadRequestException(
                "Não foi possível obter os dados do CEP, tente novamente mais tarde.",
                exception);
        }
    }

    private async Task ValidarImovelAsync(CriarImovelRequest criarImovelRequest, string cepFormatado)
    {
        List<Imovel> imoveis = await _imovelRepository.ObterPorCep(cepFormatado);
        
        if (imoveis.Any(imovel =>
                imovel.Numero == criarImovelRequest.Numero &&
                imovel.Complemento == criarImovelRequest.Complemento))
        {
            throw new ConflictException("Imóvel já foi cadastrado.");
        }
    }
}