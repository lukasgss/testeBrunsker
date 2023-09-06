using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using FluentValidation;

namespace Application.Common.Validacoes.ValidacoesImovel;

public class ValidacoesCadastroImovel : AbstractValidator<CriarImovelRequest>
{
    public ValidacoesCadastroImovel()
    {
        RuleFor(imovel => imovel.Cep)
            .NotEmpty()
            .WithMessage("Campo de CEP é obrigatório.")
            .MaximumLength(15)
            .WithMessage("Máximo de 15 caracteres permitidos.");

        RuleFor(imovel => imovel.Numero)
            .NotNull()
            .WithMessage("Campo de número é obrigatório.")
            .GreaterThan(0)
            .WithMessage("Campo de número deve ser maior que 0.");
    }
}