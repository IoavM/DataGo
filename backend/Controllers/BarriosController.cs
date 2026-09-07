using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarriosController : ControllerBase
{
    private readonly AppDbContext _context;

    public BarriosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerBarrios()
    {
        var barrios = await _context.Barrios.AsNoTracking().ToListAsync();
        return Ok(barrios);
    }
}
