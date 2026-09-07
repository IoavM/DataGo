using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Xunit;

namespace backend.Tests;

public class ReglasNegocioTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        // Semilla para pruebas
        context.ClientesCanalModerno.Add(new ClienteCanalModerno
        {
            Id = 1,
            TipoDocumento = "C.C.",
            NumeroDocumento = "12345678",
            NombreCadena = "Éxito Poblado"
        });

        context.Barrios.Add(new BarrioCatalogo
        {
            Id = 1,
            Nombre = "Laureles",
            Municipio = "Medellín",
            Departamento = "Antioquia",
            Pais = "Colombia",
            ZonaTransporte = "Zona Centro-Occidente"
        });

        context.SaveChanges();
        return context;
    }

    [Fact]
    public void Regla1_NITValido_CalculaDigitoVerificacionCorrecto()
    {
        // Bancolombia NIT 890.900.608 tiene DV = 9 oficial DIAN módulo 11
        int? dv1 = ClienteService.CalcularDigitoVerificacion("890900608");
        Assert.Equal(9, dv1);

        // NIT 901.442.809 tiene DV = 6 matemático
        int? dv2 = ClienteService.CalcularDigitoVerificacion("901442809");
        Assert.Equal(6, dv2);
    }

    [Fact]
    public async Task Regla2_ClienteCanalModerno_LanzaExcepcionNegocio()
    {
        var db = GetInMemoryDbContext();
        var service = new ClienteService(db);

        var dto = new ClienteCreateDto
        {
            NombreNegocio = "Prueba",
            RazonSocialExtendida = "Prueba S.A.",
            NumeroDocumento = "12345678", // Existe en canal moderno
            Telefono = "3109876543",
            Barrio = "Laureles",
            Estrato = 3
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearAsync(dto));
        Assert.Contains("Creación clientes canal moderno", ex.Message);
    }

    [Fact]
    public async Task Regla3_ClienteSinTelefono_LanzaExcepcion()
    {
        var db = GetInMemoryDbContext();
        var service = new ClienteService(db);

        var dto = new ClienteCreateDto
        {
            NombreNegocio = "Prueba",
            RazonSocialExtendida = "Prueba S.A.",
            NumeroDocumento = "987654321",
            Telefono = null,
            Celular = "", // Sin teléfono ni celular
            Barrio = "Laureles",
            Estrato = 3
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(dto));
        Assert.Contains("al menos un número telefónico", ex.Message);
    }

    [Fact]
    public async Task Regla4_EmpresaNIT_SinEmail_LanzaExcepcion()
    {
        var db = GetInMemoryDbContext();
        var service = new ClienteService(db);

        var dto = new ClienteCreateDto
        {
            Tratamiento = "Empresa",
            TipoDocumento = "NIT",
            NombreNegocio = "Empresa Test",
            RazonSocialExtendida = "Empresa Test S.A.S.",
            NumeroDocumento = "890900608",
            Telefono = "3109876543",
            Email = "", // Email obligatorio si es NIT
            EsRural = true,
            DireccionRural = "Vereda Las Palmas",
            Barrio = "Laureles",
            Estrato = 4
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(dto));
        Assert.Contains("correo electrónico es obligatorio cuando el tipo de documento es NIT", ex.Message);
    }

    [Fact]
    public async Task Regla5_EstratoInvalido_LanzaExcepcion()
    {
        var db = GetInMemoryDbContext();
        var service = new ClienteService(db);

        var dto = new ClienteCreateDto
        {
            NombreNegocio = "Prueba",
            RazonSocialExtendida = "Prueba Persona",
            NumeroDocumento = "77889900",
            Telefono = "3109876543",
            EsRural = true,
            DireccionRural = "Vereda El Salado",
            Barrio = "Laureles",
            Estrato = 8 // Invalido (debe ser 1 a 6)
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(dto));
        Assert.Contains("estrato debe ser un valor entre 1 y 6", ex.Message);
    }

    [Fact]
    public async Task Regla6_ModificarClienteBloqueado_LanzaExcepcion()
    {
        var db = GetInMemoryDbContext();
        var clienteBloqueado = new ClienteResidencial
        {
            Id = 10,
            Codigo = "CLI-00010",
            NombreNegocio = "Bloqueado Test",
            RazonSocialExtendida = "Bloqueado Test",
            NumeroDocumento = "555666",
            Bloqueado = true // Cliente ya bloqueado
        };
        db.Clientes.Add(clienteBloqueado);
        await db.SaveChangesAsync();

        var service = new ClienteService(db);
        var updateDto = new ClienteUpdateDto { NombreNegocio = "Nuevo Nombre", Telefono = "3109876543" };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ActualizarAsync(10, updateDto));
        Assert.Contains("No se puede modificar un cliente que se encuentra bloqueado", ex.Message);
    }

    [Fact]
    public async Task Regla7_RetiroLogico_AplicaBajaSinEliminarFisicamente()
    {
        var db = GetInMemoryDbContext();
        var cliente = new ClienteResidencial
        {
            Id = 20,
            Codigo = "CLI-00020",
            NombreNegocio = "Activo Test",
            RazonSocialExtendida = "Activo Test",
            NumeroDocumento = "777888",
            Bloqueado = false
        };
        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();

        var service = new ClienteService(db);
        await service.RetirarAsync(20);

        var resultado = await db.Clientes.FindAsync(20);
        Assert.NotNull(resultado); // Sigue existiendo en BD físicamente
        Assert.True(resultado.Bloqueado); // Está bloqueado
        Assert.NotNull(resultado.FechaRetiro); // Tiene fecha de retiro
    }

    [Fact]
    public async Task Regla8_SegundoRetiro_LanzaExcepcion()
    {
        var db = GetInMemoryDbContext();
        var cliente = new ClienteResidencial
        {
            Id = 30,
            Codigo = "CLI-00030",
            NombreNegocio = "Ya Retirado",
            RazonSocialExtendida = "Ya Retirado",
            NumeroDocumento = "999000",
            Bloqueado = true
        };
        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();

        var service = new ClienteService(db);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.RetirarAsync(30));
        Assert.Contains("ya se encuentra en estado retirado/bloqueado", ex.Message);
    }

    [Fact]
    public async Task Regla9_ConsultarClienteInexistente_LanzaKeyNotFoundException()
    {
        var db = GetInMemoryDbContext();
        var service = new ClienteService(db);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ObtenerPorIdAsync(9999));
        Assert.Equal("El cliente a consultar no existe.", ex.Message);
    }
}

