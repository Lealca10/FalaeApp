using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<object>>> GetPreferencias()
    {
        var preferencias = await _context.PreferenciasUsuarios
            .Select(p => new
            {
                p.Id,
                p.UsuarioId,
                p.HorarioFavorito,
                p.TipoComidaFavorito,
                p.NivelEstresse,
                p.GostaViajar,
                p.PreferenciaLocal,
                p.PreferenciaAmbiente,
                p.ImportanciaEspiritualidade,
                p.PosicaoPolitica,
                p.Genero,
                p.PreferenciaMusical,
                p.MoodFilmesSeries,
                p.StatusRelacionamento,
                p.TemFilhos,
                p.PreferenciaAnimal,
                p.FraseDefinicao,
                p.GostosPessoaisJson,
                p.DataAtualizacao
            })
            .ToListAsync();

        return Ok(preferencias);
    }

    // GET: api/Preferencias/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetPreferencia(string id)
    {
        var preferencia = await _context.PreferenciasUsuarios
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.UsuarioId,
                p.HorarioFavorito,
                p.TipoComidaFavorito,
                p.NivelEstresse,
                p.GostaViajar,
                p.PreferenciaLocal,
                p.PreferenciaAmbiente,
                p.ImportanciaEspiritualidade,
                p.PosicaoPolitica,
                p.Genero,
                p.PreferenciaMusical,
                p.MoodFilmesSeries,
                p.StatusRelacionamento,
                p.TemFilhos,
                p.PreferenciaAnimal,
                p.FraseDefinicao,
                p.GostosPessoaisJson,
                p.DataAtualizacao
            })
            .FirstOrDefaultAsync();

        if (preferencia == null)
            return NotFound(new { message = "Preferências não encontradas" });

        return Ok(preferencia);
    }

    // GET: api/Preferencias/usuario/{usuarioId}
    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<object>> GetPreferenciasByUsuario(string usuarioId)
    {
        var preferencias = await _context.PreferenciasUsuarios
            .Where(p => p.UsuarioId == usuarioId)
            .Select(p => new
            {
                p.Id,
                p.UsuarioId,
                p.HorarioFavorito,
                p.TipoComidaFavorito,
                p.NivelEstresse,
                p.GostaViajar,
                p.PreferenciaLocal,
                p.PreferenciaAmbiente,
                p.ImportanciaEspiritualidade,
                p.PosicaoPolitica,
                p.Genero,
                p.PreferenciaMusical,
                p.MoodFilmesSeries,
                p.StatusRelacionamento,
                p.TemFilhos,
                p.PreferenciaAnimal,
                p.FraseDefinicao,
                p.GostosPessoaisJson,
                p.DataAtualizacao
            })
            .FirstOrDefaultAsync();

        if (preferencias == null)
            return NotFound(new { message = "Preferências não encontradas para este usuário" });

        return Ok(preferencias);
    }

    // POST: api/Preferencias
    [HttpPost]
    public async Task<ActionResult<object>> PostPreferencias([FromBody] PreferenciasInput input)
    {
        try
        {
            // Verificar se usuário existe
            var usuario = await _context.Usuarios.FindAsync(input.UsuarioId);
            if (usuario == null || !usuario.Ativo)
                return BadRequest(new { message = "Usuário não encontrado" });

            // Verificar se já existe preferência para este usuário
            if (await _context.PreferenciasUsuarios.AnyAsync(p => p.UsuarioId == input.UsuarioId))
                return BadRequest(new { message = "Este usuário já possui preferências cadastradas" });

            var preferencias = new PreferenciasUsuarioDomain
            {
                Id = Guid.NewGuid().ToString(),
                UsuarioId = input.UsuarioId,
                HorarioFavorito = input.HorarioFavorito,
                TipoComidaFavorito = input.TipoComidaFavorito,
                NivelEstresse = input.NivelEstresse,
                GostaViajar = input.GostaViajar,
                PreferenciaLocal = input.PreferenciaLocal,
                PreferenciaAmbiente = input.PreferenciaAmbiente,
                ImportanciaEspiritualidade = input.ImportanciaEspiritualidade,
                PosicaoPolitica = input.PosicaoPolitica,
                Genero = input.Genero,
                PreferenciaMusical = input.PreferenciaMusical,
                MoodFilmesSeries = input.MoodFilmesSeries,
                StatusRelacionamento = input.StatusRelacionamento,
                TemFilhos = input.TemFilhos,
                PreferenciaAnimal = input.PreferenciaAnimal,
                FraseDefinicao = input.FraseDefinicao,
                GostosPessoaisJson = input.GostosPessoaisJson,
                DataAtualizacao = DateTime.UtcNow
            };

            _context.PreferenciasUsuarios.Add(preferencias);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPreferencia), new { id = preferencias.Id }, new
            {
                preferencias.Id,
                preferencias.UsuarioId,
                message = "Preferências criadas com sucesso"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // PUT: api/Preferencias/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPreferencias(string id, [FromBody] PreferenciasInput input)
    {
        try
        {
            var preferencias = await _context.PreferenciasUsuarios.FindAsync(id);
            if (preferencias == null)
                return NotFound(new { message = "Preferências não encontradas" });

            preferencias.HorarioFavorito = input.HorarioFavorito;
            preferencias.TipoComidaFavorito = input.TipoComidaFavorito;
            preferencias.NivelEstresse = input.NivelEstresse;
            preferencias.GostaViajar = input.GostaViajar;
            preferencias.PreferenciaLocal = input.PreferenciaLocal;
            preferencias.PreferenciaAmbiente = input.PreferenciaAmbiente;
            preferencias.ImportanciaEspiritualidade = input.ImportanciaEspiritualidade;
            preferencias.PosicaoPolitica = input.PosicaoPolitica;
            preferencias.Genero = input.Genero;
            preferencias.PreferenciaMusical = input.PreferenciaMusical;
            preferencias.MoodFilmesSeries = input.MoodFilmesSeries;
            preferencias.StatusRelacionamento = input.StatusRelacionamento;
            preferencias.TemFilhos = input.TemFilhos;
            preferencias.PreferenciaAnimal = input.PreferenciaAnimal;
            preferencias.FraseDefinicao = input.FraseDefinicao;
            preferencias.GostosPessoaisJson = input.GostosPessoaisJson;
            preferencias.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // DELETE: api/Preferencias/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePreferencias(string id)
    {
        try
        {
            var preferencias = await _context.PreferenciasUsuarios.FindAsync(id);
            if (preferencias == null)
                return NotFound(new { message = "Preferências não encontradas" });

            _context.PreferenciasUsuarios.Remove(preferencias);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // DELETE: api/Preferencias/usuario/{usuarioId} - Delete por usuário
    [HttpDelete("usuario/{usuarioId}")]
    public async Task<IActionResult> DeletePreferenciasByUsuario(string usuarioId)
    {
        try
        {
            var preferencias = await _context.PreferenciasUsuarios
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (preferencias == null)
                return NotFound(new { message = "Preferências não encontradas para este usuário" });

            _context.PreferenciasUsuarios.Remove(preferencias);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

// DTO de input para Preferências
public class PreferenciasInput
{
    public string UsuarioId { get; set; } = string.Empty;
    public string HorarioFavorito { get; set; } = string.Empty;
    public string TipoComidaFavorito { get; set; } = string.Empty;
    public int NivelEstresse { get; set; }
    public bool GostaViajar { get; set; }
    public string PreferenciaLocal { get; set; } = string.Empty;
    public string PreferenciaAmbiente { get; set; } = string.Empty;
    public int ImportanciaEspiritualidade { get; set; }
    public string PosicaoPolitica { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public string PreferenciaMusical { get; set; } = string.Empty;
    public string MoodFilmesSeries { get; set; } = string.Empty;
    public string StatusRelacionamento { get; set; } = string.Empty;
    public bool TemFilhos { get; set; }
    public string PreferenciaAnimal { get; set; } = string.Empty;
    public string FraseDefinicao { get; set; } = string.Empty;
    public string GostosPessoaisJson { get; set; } = string.Empty;
}