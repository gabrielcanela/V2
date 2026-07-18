namespace MejorPrecio.Data.Entities;

public class PrecioProveedor
{
    public long Precioid { get; set; }

    public string Proveedor { get; set; } = null!;
    public string Producto { get; set; } = null!;
    public string Region { get; set; } = null!;
    public DateOnly FchDDe { get; set; }
    public DateOnly? FchHta { get; set; }
    public decimal Precio { get; set; }
    public DateTime? FchCreacion { get; set; }
    public string? Zona { get; set; }
    public decimal? ListaprecioId { get; set; }
    public decimal? EstadoLP { get; set; }
    public string ComedorLP { get; set; } = null!;
    public string? TipoPrecio { get; set; }
}
