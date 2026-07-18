namespace MejorPrecio.Models.Consulta;

public class ConsultaPrecioRow
{
    public string ProveedorCodigo { get; set; } = string.Empty;
    public string ProveedorDescripcion { get; set; } = string.Empty;
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoDescripcion { get; set; } = string.Empty;
    public string FamiliaCodigo { get; set; } = string.Empty;
    public string FamiliaDescripcion { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public DateOnly VigenciaDesde { get; set; }
    public DateOnly VigenciaHasta { get; set; }
}
