namespace MejorPrecio.Models.Import;

public class PrecioImportRow
{
    public int Fila { get; set; }
    public string ProveedorCodigo { get; set; } = string.Empty;
    public string ProductoProveedCodigo { get; set; } = string.Empty;
    public string PrecioTexto { get; set; } = string.Empty;
    public string VigenciaDesdeTexto { get; set; } = string.Empty;
    public string VigenciaHastaTexto { get; set; } = string.Empty;
}
