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

    /// <summary>
    /// Consulta el listado general con filtros opcionales (búsqueda, estado y tipo de documento).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos([FromQuery] string? q, [FromQuery] string? estado, [FromQuery] string? tipoDoc)
    {
        var clientes = await _clienteService.ObtenerTodosAsync(q, estado, tipoDoc);
        return Ok(clientes);
    }

    /// <summary>
    /// Consulta el detalle de un cliente puntual por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Registra un nuevo Cliente Residencial aplicando todas las validaciones de negocio.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
            // Errores de negocio (Canal Moderno o Documento duplicado)
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno procesando la solicitud.", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Modifica los campos permitidos de un Cliente Residencial activo.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            // Bloqueado para modificación
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Ejecuta el Retiro (Baja Lógica) de un Cliente Residencial.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Endpoint alternativo explícito para retiro lógico.
    /// </summary>
    [HttpPost("{id:int}/retirar")]
    public async Task<IActionResult> RetirarPost(int id)
    {
        return await Retirar(id);
    }
}
