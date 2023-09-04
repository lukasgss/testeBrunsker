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
    
    [ForeignKey("DonoId")]
    public virtual Usuario Dono { get; set; } = null!;
    public int DonoId { get; set; }
}