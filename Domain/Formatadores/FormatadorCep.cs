namespace Domain.Formatadores;

public static class FormatadorCep
{
    public static string Formatar(string cep) => cep.Replace("-", "");
}