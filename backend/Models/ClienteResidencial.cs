namespace backend.Models;

public class ClienteResidencial
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = "Sr/Sra"; // Sr/Sra o Empresa
    public string NombreNegocio { get; set; } = string.Empty;
    public string RazonSocialExtendida { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;

    public string TipoDocumento { get; set; } = "C.C."; // C.C., C.E., NIT
    public string NumeroDocumento { get; set; } = string.Empty;
    public int? DigitoVerificacion { get; set; }

    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public string? Email { get; set; }

    public bool EsRural { get; set; } = false;
    public string? DireccionRural { get; set; }
    public string? ViaTipo { get; set; }
    public string? ViaNumero { get; set; }
    public string? ViaLetra { get; set; }
    public string? ViaCardinalidad { get; set; }
    public string? CruceNumero { get; set; }
    public string? PlacaNumero { get; set; }
    public string DireccionEstandarizada { get; set; } = string.Empty;

    public string Barrio { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Pais { get; set; } = "Colombia";
    public string? ZonaTransporte { get; set; }

    public string Centro { get; set; } = string.Empty;
    public int Estrato { get; set; } = 3;
    public string ClaseImpuesto { get; set; } = "Persona Natural"; // Persona Natural o Jurídica
    public string CondicionPago { get; set; } = "0010 Contado";

    public bool Bloqueado { get; set; } = false;
    public DateTime? FechaRetiro { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaModificacion { get; set; }
}
