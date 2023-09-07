using System;
using Domain.Entidades;
using Tests.TesteUtils.GeradoresEntidades;

namespace Tests.TesteUtils.Constantes;

public static partial class Constants
{
    public static class DadosLocacao
    {
        public const int Id = 1;
        public static readonly Imovel Imovel = GeradorImovel.GerarImovel();
        public static readonly int ImovelId = Imovel.Id;
        public static readonly Usuario Locador = GeradorUsuario.GerarUsuario();
        public static readonly int LocadorId = Locador.Id;
        public static readonly Usuario Locatario = GeradorUsuario.GerarUsuarioComId(2);
        public static readonly int LocatarioId = Locatario.Id;
        public const bool LocadorAssinou = false;
        public const bool LocatarioAssinou = false;
        public static readonly DateTime DataFechamento = new DateTime(2020, 1, 1);
        public static readonly DateOnly DataVencimento = new DateOnly(2022, 1, 1);
        public const decimal ValorMensal = 1000;
    }
}