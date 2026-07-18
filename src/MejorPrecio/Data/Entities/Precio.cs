namespace MejorPrecio.Data.Entities;

public class Precio
{
    public int Id { get; set; }

    public string Proveedor { get; set; } = null!;
    public string ProductoProveed { get; set; } = null!;
    public string Categoria { get; set; } = null!;

    public decimal PrecioUnitario { get; set; }
    public DateOnly VigenciaDesde { get; set; }
    public DateOnly VigenciaHasta { get; set; }

    public Proveedor ProveedorNavigation { get; set; } = null!;
    public ProductoProveedor ProductoProveedorNavigation { get; set; } = null!;
    public Categoria CategoriaNavigation { get; set; } = null!;
}
