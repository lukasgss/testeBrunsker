using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entidades;

public class Usuario : IdentityUser<int>
{
    [Required, MaxLength(255)] 
    public string NomeCompleto { get; set; } = null!;
}