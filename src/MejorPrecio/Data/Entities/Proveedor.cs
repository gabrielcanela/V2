namespace MejorPrecio.Data.Entities;

public class Proveedor
{
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public string? CUIT { get; set; }
    public string Estado { get; set; } = null!;
    public string? Tipo { get; set; }
    public string? Email { get; set; }
    public decimal? ImporteMinOC { get; set; }
    public string? Domicilio { get; set; }
    public string? Localidad { get; set; }
    public string? Provincia { get; set; }
    public string? CodigoPostal { get; set; }
    public string? EstadoId { get; set; }
    public string? Pais { get; set; }
    public string? ModalidadPed { get; set; }

    public ICollection<ProductoProveedor> Productos { get; set; } = new List<ProductoProveedor>();
}
