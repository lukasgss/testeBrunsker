using Application.Common.Interfaces.Entidades.Usuarios.DTOs;
using Domain.Entidades;

namespace Application.Common.Extensions.Mapeamento;

public static class MapeamentosUsuario
{
    public static RespostaUsuario ToRespostaUsuario(this Usuario usuario, string token)
    {
        return new RespostaUsuario()
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Telefone = usuario.PhoneNumber,
            NomeCompleto = usuario.NomeCompleto,
            Token = token
        };
    }
    
    public static RespostaDadosUsuario ToRespostaDadosUsuario(this Usuario usuario)
    {
        return new RespostaDadosUsuario()
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Telefone = usuario.PhoneNumber,
            NomeCompleto = usuario.NomeCompleto
        };
    }
}