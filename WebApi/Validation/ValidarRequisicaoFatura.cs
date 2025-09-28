using Application.Request;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace WebApi.Validation
{
    public class ValidarRequisicaoFatura : AbstractValidator<FaturaRequest>
    {
        public ValidarRequisicaoFatura()
        {
            RuleFor(c => c.descricao).NotEmpty().WithMessage("Descrição é obrigatória."); ;
            RuleFor(c => c.data).NotEmpty().WithMessage("Data é inválida."); ;
            RuleFor(c => c.valor).NotEmpty().WithMessage("Valor é obrigatória."); ;
            RuleFor(c => c.categoria).NotEmpty().WithMessage("Categoria é obrigatória."); ;
        }
    }
}
