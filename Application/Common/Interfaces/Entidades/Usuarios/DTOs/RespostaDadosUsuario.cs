namespace Application.Common.Interfaces.Entidades.Usuarios.DTOs;

public class RespostaDadosUsuario
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string NomeCompleto { get; set; } = null!;
    public string Telefone { get; set; } = null!;
}