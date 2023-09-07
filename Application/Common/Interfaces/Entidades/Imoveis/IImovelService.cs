using Application.Common.Interfaces.Entidades.Imoveis.DTOs;

namespace Application.Common.Interfaces.Entidades.Imoveis;

public interface IImovelService
{
    Task<RespostaImovel> ObterPorIdAsync(int imovelId);
    Task<RespostaImovel> CadastrarAsync(CriarImovelRequest criarImovelRequest, int idUsuario);
    Task<RespostaImovel> EditarAsync(EditarImovelRequest editarImovelRequest, int idUsuario, int idRota);
    Task DeletarAsync(int idUsuario, int idImovel);
}