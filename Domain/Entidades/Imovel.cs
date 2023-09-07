using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entidades;

public class Imovel
{
    public int Id { get; set; }
    
    [Required, MaxLength(255)]
    public string Endereco { get; set; } = null!;
    
    [Required, MaxLength(20)]
    public string Cep { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Cidade { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Estado { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Bairro { get; set; } = null!;
    
    [Required] 
    public int Numero { get; set; }

    // Propriedade adicionada caso o imóvel seja um apartamento.
    // Caso não seja, seu valor é nulo.
    [MaxLength(20)]
    public string? Complemento { get; set; }
    
    [ForeignKey("DonoId")]
    public virtual Usuario Dono { get; set; } = null!;
    public int DonoId { get; set; }
}