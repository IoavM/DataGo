using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ClienteResidencial> Clientes => Set<ClienteResidencial>();
    public DbSet<ClienteCanalModerno> ClientesCanalModerno => Set<ClienteCanalModerno>();
    public DbSet<BarrioCatalogo> Barrios => Set<BarrioCatalogo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Índices de búsqueda
        modelBuilder.Entity<ClienteResidencial>()
            .HasIndex(c => c.NumeroDocumento);

        modelBuilder.Entity<ClienteResidencial>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

        // Seed: Clientes Canal Moderno (Tabla auxiliar para validación de exclusión)
        modelBuilder.Entity<ClienteCanalModerno>().HasData(
            new ClienteCanalModerno { Id = 1, TipoDocumento = "C.C.", NumeroDocumento = "12345678", NombreCadena = "Éxito Poblado" },
            new ClienteCanalModerno { Id = 2, TipoDocumento = "NIT", NumeroDocumento = "900111222", NombreCadena = "Jumbo Las Vegas" },
            new ClienteCanalModerno { Id = 3, TipoDocumento = "NIT", NumeroDocumento = "800123456", NombreCadena = "Olímpica Calle 10" }
        );

        // Seed: Catálogo de Barrios para autocompletado
        modelBuilder.Entity<BarrioCatalogo>().HasData(
            new BarrioCatalogo { Id = 1, Nombre = "Laureles", Municipio = "Medellín", Departamento = "Antioquia", Pais = "Colombia", ZonaTransporte = "Zona Centro-Occidente" },
            new BarrioCatalogo { Id = 2, Nombre = "El Poblado", Municipio = "Medellín", Departamento = "Antioquia", Pais = "Colombia", ZonaTransporte = "Zona Sur-Oriente" },
            new BarrioCatalogo { Id = 3, Nombre = "La Floresta", Municipio = "Medellín", Departamento = "Antioquia", Pais = "Colombia", ZonaTransporte = "Zona Occidente" },
            new BarrioCatalogo { Id = 4, Nombre = "Zúñiga", Municipio = "Envigado", Departamento = "Antioquia", Pais = "Colombia", ZonaTransporte = "Zona Sur Aburrá" },
            new BarrioCatalogo { Id = 5, Nombre = "Chapinero", Municipio = "Bogotá D.C.", Departamento = "Cundinamarca", Pais = "Colombia", ZonaTransporte = "Zona Centro Capital" }
        );

        // Seed: Clientes Iniciales de Ejemplo
        modelBuilder.Entity<ClienteResidencial>().HasData(
            new ClienteResidencial
            {
                Id = 1,
                Codigo = "CLI-00482",
                Tratamiento = "Sr/Sra",
                NombreNegocio = "Distribuciones JC",
                RazonSocialExtendida = "Juan Carlos Pérez Gómez",
                NombreCompleto = "Juan Carlos Pérez Gómez",
                Nombres = "Juan, Carlos",
                Apellidos = "Pérez, Gómez",
                TipoDocumento = "C.C.",
                NumeroDocumento = "1020485921",
                Telefono = "312 456 7890",
                Email = "jc.perez@distribucionesjc.co",
                EsRural = false,
                ViaTipo = "Calle",
                ViaNumero = "10",
                ViaLetra = "A",
                ViaCardinalidad = "Sur",
                CruceNumero = "5",
                PlacaNumero = "30",
                DireccionEstandarizada = "Calle 10A Sur # 5-30",
                Barrio = "Laureles",
                Municipio = "Medellín",
                Departamento = "Antioquia",
                Pais = "Colombia",
                ZonaTransporte = "Zona Centro-Occidente",
                Centro = "Sede Principal Medellín",
                Estrato = 5,
                ClaseImpuesto = "Persona Natural",
                CondicionPago = "0010 Contado",
                Bloqueado = false,
                FechaCreacion = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new ClienteResidencial
            {
                Id = 2,
                Codigo = "CLI-00319",
                Tratamiento = "Sr/Sra",
                NombreNegocio = "Residencial Los Álamos",
                RazonSocialExtendida = "María Helena Restrepo",
                NombreCompleto = "María Helena Restrepo",
                Nombres = "María",
                Apellidos = "Helena, Restrepo",
                TipoDocumento = "C.C.",
                NumeroDocumento = "43892115",
                Telefono = "300 289 1144",
                Email = "mrestrepo@alamosres.org",
                EsRural = false,
                DireccionEstandarizada = "Carrera 43A # 25-10",
                Barrio = "Zúñiga",
                Municipio = "Envigado",
                Departamento = "Antioquia",
                Pais = "Colombia",
                ZonaTransporte = "Zona Sur Aburrá",
                Centro = "Sede Sur Envigado",
                Estrato = 4,
                ClaseImpuesto = "Persona Natural",
                CondicionPago = "0010 Contado",
                Bloqueado = false,
                FechaCreacion = new DateTime(2026, 1, 18, 14, 30, 0, DateTimeKind.Utc)
            },
            new ClienteResidencial
            {
                Id = 3,
                Codigo = "CLI-00754",
                Tratamiento = "Empresa",
                NombreNegocio = "Inversiones Morales S.A.S.",
                RazonSocialExtendida = "Inversiones Morales S.A.S.",
                NombreCompleto = "Inversiones Morales S.A.S.",
                Nombres = "Inversiones Morales S.A.S.",
                Apellidos = "Inversiones Morales S.A.S.",
                TipoDocumento = "NIT",
                NumeroDocumento = "901442809",
                DigitoVerificacion = 3,
                Telefono = "317 890 2311",
                Email = "gerencia@morales.co",
                EsRural = false,
                DireccionEstandarizada = "Calle 75 Sur # 45-20",
                Barrio = "Laureles",
                Municipio = "Medellín",
                Departamento = "Antioquia",
                Pais = "Colombia",
                ZonaTransporte = "Zona Centro-Occidente",
                Centro = "Sede Principal Medellín",
                Estrato = 3,
                ClaseImpuesto = "Persona Jurídica",
                CondicionPago = "0010 Contado",
                Bloqueado = false,
                FechaCreacion = new DateTime(2026, 2, 1, 9, 15, 0, DateTimeKind.Utc)
            },
            new ClienteResidencial
            {
                Id = 4,
                Codigo = "CLI-00891",
                Tratamiento = "Sr/Sra",
                NombreNegocio = "Urbanización Portal Real",
                RazonSocialExtendida = "Camila Echeverri Salazar",
                NombreCompleto = "Camila Echeverri Salazar",
                Nombres = "Camila",
                Apellidos = "Echeverri, Salazar",
                TipoDocumento = "C.C.",
                NumeroDocumento = "1017332901",
                Telefono = "315 771 0088",
                Email = "camilareal@urbanportal.co",
                EsRural = false,
                DireccionEstandarizada = "Diagonal 50 # 32-15",
                Barrio = "La Floresta",
                Municipio = "Medellín",
                Departamento = "Antioquia",
                Pais = "Colombia",
                ZonaTransporte = "Zona Occidente",
                Centro = "Sede Norte Bello",
                Estrato = 3,
                ClaseImpuesto = "Persona Natural",
                CondicionPago = "0010 Contado",
                Bloqueado = true, // Baja lógica
                FechaRetiro = new DateTime(2026, 2, 20, 16, 0, 0, DateTimeKind.Utc),
                FechaCreacion = new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
