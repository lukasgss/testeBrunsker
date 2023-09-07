using Application.Common.Interfaces.Entidades.Imoveis;
using Domain.Entidades;
using Infrastructure.Persistencia.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistencia.Repositorios;

public class ImovelRepository : GenericRepository<Imovel>, IImovelRepository
{
    private readonly AppDbContext _dbContext;
    
    public ImovelRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Imovel?> ObterPorId(int idImovel)
    {
        return await _dbContext.Imoveis
            .Include(imovel => imovel.Dono)
            .SingleOrDefaultAsync(imovel => imovel.Id == idImovel);
    }

    public async Task<List<Imovel>> ObterPorCep(string cep)
    {
        return await _dbContext.Imoveis
            .AsNoTracking()
            .Where(imovel => imovel.Cep == cep)
            .ToListAsync();
    }
}