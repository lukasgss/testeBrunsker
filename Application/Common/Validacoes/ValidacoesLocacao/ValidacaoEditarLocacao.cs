using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using FluentValidation;

namespace Application.Common.Validacoes.ValidacoesLocacao;

public class ValidacaoEditarLocacao : AbstractValidator<EditarLocacaoRequest>
{
    public ValidacaoEditarLocacao()
    {
        RuleFor(locacao => locacao.Id)
            .NotNull()
            .WithMessage("Campo de id da locação é obrigatório.");
        
        RuleFor(locacao => locacao.IdImovel)
            .NotNull()
            .WithMessage("Campo de id do imóvel é obrigatório.");

        RuleFor(locacao => locacao.IdLocatario)
            .NotNull()
            .WithMessage("Campo de id do locatário é obrigatório.");

        RuleFor(locacao => locacao.DataVencimento)
            .Must(dataVencimento => dataVencimento > DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Data de vencimento deve ser maior que a data atual.");

        RuleFor(locacao => locacao.ValorMensal)
            .NotNull()
            .WithMessage("Campo de valor mensal é obrigatório.")
            .GreaterThan(0)
            .WithMessage("Valor mensal deve ser maior que 0.");
    }
}