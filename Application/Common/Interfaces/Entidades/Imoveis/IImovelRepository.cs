using Application.Common.Interfaces.GenericRepository;
using Domain.Entidades;

namespace Application.Common.Interfaces.Entidades.Imoveis;

public interface IImovelRepository : IGenericRepository<Imovel>
{
    Task<Imovel?> ObterPorId(int idImovel);
    Task<List<Imovel>> ObterPorCep(string cep);
}