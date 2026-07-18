namespace MejorPrecio.Models.Import;

public class PrecioImportResult
{
    public List<PrecioImportError> Errores { get; set; } = new();
    public List<PrecioImportRowValida> FilasValidas { get; set; } = new();

    public bool EsValido => Errores.Count == 0;
}
