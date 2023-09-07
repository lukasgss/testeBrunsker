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
        public const string Cidade = Constants.DadosCep.Localidade;
        public const string Bairro = Constants.DadosCep.Bairro;
        public const string Estado = Constants.DadosCep.Uf;
        public const int Numero = 100;
        public const string Complemento = Constants.DadosCep.Complemento;
        public static readonly Usuario Dono = GeradorUsuario.GerarUsuario();
        public static readonly int DonoId = Dono.Id;
    }
}