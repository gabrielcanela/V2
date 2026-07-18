namespace MejorPrecio.Models.Consulta;

public class ConsultaPrecioFiltro
{
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public string? ProveedorCodigo { get; set; }
    public string? CategoriaCodigo { get; set; }
    public string? DescripcionProducto { get; set; }
}
