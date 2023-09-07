using Domain.Entidades;
using Tests.TesteUtils.GeradoresEntidades;

namespace Tests.TesteUtils.Constantes;

public static partial class Constants
{
    public static class DadosImovel
    {
        public const int Id = 1;
        public const string Endereco = Constants.DadosCep.Logradouro;
        public const string Cep = Constants.DadosCep.Cep;
        public const int Numero = 100;
        public const string Complemento = Constants.DadosCep.Complemento;
        public static readonly Usuario Dono = GeradorUsuario.GerarUsuario();
        public static readonly int DonoId = Dono.Id;
    }
}