namespace MejorPrecio.Data.Entities;

public class ProductoProveedor
{
    public string Proveedor { get; set; } = null!;
    public string ProductoProveed { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public string? UnidadProveed { get; set; }

    public Proveedor ProveedorNavigation { get; set; } = null!;
    public ICollection<Precio> Precios { get; set; } = new List<Precio>();
}
