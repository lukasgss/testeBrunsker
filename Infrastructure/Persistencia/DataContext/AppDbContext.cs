using Domain.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistencia.DataContext;

public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Imovel> Imoveis { get; set; } = null!;
    public DbSet<Locacao> Locacoes { get; set; } = null!;
}