using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;
using System.Text.RegularExpressions;

namespace backend.Services;

public class ClienteService
{
    private readonly AppDbContext _context;

    public ClienteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClienteListDto>> ObtenerTodosAsync(string? q, string? estado, string? tipoDoc)
    {
        var query = _context.Clientes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.Trim().ToLower();
            query = query.Where(c => 
                c.NombreCompleto.ToLower().Contains(search) ||
                c.NombreNegocio.ToLower().Contains(search) ||
                c.NumeroDocumento.Contains(search) ||
                c.Codigo.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(estado) && estado.ToLower() != "todos")
        {
            if (estado.ToLower() == "activos") query = query.Where(c => !c.Bloqueado);
            if (estado.ToLower() == "bloqueados") query = query.Where(c => c.Bloqueado);
        }

        if (!string.IsNullOrWhiteSpace(tipoDoc) && tipoDoc.ToLower() != "todos")
        {
            query = query.Where(c => c.TipoDocumento.ToLower() == tipoDoc.ToLower());
        }

        return await query
            .OrderByDescending(c => c.Id)
            .Select(c => new ClienteListDto
            {
                Id = c.Id,
                Codigo = c.Codigo,
                NombreCompleto = c.NombreCompleto,
                NombreNegocio = c.NombreNegocio,
                TipoDocumento = c.TipoDocumento,
                NumeroDocumento = c.DigitoVerificacion.HasValue 
                    ? $"{c.NumeroDocumento}-{c.DigitoVerificacion}" 
                    : c.NumeroDocumento,
                Telefono = c.Celular ?? c.Telefono,
                Email = c.Email,
                Municipio = c.Municipio,
                Barrio = c.Barrio,
                Estrato = c.Estrato,
                Bloqueado = c.Bloqueado
            })
            .ToListAsync();
    }

    public async Task<ClienteResponseDto> ObtenerPorIdAsync(int id)
    {
        var c = await _context.Clientes.FindAsync(id);
        if (c == null)
        {
            throw new KeyNotFoundException("El cliente a consultar no existe.");
        }

        return new ClienteResponseDto
        {
            Id = c.Id,
            Codigo = c.Codigo,
            Tratamiento = c.Tratamiento,
            NombreNegocio = c.NombreNegocio,
            RazonSocialExtendida = c.RazonSocialExtendida,
            NombreCompleto = c.NombreCompleto,
            Nombres = c.Nombres,
            Apellidos = c.Apellidos,
            TipoDocumento = c.TipoDocumento,
            NumeroDocumento = c.NumeroDocumento,
            DigitoVerificacion = c.DigitoVerificacion,
            Telefono = c.Telefono,
            Celular = c.Celular,
            Email = c.Email,
            EsRural = c.EsRural,
            DireccionRural = c.DireccionRural,
            DireccionEstandarizada = c.DireccionEstandarizada,
            Barrio = c.Barrio,
            Municipio = c.Municipio,
            Departamento = c.Departamento,
            Pais = c.Pais,
            ZonaTransporte = c.ZonaTransporte,
            Centro = c.Centro,
            Estrato = c.Estrato,
            ClaseImpuesto = c.ClaseImpuesto,
            CondicionPago = c.CondicionPago,
            Bloqueado = c.Bloqueado,
            FechaRetiro = c.FechaRetiro,
            Advertencia = c.Bloqueado ? "Cliente bloqueado" : null
        };
    }

