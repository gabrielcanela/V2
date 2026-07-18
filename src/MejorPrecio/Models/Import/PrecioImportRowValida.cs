namespace MejorPrecio.Models.Import;

public class PrecioImportRowValida
{
    public string ProveedorCodigo { get; set; } = string.Empty;
    public string ProductoProveedCodigo { get; set; } = string.Empty;
    public string CategoriaCodigo { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public DateOnly VigenciaDesde { get; set; }
    public DateOnly VigenciaHasta { get; set; }
}
