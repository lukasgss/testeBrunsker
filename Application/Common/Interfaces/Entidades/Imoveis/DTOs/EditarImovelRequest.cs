namespace Application.Common.Interfaces.Entidades.Imoveis.DTOs;

public class EditarImovelRequest
{
    public int Id { get; set; }
    public string Cep { get; set; } = null!;
    public int Numero { get; set; }
    public string? Complemento { get; set; }
}