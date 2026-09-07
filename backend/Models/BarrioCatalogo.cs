namespace backend.Models;

public class BarrioCatalogo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Pais { get; set; } = "Colombia";
    public string ZonaTransporte { get; set; } = string.Empty;
}