    public async Task<ClienteResponseDto> CrearAsync(ClienteCreateDto dto)
    {
        // 1. Validar campos obligatorios básicos
        if (string.IsNullOrWhiteSpace(dto.NombreNegocio))
            throw new ArgumentException("El nombre de negocio es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.RazonSocialExtendida))
            throw new ArgumentException("La razón social extendida es obligatoria.");
        if (string.IsNullOrWhiteSpace(dto.NumeroDocumento))
            throw new ArgumentException("El número de documento es obligatorio.");

        // 2. Validación de Canal Moderno (Regla 3.4)
        var docLimpio = Regex.Replace(dto.NumeroDocumento, @"\D", "");
        var existeEnCanalModerno = await _context.ClientesCanalModerno
            .AnyAsync(cm => cm.NumeroDocumento == docLimpio);
        if (existeEnCanalModerno)
        {
            throw new InvalidOperationException("El cliente debe crearse mediante el flujo 'Creación clientes canal moderno'.");
        }

        // 3. Validación de documento duplicado activo
        var duplicado = await _context.Clientes
            .AnyAsync(c => c.NumeroDocumento == docLimpio && !c.Bloqueado);
        if (duplicado)
        {
            throw new InvalidOperationException($"Ya existe un cliente activo registrado con el documento {docLimpio}.");
        }

        // 4. Validación de Tratamiento e Identificación Fiscal (NIT / D.V.)
        int? dvCalculado = null;
        if (dto.Tratamiento.Equals("Empresa", StringComparison.OrdinalIgnoreCase))
        {
            if (!dto.TipoDocumento.Equals("NIT", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Para empresas, el tipo de documento debe ser obligatoriamente NIT.");

            dvCalculado = CalcularDigitoVerificacion(docLimpio);
            if (dvCalculado == null)
                throw new ArgumentException("El formato del NIT es incorrecto para el cálculo de dígito de verificación.");
        }

        // 5. Validación de Comunicación (Regla 3.2)
        bool tieneTelefono = !string.IsNullOrWhiteSpace(dto.Telefono) || !string.IsNullOrWhiteSpace(dto.Celular);
        if (!tieneTelefono)
            throw new ArgumentException("Debe ingresar al menos un número telefónico (fijo o celular).");

        if (dto.TipoDocumento.Equals("NIT", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("El correo electrónico es obligatorio cuando el tipo de documento es NIT.");

        ValidarValoresDummy(dto.Telefono, dto.Celular, dto.Email);

        // 6. Validación de Dirección (Regla 3.3)
        string direccionFinal;
        if (dto.EsRural)
        {
            if (string.IsNullOrWhiteSpace(dto.DireccionRural))
                throw new ArgumentException("Debe ingresar la descripción de la dirección rural.");
            direccionFinal = dto.DireccionRural.Trim();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.ViaTipo) || string.IsNullOrWhiteSpace(dto.ViaNumero) || 
                string.IsNullOrWhiteSpace(dto.CruceNumero) || string.IsNullOrWhiteSpace(dto.PlacaNumero))
            {
                throw new ArgumentException("Debe diligenciar los componentes obligatorios de la vía principal y alimentadora.");
            }
            direccionFinal = $"{dto.ViaTipo} {dto.ViaNumero}{dto.ViaLetra} {dto.ViaCardinalidad} # {dto.CruceNumero}-{dto.PlacaNumero}".Replace("  ", " ").Trim();
        }

        // 7. Autocompletar datos del Barrio
        var barrioInfo = await _context.Barrios.FirstOrDefaultAsync(b => b.Nombre.ToLower() == dto.Barrio.ToLower());
        string municipio = barrioInfo?.Municipio ?? "Medellín";
        string departamento = barrioInfo?.Departamento ?? "Antioquia";
        string pais = barrioInfo?.Pais ?? "Colombia";
        string zonaTransporte = barrioInfo?.ZonaTransporte ?? "Zona Metropolitana";

        // 8. Validación de Estrato (Regla 3.5)
        if (dto.Estrato < 1 || dto.Estrato > 6)
            throw new ArgumentException("El estrato debe ser un valor entre 1 y 6.");

        // 9. Cálculo de Nombres y Campos Derivados (Reglas 3.1 y 3.6)
        string nombresCalculados;
        string apellidosCalculados;
        string nombreCompletoCalculado;

        if (dto.Tratamiento.Equals("Empresa", StringComparison.OrdinalIgnoreCase))
        {
            nombresCalculados = dto.RazonSocialExtendida.Trim();
            apellidosCalculados = dto.RazonSocialExtendida.Trim();
            nombreCompletoCalculado = dto.RazonSocialExtendida.Trim();
        }
        else
        {
            nombreCompletoCalculado = dto.RazonSocialExtendida.Trim();
            var partes = SepararNombresYApellidos(dto.RazonSocialExtendida);
            nombresCalculados = partes.nombres;
            apellidosCalculados = partes.apellidos;
        }

        // Generar código consecutivo único
        int proximoNumero = (await _context.Clientes.CountAsync()) + 1;
        string codigo = $"CLI-{proximoNumero:D5}";

        var nuevo = new ClienteResidencial
        {
            Codigo = codigo,
            Tratamiento = dto.Tratamiento,
            NombreNegocio = dto.NombreNegocio.Trim(),
            RazonSocialExtendida = dto.RazonSocialExtendida.Trim(),
            NombreCompleto = nombreCompletoCalculado,
            Nombres = nombresCalculados,
            Apellidos = apellidosCalculados,
            TipoDocumento = dto.TipoDocumento.ToUpper(),
            NumeroDocumento = docLimpio,
            DigitoVerificacion = dvCalculado,
            Telefono = dto.Telefono?.Trim(),
            Celular = dto.Celular?.Trim(),
            Email = dto.Email?.Trim(),
            EsRural = dto.EsRural,
            DireccionRural = dto.DireccionRural?.Trim(),
            ViaTipo = dto.ViaTipo,
            ViaNumero = dto.ViaNumero,
            ViaLetra = dto.ViaLetra,
            ViaCardinalidad = dto.ViaCardinalidad,
            CruceNumero = dto.CruceNumero,
            PlacaNumero = dto.PlacaNumero,
            DireccionEstandarizada = direccionFinal,
            Barrio = dto.Barrio,
            Municipio = municipio,
            Departamento = departamento,
            Pais = pais,
            ZonaTransporte = zonaTransporte,
            Centro = string.IsNullOrWhiteSpace(dto.Centro) ? "Sede Principal" : dto.Centro.Trim(),
            Estrato = dto.Estrato,
            ClaseImpuesto = dto.Tratamiento.Equals("Empresa", StringComparison.OrdinalIgnoreCase) ? "Persona Jurídica" : "Persona Natural",
            CondicionPago = "0010 Contado",
            Bloqueado = false,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Clientes.Add(nuevo);
        await _context.SaveChangesAsync();

        return await ObtenerPorIdAsync(nuevo.Id);
    }

    public async Task<ClienteResponseDto> ActualizarAsync(int id, ClienteUpdateDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            throw new KeyNotFoundException("El cliente a consultar no existe.");

        // Regla 5.2: Un cliente bloqueado no puede modificarse
        if (cliente.Bloqueado)
            throw new InvalidOperationException("No se puede modificar un cliente que se encuentra bloqueado o retirado.");

        if (string.IsNullOrWhiteSpace(dto.NombreNegocio))
            throw new ArgumentException("El nombre de negocio no puede estar vacío.");

        // Regla 5.1: Si cambian nombres/apellidos, se recalcula la Razón Social y Nombre Completo
        if (!string.IsNullOrWhiteSpace(dto.Nombres) && !string.IsNullOrWhiteSpace(dto.Apellidos))
        {
            cliente.Nombres = dto.Nombres.Trim();
            cliente.Apellidos = dto.Apellidos.Trim();
            cliente.RazonSocialExtendida = $"{dto.Nombres.Replace(",", "")} {dto.Apellidos.Replace(",", "")}".Trim();
            cliente.NombreCompleto = cliente.RazonSocialExtendida;
        }

        cliente.NombreNegocio = dto.NombreNegocio.Trim();

        // Validar comunicación
        bool tieneTelefono = !string.IsNullOrWhiteSpace(dto.Telefono) || !string.IsNullOrWhiteSpace(dto.Celular);
        if (!tieneTelefono)
            throw new ArgumentException("Debe existir al menos un teléfono (fijo o celular).");

        ValidarValoresDummy(dto.Telefono, dto.Celular, dto.Email);
        cliente.Telefono = dto.Telefono?.Trim();
        cliente.Celular = dto.Celular?.Trim();
        cliente.Email = dto.Email?.Trim();

        // Dirección
        if (dto.EsRural)
        {
            if (string.IsNullOrWhiteSpace(dto.DireccionRural))
                throw new ArgumentException("Debe ingresar la dirección rural.");
            cliente.EsRural = true;
            cliente.DireccionRural = dto.DireccionRural.Trim();
            cliente.DireccionEstandarizada = cliente.DireccionRural;
        }
        else
        {
            cliente.EsRural = false;
            cliente.ViaTipo = dto.ViaTipo;
            cliente.ViaNumero = dto.ViaNumero;
            cliente.ViaLetra = dto.ViaLetra;
            cliente.ViaCardinalidad = dto.ViaCardinalidad;
            cliente.CruceNumero = dto.CruceNumero;
            cliente.PlacaNumero = dto.PlacaNumero;
            cliente.DireccionEstandarizada = $"{dto.ViaTipo} {dto.ViaNumero}{dto.ViaLetra} {dto.ViaCardinalidad} # {dto.CruceNumero}-{dto.PlacaNumero}".Replace("  ", " ").Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.Barrio))
        {
            var barrioInfo = await _context.Barrios.FirstOrDefaultAsync(b => b.Nombre.ToLower() == dto.Barrio.ToLower());
            cliente.Barrio = dto.Barrio;
            if (barrioInfo != null)
            {
                cliente.Municipio = barrioInfo.Municipio;
                cliente.Departamento = barrioInfo.Departamento;
                cliente.ZonaTransporte = barrioInfo.ZonaTransporte;
            }
        }

        if (dto.Estrato >= 1 && dto.Estrato <= 6)
            cliente.Estrato = dto.Estrato;

        if (!string.IsNullOrWhiteSpace(dto.Centro))
            cliente.Centro = dto.Centro.Trim();

        cliente.FechaModificacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await ObtenerPorIdAsync(cliente.Id);
    }

    public async Task RetirarAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            throw new KeyNotFoundException("El cliente a consultar no existe.");

        // Regla 6: No se puede retirar a alguien ya retirado
        if (cliente.Bloqueado)
            throw new InvalidOperationException("El cliente ya se encuentra en estado retirado/bloqueado.");

        // Baja lógica
        cliente.Bloqueado = true;
        cliente.FechaRetiro = DateTime.UtcNow;
        cliente.FechaModificacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    // Algoritmo oficial DIAN Módulo 11 para Dígito de Verificación
    public static int? CalcularDigitoVerificacion(string nitLimpio)
    {
        if (string.IsNullOrWhiteSpace(nitLimpio) || nitLimpio.Length < 5) return null;

        int[] factores = { 71, 67, 59, 53, 47, 43, 41, 37, 29, 23, 19, 17, 13, 7, 3 };
        int suma = 0;
        int len = nitLimpio.Length;

        for (int i = 0; i < len; i++)
        {
            if (!char.IsDigit(nitLimpio[len - 1 - i])) return null;
            int digito = nitLimpio[len - 1 - i] - '0';
            suma += digito * factores[15 - 1 - i];
        }

        int residuo = suma % 11;
        return (residuo == 0 || residuo == 1) ? residuo : 11 - residuo;
    }

    private static (string nombres, string apellidos) SepararNombresYApellidos(string nombreCompleto)
    {
        var palabras = nombreCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (palabras.Length == 0) return ("", "");
        if (palabras.Length == 1) return (palabras[0], "");
        if (palabras.Length == 2) return (palabras[0], palabras[1]);
        if (palabras.Length == 3) return (palabras[0], $"{palabras[1]}, {palabras[2]}");

        // 4 o más palabras: primeras como nombres, últimas dos como apellidos
        var nombres = string.Join(", ", palabras.Take(palabras.Length - 2));
        var apellidos = string.Join(", ", palabras.Skip(palabras.Length - 2));
        return (nombres, apellidos);
    }

    private static void ValidarValoresDummy(string? tel, string? cel, string? email)
    {
        string[] dummyPhones = { "0000000", "1111111", "1234567", "9999999" };
        string[] dummyEmails = { "test@test.com", "fake@fake.com", "asd@asd.com", "a@a.com", "dummy@" };

        if (!string.IsNullOrWhiteSpace(tel) && dummyPhones.Any(d => tel.Contains(d)))
            throw new ArgumentException("El teléfono contiene un valor dummy no permitido.");

        if (!string.IsNullOrWhiteSpace(cel) && dummyPhones.Any(d => cel.Contains(d)))
            throw new ArgumentException("El celular contiene un valor dummy no permitido.");

        if (!string.IsNullOrWhiteSpace(email) && dummyEmails.Any(d => email.ToLower().Contains(d)))
            throw new ArgumentException("El correo electrónico contiene una expresión dummy no permitida.");
    }
}
