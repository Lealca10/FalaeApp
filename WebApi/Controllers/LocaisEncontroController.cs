using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;

[ApiController]
[Route("api/[controller]")]
public class LocaisEncontroController : ControllerBase
{
    private readonly DatabaseContext _context;

    public LocaisEncontroController(DatabaseContext context)
    {
        _context = context;
    }

    // GET: api/LocaisEncontro
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetLocaisEncontro()
    {
        try
        {
            var locais = await _context.LocaisEncontro
                .Select(l => new
                {
                    l.Id,
                    l.Nome,
                    l.Endereco,
                    l.Capacidade,
                    l.Ativo,
                    l.DataCriacao
                })
                .ToListAsync();

            return Ok(locais);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar locais de encontro", details = ex.Message });
        }
    }

    // GET: api/LocaisEncontro/ativos
    [HttpGet("ativos")]
    public async Task<ActionResult<IEnumerable<object>>> GetLocaisAtivos()
    {
        try
        {
            var locais = await _context.LocaisEncontro
                .Where(l => l.Ativo)
                .Select(l => new
                {
                    l.Id,
                    l.Nome,
                    l.Endereco,
                    l.Capacidade,
                    l.Ativo,
                    l.DataCriacao
                })
                .ToListAsync();

            return Ok(locais);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar locais ativos", details = ex.Message });
        }
    }

    // GET: api/LocaisEncontro/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetLocalEncontro(string id)
    {
        try
        {
            var local = await _context.LocaisEncontro
                .Where(l => l.Id == id)
                .Select(l => new
                {
                    l.Id,
                    l.Nome,
                    l.Endereco,
                    l.Capacidade,
                    l.Ativo,
                    l.DataCriacao
                })
                .FirstOrDefaultAsync();

            if (local == null)
                return NotFound(new { message = "Local de encontro não encontrado" });

            return Ok(local);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar local de encontro", details = ex.Message });
        }
    }

    // POST: api/LocaisEncontro
    [HttpPost]
    public async Task<ActionResult<object>> PostLocalEncontro([FromBody] LocalEncontroInput input)
    {
        try
        {
            // Validar se já existe local com mesmo nome
            var localExistente = await _context.LocaisEncontro
                .AnyAsync(l => l.Nome.ToLower() == input.Nome.ToLower());

            if (localExistente)
                return BadRequest(new { message = "Já existe um local com este nome" });

            var local = new LocalEncontroDomain
            {
                Id = Guid.NewGuid().ToString(),
                Nome = input.Nome,
                Endereco = input.Endereco,
                Capacidade = input.Capacidade,
                Ativo = input.Ativo,
                DataCriacao = DateTime.UtcNow
            };

            _context.LocaisEncontro.Add(local);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocalEncontro), new { id = local.Id }, new
            {
                local.Id,
                local.Nome,
                local.Endereco,
                local.Capacidade,
                local.Ativo,
                local.DataCriacao,
                message = "Local de encontro criado com sucesso"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao criar local de encontro", details = ex.Message });
        }
    }

    // PUT: api/LocaisEncontro/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutLocalEncontro(string id, [FromBody] LocalEncontroInput input)
    {
        try
        {
            var local = await _context.LocaisEncontro.FindAsync(id);
            if (local == null)
                return NotFound(new { message = "Local de encontro não encontrado" });

            // Validar se outro local já tem este nome (excluindo o atual)
            var nomeEmUso = await _context.LocaisEncontro
                .AnyAsync(l => l.Nome.ToLower() == input.Nome.ToLower() && l.Id != id);

            if (nomeEmUso)
                return BadRequest(new { message = "Já existe outro local com este nome" });

            local.Nome = input.Nome;
            local.Endereco = input.Endereco;
            local.Capacidade = input.Capacidade;
            local.Ativo = input.Ativo;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao atualizar local de encontro", details = ex.Message });
        }
    }

    // DELETE: api/LocaisEncontro/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocalEncontro(string id)
    {
        try
        {
            var local = await _context.LocaisEncontro.FindAsync(id);
            if (local == null)
                return NotFound(new { message = "Local de encontro não encontrado" });

            // Verificar se existem encontros associados a este local
            var encontrosAssociados = await _context.Encontros
                .AnyAsync(e => e.LocalId == id);

            if (encontrosAssociados)
                return BadRequest(new { message = "Não é possível excluir este local pois existem encontros associados a ele" });

            _context.LocaisEncontro.Remove(local);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao excluir local de encontro", details = ex.Message });
        }
    }

    // PATCH: api/LocaisEncontro/{id}/ativar
    [HttpPatch("{id}/ativar")]
    public async Task<IActionResult> AtivarLocalEncontro(string id)
    {
        try
        {
            var local = await _context.LocaisEncontro.FindAsync(id);
            if (local == null)
                return NotFound(new { message = "Local de encontro não encontrado" });

            local.Ativo = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao ativar local de encontro", details = ex.Message });
        }
    }

    // PATCH: api/LocaisEncontro/{id}/desativar
    [HttpPatch("{id}/desativar")]
    public async Task<IActionResult> DesativarLocalEncontro(string id)
    {
        try
        {
            var local = await _context.LocaisEncontro.FindAsync(id);
            if (local == null)
                return NotFound(new { message = "Local de encontro não encontrado" });

            local.Ativo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao desativar local de encontro", details = ex.Message });
        }
    }

    // GET: api/LocaisEncontro/{id}/encontros
    [HttpGet("{id}/encontros")]
    public async Task<ActionResult<IEnumerable<object>>> GetEncontrosPorLocal(string id)
    {
        try
        {
            var local = await _context.LocaisEncontro.FindAsync(id);
            if (local == null)
                return NotFound(new { message = "Local de encontro não encontrado" });

            var encontros = await _context.Encontros
                .Where(e => e.LocalId == id)
                .Select(e => new
                {
                    e.Id,
                    e.DataHora,
                    e.Status,
                    e.DataCriacao,
                    TotalParticipantes = e.Participantes.Count
                })
                .ToListAsync();

            return Ok(encontros);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar encontros do local", details = ex.Message });
        }
    }
}

// DTO de input para LocalEncontro
public class LocalEncontroInput
{
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public int Capacidade { get; set; }
    public bool Ativo { get; set; } = true;
}