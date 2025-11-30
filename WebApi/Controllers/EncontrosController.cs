using Domain.Entities;
using Infrastructure.BaseDados;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Application.Request;
using Application.Request;
using Application.response;
using Infrastructure.Data;
using Application.Interfaces;
using Application.Request;
using WebApi.Models.Request;

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
                        Capacidade = e.Local.Capacidade,
                        ImagemUrl = e.Local.ImagemUrl
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
                NumeroParticipantes = 3
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


    // PUT: api/Encontros/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(string id, [FromBody] AtualizarStatusRequest request)
    {
        try
        {
            var encontro = await _context.Encontros.FindAsync(id);
            if (encontro == null)
                return NotFound(new { message = "Encontro não encontrado" });

            if (!new[] { "agendado", "realizado", "cancelado" }.Contains(request.Status.ToLower()))
                return BadRequest(new { message = "Status inválido. Use: agendado, realizado ou cancelado" });

            encontro.Status = request.Status.ToLower();
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao atualizar status", details = ex.Message });
        }
    }

    // PUT: api/Encontros/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarEncontro(string id, [FromBody] EncontroRequest request)
    {
        try
        {
            var encontro = await _context.Encontros
                .Include(e => e.Participantes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (encontro == null)
                return NotFound(new { message = "Encontro não encontrado" });

            // Verificar se local existe
            var local = await _context.LocaisEncontro.FindAsync(request.LocalId);
            if (local == null || !local.Ativo)
                return BadRequest(new { message = "Local não encontrado ou inativo" });

            encontro.LocalId = request.LocalId;
            encontro.DataHora = request.DataHora;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao atualizar encontro", details = ex.Message });
        }
    }

    // DELETE: api/Encontros/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarEncontro(string id)
    {
        try
        {
            var encontro = await _context.Encontros
                .Include(e => e.Feedbacks)
                .Include(e => e.Participantes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (encontro == null)
                return NotFound(new { message = "Encontro não encontrado" });

            // Verificar se existem feedbacks
            if (encontro.Feedbacks.Any())
                return BadRequest(new { message = "Não é possível excluir encontro com feedbacks associados" });

            // CORREÇÃO: Remover relações many-to-many primeiro
            encontro.Participantes.Clear();

            _context.Encontros.Remove(encontro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao excluir encontro", details = ex.Message });
        }
    }

    // GET: api/Encontros/status/{status}
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<EncontroResponse>>> GetEncontrosPorStatus(string status)
    {
        try
        {
            var encontros = await _context.Encontros
                .Include(e => e.Local)
                .Include(e => e.Participantes)
                .Where(e => e.Status.ToLower() == status.ToLower())
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
            return StatusCode(500, new { error = "Erro ao buscar encontros por status", details = ex.Message });
        }
    }

    // POST: api/Encontros/{id}/participantes/{usuarioId}
    [HttpPost("{id}/participantes/{usuarioId}")]
    public async Task<IActionResult> AdicionarParticipante(string id, string usuarioId)
    {
        try
        {
            var encontro = await _context.Encontros
                .Include(e => e.Participantes)
                .Include(e => e.Local)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (encontro == null)
                return NotFound(new { message = "Encontro não encontrado" });

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null || !usuario.Ativo)
                return NotFound(new { message = "Usuário não encontrado ou inativo" });

            // Verificar se usuário já é participante
            if (encontro.Participantes.Any(p => p.Id == usuarioId))
                return BadRequest(new { message = "Usuário já é participante deste encontro" });

            // Verificar capacidade
            if (encontro.Participantes.Count >= 5)
                return BadRequest(new { message = "Encontro já atingiu o número máximo de participantes (5)" });

            // Verificar capacidade do local
            if (encontro.Participantes.Count >= encontro.Local.Capacidade)
                return BadRequest(new { message = "Local já atingiu sua capacidade máxima" });

            encontro.Participantes.Add(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao adicionar participante", details = ex.Message });
        }
    }

    // DELETE: api/Encontros/{id}/participantes/{usuarioId}
    [HttpDelete("{id}/participantes/{usuarioId}")]
    public async Task<IActionResult> RemoverParticipante(string id, string usuarioId)
    {
        try
        {
            var encontro = await _context.Encontros
                .Include(e => e.Participantes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (encontro == null)
                return NotFound(new { message = "Encontro não encontrado" });

            var usuario = encontro.Participantes.FirstOrDefault(p => p.Id == usuarioId);
            if (usuario == null)
                return NotFound(new { message = "Usuário não é participante deste encontro" });

            encontro.Participantes.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao remover participante", details = ex.Message });
        }
    }
}

// Model para atualização de status
public class AtualizarStatusRequest
{
    public string Status { get; set; } = string.Empty;
}