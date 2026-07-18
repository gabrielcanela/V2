using MejorPrecio.Data;
using MejorPrecio.Models.Consulta;
using Microsoft.EntityFrameworkCore;

namespace MejorPrecio.Services;

public class PrecioConsultaService
{
    private readonly MejorPrecioContext _db;

    public PrecioConsultaService(MejorPrecioContext db)
    {
        _db = db;
    }

    // RF-18, RF-19, RF-21: filtra por fecha (vigencia), proveedor y descripción de producto.
    // RF-23: el "producto" se identifica por su código (ProductoProveed), que es el código propio
    // de la empresa compartido entre los distintos proveedores que lo cotizan.
    // PrecioProveedor no declara FK hacia Proveedor/ProductoProveedor, así que las descripciones
    // se resuelven acá con joins explícitos en vez de navigation properties.
    public async Task<List<ConsultaPrecioRow>> BuscarAsync(ConsultaPrecioFiltro filtro, CancellationToken ct = default)
    {
        var query = _db.PreciosProveedor
            .AsNoTracking()
            .Where(p => p.FchDDe <= filtro.Fecha && (p.FchHta == null || p.FchHta >= filtro.Fecha));

        if (!string.IsNullOrWhiteSpace(filtro.ProveedorCodigo))
        {
            query = query.Where(p => p.Proveedor == filtro.ProveedorCodigo);
        }

        var vigentesConProducto = query.Join(_db.ProductosProveedor,
            p => new { p.Proveedor, Producto = p.Producto },
            pp => new { pp.Proveedor, Producto = pp.ProductoProveed },
            (p, pp) => new { Precio = p, ProductoDescripcion = pp.Descripcion });

        if (!string.IsNullOrWhiteSpace(filtro.DescripcionProducto))
        {
            vigentesConProducto = vigentesConProducto.Where(x => x.ProductoDescripcion.Contains(filtro.DescripcionProducto));
        }

        var preciosMinimosPorProducto = vigentesConProducto
            .GroupBy(x => x.Precio.Producto)
            .Select(g => new { Producto = g.Key, PrecioMinimo = g.Min(x => x.Precio.Precio) });

        var candidatos = await vigentesConProducto
            .Join(preciosMinimosPorProducto,
                x => new { x.Precio.Producto, Precio = x.Precio.Precio },
                m => new { m.Producto, Precio = m.PrecioMinimo },
                (x, _) => x)
            .Join(_db.Proveedores,
                x => x.Precio.Proveedor,
                prov => prov.Codigo,
                (x, prov) => new ConsultaPrecioRow
                {
                    ProveedorCodigo = x.Precio.Proveedor,
                    ProveedorDescripcion = prov.Descripcion,
                    ProductoCodigo = x.Precio.Producto,
                    ProductoDescripcion = x.ProductoDescripcion,
                    PrecioUnitario = x.Precio.Precio,
                    VigenciaDesde = x.Precio.FchDDe,
                    VigenciaHasta = x.Precio.FchHta,
                })
            .ToListAsync(ct);

        // Si dos proveedores empatan en el precio mínimo de un mismo producto, nos quedamos con
        // uno solo (RF-23 exige una única fila por producto); el desempate es determinístico.
        return candidatos
            .GroupBy(c => c.ProductoCodigo)
            .Select(g => g.OrderBy(c => c.ProveedorCodigo).First())
            .OrderBy(c => c.ProductoDescripcion, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(c => c.PrecioUnitario)
            .ToList();
    }

    public async Task<List<(string Codigo, string Descripcion)>> ObtenerProveedoresAsync(CancellationToken ct = default)
    {
        return await _db.Proveedores
            .AsNoTracking()
            .OrderBy(p => p.Descripcion)
            .Select(p => new ValueTuple<string, string>(p.Codigo, p.Descripcion))
            .ToListAsync(ct);
    }
}
