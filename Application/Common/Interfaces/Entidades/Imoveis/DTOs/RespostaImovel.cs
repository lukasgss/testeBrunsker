using Application.Common.Interfaces.Entidades.Usuarios.DTOs;

namespace Application.Common.Interfaces.Entidades.Imoveis.DTOs;

public class RespostaImovel
{
    public int Id { get; set; }
    public string Cep { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string Bairro { get; set; } = null!;
    public int Numero { get; set; }
    public string? Complemento { get; set; }
    public RespostaDadosUsuario Dono { get; set; } = null!;
}