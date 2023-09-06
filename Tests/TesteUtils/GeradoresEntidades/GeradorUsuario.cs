using Application.Common.Interfaces.Entidades.Usuarios.DTOs;
using Domain.Entidades;
using Tests.TesteUtils.Constantes;

namespace Tests.TesteUtils.GeradoresEntidades;

public static class GeradorUsuario
{
    public static Usuario GerarUsuario()
    {
        return new Usuario()
        {
            Id = Constants.DadosUsuario.Id,
            NomeCompleto = Constants.DadosUsuario.NomeCompleto,
            Email = Constants.DadosUsuario.Email,
            PhoneNumber = Constants.DadosUsuario.Telefone,
            UserName = Constants.DadosUsuario.UserName,
            EmailConfirmed = Constants.DadosUsuario.EmailConfirmed,
        };
    }

    public static RespostaDadosUsuario GerarRespostaDadosUsuario()
    {
        return new RespostaDadosUsuario()
        {
            Id = Constants.DadosUsuario.Id,
            NomeCompleto = Constants.DadosUsuario.NomeCompleto,
            Email = Constants.DadosUsuario.Email,
            Telefone = Constants.DadosUsuario.Telefone
        };
    }

    public static RespostaUsuario GerarRespostaUsuario()
    {
        return new RespostaUsuario()
        {
            Id = Constants.DadosUsuario.Id,
            NomeCompleto = Constants.DadosUsuario.NomeCompleto,
            Email = Constants.DadosUsuario.Email,
            Telefone = Constants.DadosUsuario.Telefone,
            Token = Constants.DadosUsuario.JwtToken
        };
    }

    public static CriarUsuarioRequest GerarCriarUsuarioRequest()
    {
        return new CriarUsuarioRequest()
        {
            NomeCompleto = Constants.DadosUsuario.NomeCompleto,
            Email = Constants.DadosUsuario.Email,
            Telefone = Constants.DadosUsuario.Telefone,
            Senha = Constants.DadosUsuario.Senha,
            ConfirmarSenha = Constants.DadosUsuario.ConfirmarSenha
        };
    }

    public static LoginRequest GerarLoginRequest()
    {
        return new LoginRequest()
        {
            Email = Constants.DadosUsuario.Email,
            Senha = Constants.DadosUsuario.Senha
        };
    }
}