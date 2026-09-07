namespace backend.DTOs;

public class ClienteCreateDto
{
    public string Tratamiento { get; set; } = "Sr/Sra"; // Sr/Sra o Empresa
    public string NombreNegocio { get; set; } = string.Empty;
    public string RazonSocialExtendida { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = "C.C.";
    public string NumeroDocumento { get; set; } = string.Empty;

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

    public string Barrio { get; set; } = string.Empty;
    public string Centro { get; set; } = string.Empty;
    public int Estrato { get; set; } = 3;
}

public class ClienteUpdateDto
{
    // Solo campos permitidos por el PDF (Tratamiento, Documento y Condiciones son solo lectura)
    public string NombreNegocio { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;

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

    public string Barrio { get; set; } = string.Empty;
    public string Centro { get; set; } = string.Empty;
    public int Estrato { get; set; } = 3;
}

public class ClienteResponseDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public string NombreNegocio { get; set; } = string.Empty;
    public string RazonSocialExtendida { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;

    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public int? DigitoVerificacion { get; set; }

    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public string? Email { get; set; }

    public bool EsRural { get; set; }
    public string? DireccionRural { get; set; }
    public string DireccionEstandarizada { get; set; } = string.Empty;

    public string Barrio { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string? ZonaTransporte { get; set; }

    public string Centro { get; set; } = string.Empty;
    public int Estrato { get; set; }
    public string ClaseImpuesto { get; set; } = string.Empty;
    public string CondicionPago { get; set; } = string.Empty;

    public bool Bloqueado { get; set; }
    public DateTime? FechaRetiro { get; set; }
    public string? Advertencia { get; set; } // "Cliente bloqueado" si aplica
}

public class ClienteListDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string NombreNegocio { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string Municipio { get; set; } = string.Empty;
    public string Barrio { get; set; } = string.Empty;
    public int Estrato { get; set; }
    public bool Bloqueado { get; set; }
}
