using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly DatabaseContext _context;

    public TestController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpPost("create-tables")]
    public async Task<IActionResult> CreateTables()
    {
        try
        {
            // Força a criação das tabelas
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            return Ok(new
            {
                message = "Tabelas criadas com sucesso!",
                success = true
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = ex.Message,
                details = ex.InnerException?.Message
            });
        }
    }
}