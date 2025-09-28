using Application.UsesCases;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaturaController : ControllerBase
    {
        private readonly ObterFaturasUseCase _useCase;

        public FaturaController(ObterFaturasUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpGet]
        public ActionResult<List<FaturaDomain>> Get()
        {
            var faturas = _useCase.Executar();
            return Ok(faturas);
        }
    }
}
