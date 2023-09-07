using Application.Common.Interfaces.Entidades.Locacoes.DTOs;

namespace Application.Common.Interfaces.Entidades.Locacoes;

public interface ILocacaoService
{
    Task<RespostaLocacao> ObterPorIdAsync(int locacaoId);
    Task<RespostaLocacao> CadastrarAsync(CriarLocacaoRequest criarLocacaoRequest, int idLocador);
    Task<RespostaLocacao> EditarAsync(EditarLocacaoRequest editarLocacaoRequest, int idLocador, int idRota);
    Task DeletarAsync(int idLocacao, int idUsuario);
}