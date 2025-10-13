using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;

[ApiController]
[Route("api/[controller]")]
public class PreferenciasController : ControllerBase
{
    private readonly DatabaseContext _context;

    public PreferenciasController(DatabaseContext context)
    {
        _context = context;
    }

    // GET: api/Preferencias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PreferenciasUsuarioDomain>>> GetPreferencias()
    {
        return await _context.PreferenciasUsuarios.ToListAsync();
    }

    // GET: api/Preferencias/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PreferenciasUsuarioDomain>> GetPreferencia(string id)
    {
        var preferencia = await _context.PreferenciasUsuarios.FindAsync(id);

        if (preferencia == null)
        {
            return NotFound();
        }

        return preferencia;
    }

    // PUT: api/Preferencias/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPreferencia(string id, PreferenciasUsuarioDomain preferencia)
    {
        if (id != preferencia.Id)
        {
            return BadRequest();
        }

        _context.Entry(preferencia).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PreferenciaExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Preferencias
    [HttpPost]
    public async Task<ActionResult<PreferenciasUsuarioDomain>> PostPreferencia(PreferenciasUsuarioDomain preferencia)
    {
        preferencia.Id = Guid.NewGuid().ToString();
        preferencia.DataAtualizacao = DateTime.UtcNow;

        _context.PreferenciasUsuarios.Add(preferencia);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPreferencia", new { id = preferencia.Id }, preferencia);
    }

    // DELETE: api/Preferencias/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePreferencia(string id)
    {
        var preferencia = await _context.PreferenciasUsuarios.FindAsync(id);
        if (preferencia == null)
        {
            return NotFound();
        }

        _context.PreferenciasUsuarios.Remove(preferencia);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PreferenciaExists(string id)
    {
        return _context.PreferenciasUsuarios.Any(e => e.Id == id);
    }
}