using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly DatabaseContext _context;

    public DatabaseController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet("tables")]
    public IActionResult GetTables()
    {
        try
        {
            var entityTypes = _context.Model.GetEntityTypes();
            var tables = entityTypes.Select(e => new
            {
                Entity = e.Name,
                TableName = e.GetTableName(),
                Properties = e.GetProperties().Select(p => p.Name).ToList()
            }).ToList();

            return Ok(tables);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}