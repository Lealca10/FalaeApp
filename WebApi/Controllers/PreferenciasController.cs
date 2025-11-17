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
                IdiomaPreferido = p.IdiomaPreferido ?? string.Empty, // Trata NULL
                InvestimentoEncontro = p.InvestimentoEncontro ?? string.Empty, // Trata NULL
                GostosPessoaisJson = p.GostosPessoaisJson ?? string.Empty, // Trata NULL
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
                IdiomaPreferido = p.IdiomaPreferido ?? string.Empty, // Trata NULL
                InvestimentoEncontro = p.InvestimentoEncontro ?? string.Empty, // Trata NULL
                GostosPessoaisJson = p.GostosPessoaisJson ?? string.Empty, // Trata NULL
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
                IdiomaPreferido = p.IdiomaPreferido ?? string.Empty, // Trata NULL
                InvestimentoEncontro = p.InvestimentoEncontro ?? string.Empty, // Trata NULL
                GostosPessoaisJson = p.GostosPessoaisJson ?? string.Empty, // Trata NULL
                p.DataAtualizacao
            })
            .FirstOrDefaultAsync();

        if (preferencias == null)
            return NotFound(new { message = "Preferências não encontradas para este usuário" });

        return Ok(preferencias);
    }

    // POST: api/Preferencias (agora com comportamento UPSERT)
    [HttpPost]
    public async Task<ActionResult<object>> PostPreferencias([FromBody] PreferenciasInput input)
    {
        try
        {
            // Verificar se usuário existe
            var usuario = await _context.Usuarios.FindAsync(input.UsuarioId);
            if (usuario == null || !usuario.Ativo)
                return BadRequest(new { message = "Usuário não encontrado ou inativo" });

            // Buscar preferências existentes
            var existente = await _context.PreferenciasUsuarios
                .FirstOrDefaultAsync(p => p.UsuarioId == input.UsuarioId);

            if (existente == null)
            {
                // CREATE
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
                    IdiomaPreferido = input.IdiomaPreferido ?? string.Empty,
                    InvestimentoEncontro = input.InvestimentoEncontro ?? string.Empty,
                    GostosPessoaisJson = input.GostosPessoaisJson ?? string.Empty,
                    DataAtualizacao = DateTime.UtcNow
                };

                _context.PreferenciasUsuarios.Add(preferencias);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPreferencia), new { id = preferencias.Id }, new
                {
                    message = "Preferências criadas com sucesso",
                    data = new { preferencias.Id, preferencias.UsuarioId }
                });
            }
            else
            {
                // UPDATE
                existente.HorarioFavorito = input.HorarioFavorito;
                existente.TipoComidaFavorito = input.TipoComidaFavorito;
                existente.NivelEstresse = input.NivelEstresse;
                existente.GostaViajar = input.GostaViajar;
                existente.PreferenciaLocal = input.PreferenciaLocal;
                existente.PreferenciaAmbiente = input.PreferenciaAmbiente;
                existente.ImportanciaEspiritualidade = input.ImportanciaEspiritualidade;
                existente.PosicaoPolitica = input.PosicaoPolitica;
                existente.Genero = input.Genero;
                existente.PreferenciaMusical = input.PreferenciaMusical;
                existente.MoodFilmesSeries = input.MoodFilmesSeries;
                existente.StatusRelacionamento = input.StatusRelacionamento;
                existente.TemFilhos = input.TemFilhos;
                existente.PreferenciaAnimal = input.PreferenciaAnimal;
                existente.FraseDefinicao = input.FraseDefinicao;
                existente.IdiomaPreferido = input.IdiomaPreferido ?? string.Empty;
                existente.InvestimentoEncontro = input.InvestimentoEncontro ?? string.Empty;
                existente.GostosPessoaisJson = input.GostosPessoaisJson ?? string.Empty;
                existente.DataAtualizacao = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Preferências atualizadas com sucesso",
                    data = new { existente.Id, existente.UsuarioId }
                });
            }
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
    public string? IdiomaPreferido { get; set; } // Permite NULL
    public string? InvestimentoEncontro { get; set; } // Permite NULL
    public string? GostosPessoaisJson { get; set; } // Permite NULL
}