using Application.Interfaces;
using Application.Request;
using Application.UsesCases;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly IAdicionarFaturaUseCase _faturausecase;
        private readonly IValidator<FaturaRequest> ValidarRequisicaoFatura;

        public CartaoController(IAdicionarFaturaUseCase faturausecase, IValidator<FaturaRequest> validarRequisicaoFatura)
        {
            _faturausecase = faturausecase;
            ValidarRequisicaoFatura = validarRequisicaoFatura;
        }

        [HttpPost]
        public IActionResult AdicionarFatura(FaturaRequest faturarequest)
        {
            var validation = ValidarRequisicaoFatura.Validate(faturarequest);
            if (!validation.IsValid) {
            return BadRequest(validation.Errors);
            }

            _faturausecase.AdicionarFatura(faturarequest);

            return Created("url", null);
        }

    }
}
