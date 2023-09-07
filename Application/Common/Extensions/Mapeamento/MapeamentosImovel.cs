using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Domain.Entidades;

namespace Application.Common.Extensions.Mapeamento;

public static class MapeamentosImovel
{
    public static RespostaImovel ToRespostaImovel(this Imovel imovel)
    {
        return new RespostaImovel()
        {
            Id = imovel.Id,
            Cep = imovel.Cep,
            Endereco = imovel.Endereco,
            Cidade = imovel.Cidade,
            Estado = imovel.Estado,
            Bairro = imovel.Bairro,
            Numero = imovel.Numero,
            Complemento = imovel.Complemento,
            Dono = imovel.Dono.ToRespostaDadosUsuario()
        };
    }
}