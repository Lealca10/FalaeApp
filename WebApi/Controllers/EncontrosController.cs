using Domain.Entities;
using Infrastructure.BaseDados;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Services;
using WebApi.Models.Request;
using Application.response;
using Infrastructure.Data;

[ApiController]
[Route("api/[controller]")]
public class EncontrosController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IEncontroMatchingService _matchingService;

    public EncontrosController(DatabaseContext context, IEncontroMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    // GET: api/Encontros
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EncontroResponse>>> GetEncontros()
    {
        try
        {
            var encontros = await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .Select(e => new EncontroResponse
                {
                    Id = e.Id,
                    LocalId = e.LocalId,
                    Local = new LocalEncontroInfo 
                    {
                        Id = e.Local.Id,
                        Nome = e.Local.Nome,
                        Endereco = e.Local.Endereco,
                        Capacidade = e.Local.Capacidade
                    },
                    DataHora = e.DataHora,
                    Status = e.Status,
                    DataCriacao = e.DataCriacao,
                    Participantes = e.Participantes.Select(p => new UsuarioInfo 
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        Email = p.Email,
                        Cidade = p.Cidade
                    }).ToList(),
                    TotalParticipantes = e.Participantes.Count
                })
                .ToListAsync();

            return Ok(encontros);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar encontros", details = ex.Message });
        }
    }

    // GET: api/Encontros/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<EncontroResponse>> GetEncontro(string id)
    {
        try
        {
            var encontro = await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .Where(e => e.Id == id)
                .Select(e => new EncontroResponse
                {
                    Id = e.Id,
                    LocalId = e.LocalId,
                    Local = new LocalEncontroInfo
                    {
                        Id = e.Local.Id,
                        Nome = e.Local.Nome,
                        Endereco = e.Local.Endereco,
                        Capacidade = e.Local.Capacidade
                    },
                    DataHora = e.DataHora,
                    Status = e.Status,
                    DataCriacao = e.DataCriacao,
                    Participantes = e.Participantes.Select(p => new UsuarioInfo
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        Email = p.Email,
                        Cidade = p.Cidade
                    }).ToList(),
                    TotalParticipantes = e.Participantes.Count
                })
                .FirstOrDefaultAsync();

            if (encontro == null)
                return NotFound(new { message = "Encontro não encontrado" });

            return Ok(encontro);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar encontro", details = ex.Message });
        }
    }

    // POST: api/Encontros/matching
    [HttpPost("matching")]
    public async Task<ActionResult<MatchingResult>> ProporEncontro([FromBody] MatchingRequest request)
    {
        try
        {
            // Verificar se local existe e está ativo
            var local = await _context.LocaisEncontro
                .FirstOrDefaultAsync(l => l.Id == request.LocalId && l.Ativo);

            if (local == null)
                return BadRequest(new { message = "Local não encontrado ou inativo" });

            // Verificar capacidade do local
            if (local.Capacidade < request.NumeroParticipantes)
                return BadRequest(new { message = "Local não tem capacidade para este número de participantes" });

            var resultado = await _matchingService.EncontrarParticipantesCompatíveis(request);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao propor encontro", details = ex.Message });
        }
    }

    // POST: api/Encontros
    [HttpPost]
    public async Task<ActionResult<EncontroResponse>> CriarEncontro([FromBody] EncontroRequest request)
    {
        try
        {
            // Verificar se local existe
            var local = await _context.LocaisEncontro.FindAsync(request.LocalId);
            if (local == null || !local.Ativo)
                return BadRequest(new { message = "Local não encontrado ou inativo" });

            // Buscar participantes compatíveis
            var matchingRequest = new MatchingRequest
            {
                LocalId = request.LocalId,
                DataHora = request.DataHora,
                MinimoPreferenciasIguais = request.MinimoPreferenciasIguais,
                NumeroParticipantes = 5
            };

            var matchingResult = await _matchingService.EncontrarParticipantesCompatíveis(matchingRequest);

            if (!matchingResult.Sucesso)
                return BadRequest(new { message = matchingResult.Mensagem });

            // Criar encontro
            var encontro = new EncontroDomain
            {
                Id = Guid.NewGuid().ToString(),
                LocalId = request.LocalId,
                DataHora = request.DataHora,
                Status = "agendado",
                DataCriacao = DateTime.UtcNow
            };

            // Adicionar participantes
            foreach (var participanteInfo in matchingResult.ParticipantesSugeridos)
            {
                var usuario = await _context.Usuarios.FindAsync(participanteInfo.Id);
                if (usuario != null)
                {
                    encontro.Participantes.Add(usuario);
                }
            }

            _context.Encontros.Add(encontro);
            await _context.SaveChangesAsync();

            // Buscar o encontro com os dados completos após salvar
            var encontroCompleto = await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .FirstOrDefaultAsync(e => e.Id == encontro.Id);

            if (encontroCompleto == null)
                return BadRequest(new { message = "Erro ao recuperar encontro criado" });

            // Retornar resposta com dados completos
            var response = new EncontroResponse
            {
                Id = encontroCompleto.Id,
                LocalId = encontroCompleto.LocalId,
                Local = new LocalEncontroInfo
                {
                    Id = encontroCompleto.Local.Id,
                    Nome = encontroCompleto.Local.Nome,
                    Endereco = encontroCompleto.Local.Endereco,
                    Capacidade = encontroCompleto.Local.Capacidade
                },
                DataHora = encontroCompleto.DataHora,
                Status = encontroCompleto.Status,
                DataCriacao = encontroCompleto.DataCriacao,
                Participantes = encontroCompleto.Participantes.Select(p => new UsuarioInfo
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Email = p.Email,
                    Cidade = p.Cidade
                }).ToList(),
                TotalParticipantes = encontroCompleto.Participantes.Count
            };

            return CreatedAtAction(nameof(GetEncontro), new { id = encontro.Id }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao criar encontro", details = ex.Message });
        }
    }

    // ... (mantenha os outros métodos com a mesma lógica de correção)
}