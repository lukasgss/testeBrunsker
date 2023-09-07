using Application.Common.Exceptions;
using Application.Common.Extensions.Mapeamento;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Locacoes;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Providers;
using Domain.Entidades;

namespace Application.Services.Entidades.Locacoes;

public class LocacaoService : ILocacaoService
{
    private readonly ILocacaoRepository _locacaoRepository;
    private readonly IImovelRepository _imovelRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LocacaoService(
        ILocacaoRepository locacaoRepository,
        IImovelRepository imovelRepository,
        IUsuarioRepository usuarioRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _locacaoRepository = locacaoRepository ?? throw new ArgumentNullException(nameof(locacaoRepository));
        _imovelRepository = imovelRepository ?? throw new ArgumentNullException(nameof(imovelRepository));
        _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<RespostaLocacao> ObterPorIdAsync(int locacaoId)
    {
        Locacao? locacao = await _locacaoRepository.ObterPorIdAsync(locacaoId);
        if (locacao is null)
        {
            throw new NotFoundException("Locação com o id especificado não existe.");
        }

        return locacao.ToRespostaLocacao();
    }

    private async Task ValidarSeImovelJaPossuiLocacaoAsync(int idImovel)
    {
        Locacao? locacaoDb = await _locacaoRepository.ObterPorIdDoImovelAsync(idImovel);
        if (locacaoDb is not null)
        {
            throw new ConflictException("Imóvel já possui uma locação cadastrada.");
        }
    }

    public async Task<RespostaLocacao> CadastrarAsync(CriarLocacaoRequest criarLocacaoRequest, int idLocador)
    {
        Imovel imovel = await ValidarEObterImovelAsync(criarLocacaoRequest.IdImovel);

        await ValidarSeImovelJaPossuiLocacaoAsync(criarLocacaoRequest.IdImovel);

        ValidarSeUsuarioEDonoDoImovel(imovel, idLocador);

        Usuario locatario = await ValidarEObterLocatario(criarLocacaoRequest.IdLocatario);
        Usuario? locador = await _usuarioRepository.ObterPorIdAsync(idLocador);

        Locacao locacaoParaCriar = new()
        {
            Imovel = imovel,
            Locador = locador!,
            Locatario = locatario,
            LocadorAssinou = false,
            LocatarioAssinou = false,
            DataFechamento = _dateTimeProvider.UtcNow(),
            DataVencimento = criarLocacaoRequest.DataVencimento,
            ValorMensal = criarLocacaoRequest.ValorMensal
        };

        _locacaoRepository.Add(locacaoParaCriar);
        await _locacaoRepository.CommitAsync();

        return locacaoParaCriar.ToRespostaLocacao();
    }

    public async Task<RespostaLocacao> EditarAsync(EditarLocacaoRequest editarLocacaoRequest, int idLocador, int idRota)
    {
        if (editarLocacaoRequest.Id != idRota)
        {
            throw new BadRequestException("Id da rota não coincide com o id especificado.");
        }

        Locacao locacaoDb = await ValidarEObterLocacaoAsync(editarLocacaoRequest.Id);
        Imovel imovel = await ValidarEObterImovelAsync(editarLocacaoRequest.IdImovel);

        await ValidarSeImovelJaPossuiLocacaoAsync(editarLocacaoRequest.IdImovel);
        ValidarSeUsuarioEDonoDoImovel(imovel, idLocador);

        Usuario locatario = await ValidarEObterLocatario(editarLocacaoRequest.IdLocatario);

        locacaoDb.Imovel = imovel;
        locacaoDb.Locatario = locatario;
        locacaoDb.DataVencimento = editarLocacaoRequest.DataVencimento;
        locacaoDb.ValorMensal = editarLocacaoRequest.ValorMensal;
        // Quando o locatário alterar dados da locação, é necessário que ambos assinem novamente
        locacaoDb.LocadorAssinou = false;
        locacaoDb.LocatarioAssinou = false;

        await _locacaoRepository.CommitAsync();

        return locacaoDb.ToRespostaLocacao();
    }

    public async Task DeletarAsync(int idLocacao, int idUsuario)
    {
        Locacao locacao = await ValidarEObterLocacaoAsync(idLocacao);
        if (locacao.Imovel.Dono.Id != idUsuario)
        {
            throw new UnauthorizedException("Não é possível excluir locações de imóveis que não é dono.");
        }

        _locacaoRepository.Delete(locacao);
        await _locacaoRepository.CommitAsync();
    }

    private async Task<Locacao> ValidarEObterLocacaoAsync(int idLocacao)
    {
        Locacao? locacao = await _locacaoRepository.ObterPorIdAsync(idLocacao);
        if (locacao is null)
        {
            throw new NotFoundException("Locação com o id especificado não existe.");
        }

        return locacao;
    }

    private async Task<Imovel> ValidarEObterImovelAsync(int idImovel)
    {
        Imovel? imovel = await _imovelRepository.ObterPorIdAsync(idImovel);
        if (imovel is null)
        {
            throw new NotFoundException("Imóvel com o id especificado não existe.");
        }

        return imovel;
    }

    private void ValidarSeUsuarioEDonoDoImovel(Imovel imovel, int idUsuario)
    {
        if (imovel.Dono.Id != idUsuario)
        {
            throw new UnauthorizedException("Não é possível editar locações de imóveis que não é dono.");
        }
    }

    private async Task<Usuario> ValidarEObterLocatario(int idLocatario)
    {
        Usuario? locatario = await _usuarioRepository.ObterPorIdAsync(idLocatario);
        if (locatario is null)
        {
            throw new NotFoundException("Usuário com o id especificado para locatário não existe.");
        }

        return locatario;
    }
}