using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IPasswordService _passwordService;

    public UsuariosController(DatabaseContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    // GET: api/Usuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Where(u => u.Ativo)
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Cpf,
                u.DataNascimento,
                u.Cidade,
                u.Email,
                u.Ativo
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    // GET: api/Usuarios/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetUsuario(string id)
    {
        var usuario = await _context.Usuarios
            .Where(u => u.Id == id && u.Ativo)
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Cpf,
                u.DataNascimento,
                u.Cidade,
                u.Email,
                u.Ativo
            })
            .FirstOrDefaultAsync();

        if (usuario == null)
            return NotFound(new { message = "Usuário não encontrado" });

        return Ok(usuario);
    }

    // POST: api/Usuarios
    [HttpPost]
    public async Task<ActionResult<object>> PostUsuario([FromBody] UsuarioInput input)
    {
        try
        {
            // Verificar se email já existe
            if (await _context.Usuarios.AnyAsync(u => u.Email == input.Email))
                return BadRequest(new { message = "Email já cadastrado" });

            // Verificar se CPF já existe
            if (await _context.Usuarios.AnyAsync(u => u.Cpf == input.Cpf))
                return BadRequest(new { message = "CPF já cadastrado" });

            var usuario = new UsuarioDomain
            {
                Id = Guid.NewGuid().ToString(),
                Nome = input.Nome,
                Cpf = input.Cpf,
                DataNascimento = input.DataNascimento,
                Cidade = input.Cidade,
                Email = input.Email,
                Senha = _passwordService.HashPassword(input.Senha),
                Ativo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Cidade,
                message = "Usuário criado com sucesso"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // PUT: api/Usuarios/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(string id, [FromBody] UsuarioInput input)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null || !usuario.Ativo)
                return NotFound(new { message = "Usuário não encontrado" });

            // Verificar se email já existe (outro usuário)
            if (await _context.Usuarios.AnyAsync(u => u.Email == input.Email && u.Id != id))
                return BadRequest(new { message = "Email já está em uso" });

            // Verificar se CPF já existe (outro usuário)
            if (await _context.Usuarios.AnyAsync(u => u.Cpf == input.Cpf && u.Id != id))
                return BadRequest(new { message = "CPF já está em uso" });

            usuario.Nome = input.Nome;
            usuario.Cpf = input.Cpf;
            usuario.DataNascimento = input.DataNascimento;
            usuario.Cidade = input.Cidade;
            usuario.Email = input.Email;
            usuario.Senha = _passwordService.HashPassword(input.Senha);


            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // DELETE: api/Usuarios/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(string id)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null || !usuario.Ativo)
                return NotFound(new { message = "Usuário não encontrado" });

            usuario.Ativo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

}

// DTO de input para Usuário
public class UsuarioInput
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

