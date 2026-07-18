namespace MejorPrecio.Data.Entities;

public class Categoria
{
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public string Grupo { get; set; } = null!;
    public string Presentar { get; set; } = null!;

    public ICollection<Precio> Precios { get; set; } = new List<Precio>();
}
