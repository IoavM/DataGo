using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClientesController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] string? q, [FromQuery] string? estado, [FromQuery] string? tipoDoc)
    {
        var clientes = await _clienteService.ObtenerTodosAsync(q, estado, tipoDoc);
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        try
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            return Ok(cliente);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ClienteCreateDto dto)
    {
        try
        {
            var cliente = await _clienteService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ClienteUpdateDto dto)
    {
        try
        {
            var cliente = await _clienteService.ActualizarAsync(id, dto);
            return Ok(cliente);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Retirar(int id)
    {
        try
        {
            await _clienteService.RetirarAsync(id);
            return Ok(new { mensaje = "Cliente retirado exitosamente (baja lógica aplicada)." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("{id:int}/retirar")]
    public async Task<IActionResult> RetirarPost(int id)
    {
        return await Retirar(id);
    }
}
