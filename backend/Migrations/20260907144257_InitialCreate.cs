using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Barrios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Municipio = table.Column<string>(type: "text", nullable: false),
                    Departamento = table.Column<string>(type: "text", nullable: false),
                    Pais = table.Column<string>(type: "text", nullable: false),
                    ZonaTransporte = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barrios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Tratamiento = table.Column<string>(type: "text", nullable: false),
                    NombreNegocio = table.Column<string>(type: "text", nullable: false),
                    RazonSocialExtendida = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    Apellidos = table.Column<string>(type: "text", nullable: false),
                    TipoDocumento = table.Column<string>(type: "text", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "text", nullable: false),
                    DigitoVerificacion = table.Column<int>(type: "integer", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Celular = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    EsRural = table.Column<bool>(type: "boolean", nullable: false),
                    DireccionRural = table.Column<string>(type: "text", nullable: true),
                    ViaTipo = table.Column<string>(type: "text", nullable: true),
                    ViaNumero = table.Column<string>(type: "text", nullable: true),
                    ViaLetra = table.Column<string>(type: "text", nullable: true),
                    ViaCardinalidad = table.Column<string>(type: "text", nullable: true),
                    CruceNumero = table.Column<string>(type: "text", nullable: true),
                    PlacaNumero = table.Column<string>(type: "text", nullable: true),
                    DireccionEstandarizada = table.Column<string>(type: "text", nullable: false),
                    Barrio = table.Column<string>(type: "text", nullable: false),
                    Municipio = table.Column<string>(type: "text", nullable: false),
                    Departamento = table.Column<string>(type: "text", nullable: false),
                    Pais = table.Column<string>(type: "text", nullable: false),
                    ZonaTransporte = table.Column<string>(type: "text", nullable: true),
                    Centro = table.Column<string>(type: "text", nullable: false),
                    Estrato = table.Column<int>(type: "integer", nullable: false),
                    ClaseImpuesto = table.Column<string>(type: "text", nullable: false),
                    CondicionPago = table.Column<string>(type: "text", nullable: false),
                    Bloqueado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaRetiro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientesCanalModerno",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoDocumento = table.Column<string>(type: "text", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "text", nullable: false),
                    NombreCadena = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesCanalModerno", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Barrios",
                columns: new[] { "Id", "Departamento", "Municipio", "Nombre", "Pais", "ZonaTransporte" },
                values: new object[,]
                {
                    { 1, "Antioquia", "Medellín", "Laureles", "Colombia", "Zona Centro-Occidente" },
                    { 2, "Antioquia", "Medellín", "El Poblado", "Colombia", "Zona Sur-Oriente" },
                    { 3, "Antioquia", "Medellín", "La Floresta", "Colombia", "Zona Occidente" },
                    { 4, "Antioquia", "Envigado", "Zúñiga", "Colombia", "Zona Sur Aburrá" },
                    { 5, "Cundinamarca", "Bogotá D.C.", "Chapinero", "Colombia", "Zona Centro Capital" }
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Apellidos", "Barrio", "Bloqueado", "Celular", "Centro", "ClaseImpuesto", "Codigo", "CondicionPago", "CruceNumero", "Departamento", "DigitoVerificacion", "DireccionEstandarizada", "DireccionRural", "Email", "EsRural", "Estrato", "FechaCreacion", "FechaModificacion", "FechaRetiro", "Municipio", "NombreCompleto", "NombreNegocio", "Nombres", "NumeroDocumento", "Pais", "PlacaNumero", "RazonSocialExtendida", "Telefono", "TipoDocumento", "Tratamiento", "ViaCardinalidad", "ViaLetra", "ViaNumero", "ViaTipo", "ZonaTransporte" },
                values: new object[,]
                {
                    { 1, "Pérez, Gómez", "Laureles", false, null, "Sede Principal Medellín", "Persona Natural", "CLI-00482", "0010 Contado", "5", "Antioquia", null, "Calle 10A Sur # 5-30", null, "jc.perez@distribucionesjc.co", false, 5, new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), null, null, "Medellín", "Juan Carlos Pérez Gómez", "Distribuciones JC", "Juan, Carlos", "1020485921", "Colombia", "30", "Juan Carlos Pérez Gómez", "312 456 7890", "C.C.", "Sr/Sra", "Sur", "A", "10", "Calle", "Zona Centro-Occidente" },
                    { 2, "Helena, Restrepo", "Zúñiga", false, null, "Sede Sur Envigado", "Persona Natural", "CLI-00319", "0010 Contado", null, "Antioquia", null, "Carrera 43A # 25-10", null, "mrestrepo@alamosres.org", false, 4, new DateTime(2026, 1, 18, 14, 30, 0, 0, DateTimeKind.Utc), null, null, "Envigado", "María Helena Restrepo", "Residencial Los Álamos", "María", "43892115", "Colombia", null, "María Helena Restrepo", "300 289 1144", "C.C.", "Sr/Sra", null, null, null, null, "Zona Sur Aburrá" },
                    { 3, "Inversiones Morales S.A.S.", "Laureles", false, null, "Sede Principal Medellín", "Persona Jurídica", "CLI-00754", "0010 Contado", null, "Antioquia", 3, "Calle 75 Sur # 45-20", null, "gerencia@morales.co", false, 3, new DateTime(2026, 2, 1, 9, 15, 0, 0, DateTimeKind.Utc), null, null, "Medellín", "Inversiones Morales S.A.S.", "Inversiones Morales S.A.S.", "Inversiones Morales S.A.S.", "901442809", "Colombia", null, "Inversiones Morales S.A.S.", "317 890 2311", "NIT", "Empresa", null, null, null, null, "Zona Centro-Occidente" },
                    { 4, "Echeverri, Salazar", "La Floresta", true, null, "Sede Norte Bello", "Persona Natural", "CLI-00891", "0010 Contado", null, "Antioquia", null, "Diagonal 50 # 32-15", null, "camilareal@urbanportal.co", false, 3, new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 2, 20, 16, 0, 0, 0, DateTimeKind.Utc), "Medellín", "Camila Echeverri Salazar", "Urbanización Portal Real", "Camila", "1017332901", "Colombia", null, "Camila Echeverri Salazar", "315 771 0088", "C.C.", "Sr/Sra", null, null, null, null, "Zona Occidente" }
                });

            migrationBuilder.InsertData(
                table: "ClientesCanalModerno",
                columns: new[] { "Id", "NombreCadena", "NumeroDocumento", "TipoDocumento" },
                values: new object[,]
                {
                    { 1, "Éxito Poblado", "12345678", "C.C." },
                    { 2, "Jumbo Las Vegas", "900111222", "NIT" },
                    { 3, "Olímpica Calle 10", "800123456", "NIT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Codigo",
                table: "Clientes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_NumeroDocumento",
                table: "Clientes",
                column: "NumeroDocumento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Barrios");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "ClientesCanalModerno");
        }
    }
}
