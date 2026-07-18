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

    // RF-18 a RF-21: filtra por fecha (vigencia), proveedor, familia y descripción de producto.
    // RF-23: el "producto" se identifica por su código (ProductoProveed), que es el código propio
    // de la empresa compartido entre los distintos proveedores que lo cotizan.
    public async Task<List<ConsultaPrecioRow>> BuscarAsync(ConsultaPrecioFiltro filtro, CancellationToken ct = default)
    {
        var query = _db.Precios
            .AsNoTracking()
            .Where(p => p.VigenciaDesde <= filtro.Fecha && p.VigenciaHasta >= filtro.Fecha);

        if (!string.IsNullOrWhiteSpace(filtro.ProveedorCodigo))
        {
            query = query.Where(p => p.Proveedor == filtro.ProveedorCodigo);
        }

        if (!string.IsNullOrWhiteSpace(filtro.CategoriaCodigo))
        {
            query = query.Where(p => p.Categoria == filtro.CategoriaCodigo);
        }

        if (!string.IsNullOrWhiteSpace(filtro.DescripcionProducto))
        {
            query = query.Where(p => p.ProductoProveedorNavigation.Descripcion.Contains(filtro.DescripcionProducto));
        }

        var preciosMinimosPorProducto = query
            .GroupBy(p => p.ProductoProveed)
            .Select(g => new { ProductoProveed = g.Key, PrecioMinimo = g.Min(p => p.PrecioUnitario) });

        var candidatos = await query
            .Join(preciosMinimosPorProducto,
                p => new { p.ProductoProveed, Precio = p.PrecioUnitario },
                m => new { m.ProductoProveed, Precio = m.PrecioMinimo },
                (p, _) => new ConsultaPrecioRow
                {
                    ProveedorCodigo = p.Proveedor,
                    ProveedorDescripcion = p.ProveedorNavigation.Descripcion,
                    ProductoCodigo = p.ProductoProveed,
                    ProductoDescripcion = p.ProductoProveedorNavigation.Descripcion,
                    FamiliaCodigo = p.Categoria,
                    FamiliaDescripcion = p.CategoriaNavigation.Descripcion,
                    PrecioUnitario = p.PrecioUnitario,
                    VigenciaDesde = p.VigenciaDesde,
                    VigenciaHasta = p.VigenciaHasta,
                })
            .ToListAsync(ct);

        // Si dos proveedores empatan en el precio mínimo de un mismo producto, nos quedamos con
        // uno solo (RF-23 exige una única fila por producto); el desempate es determinístico.
        return candidatos
            .GroupBy(c => c.ProductoCodigo)
            .Select(g => g.OrderBy(c => c.ProveedorCodigo).First())
            .OrderBy(c => c.FamiliaDescripcion, StringComparer.CurrentCultureIgnoreCase)
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

    public async Task<List<(string Codigo, string Descripcion)>> ObtenerCategoriasAsync(CancellationToken ct = default)
    {
        return await _db.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Descripcion)
            .Select(c => new ValueTuple<string, string>(c.Codigo, c.Descripcion))
            .ToListAsync(ct);
    }
}
