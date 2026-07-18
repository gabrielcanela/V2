using System.Globalization;
using MejorPrecio.Data;
using MejorPrecio.Models.Import;
using Microsoft.EntityFrameworkCore;

namespace MejorPrecio.Services;

public class PrecioImportValidator
{
    private static readonly CultureInfo CulturaArgentina = CultureInfo.GetCultureInfo("es-AR");

    private readonly MejorPrecioContext _db;

    public PrecioImportValidator(MejorPrecioContext db)
    {
        _db = db;
    }

    public async Task<PrecioImportResult> ValidarAsync(List<PrecioImportRow> filas, CancellationToken ct = default)
    {
        var result = new PrecioImportResult();

        var codigosProveedor = filas.Select(f => f.ProveedorCodigo)
            .Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
        var codigosCategoria = filas.Select(f => f.CategoriaCodigo)
            .Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();

        var proveedoresExistentes = (await _db.Proveedores
                .Where(p => codigosProveedor.Contains(p.Codigo))
                .Select(p => p.Codigo)
                .ToListAsync(ct))
            .ToHashSet();

        var categoriasExistentes = (await _db.Categorias
                .Where(c => codigosCategoria.Contains(c.Codigo))
                .Select(c => c.Codigo)
                .ToListAsync(ct))
            .ToHashSet();

        var proveedoresConProducto = filas
            .Where(f => !string.IsNullOrWhiteSpace(f.ProveedorCodigo) && !string.IsNullOrWhiteSpace(f.ProductoProveedCodigo))
            .Select(f => f.ProveedorCodigo)
            .Distinct()
            .ToList();

        var productosExistentes = (await _db.ProductosProveedor
                .Where(pp => proveedoresConProducto.Contains(pp.Proveedor))
                .Select(pp => new { pp.Proveedor, pp.ProductoProveed })
                .ToListAsync(ct))
            .Select(pp => (pp.Proveedor, pp.ProductoProveed))
            .ToHashSet();

        var hoy = DateOnly.FromDateTime(DateTime.Now);

        foreach (var fila in filas)
        {
            var errores = new List<string>();

            var proveedorOk = ValidarProveedor(fila, proveedoresExistentes, errores);
            ValidarProducto(fila, proveedorOk, productosExistentes, errores);
            ValidarCategoria(fila, categoriasExistentes, errores);
            var precioOk = ValidarPrecio(fila, errores, out var precio);
            var vigenciaDesdeOk = ValidarVigenciaDesde(fila, hoy, errores, out var vigenciaDesde);
            ValidarVigenciaHasta(fila, vigenciaDesdeOk, vigenciaDesde, errores, out var vigenciaHasta);

            if (errores.Count > 0)
            {
                result.Errores.AddRange(errores.Select(mensaje => new PrecioImportError { Fila = fila.Fila, Mensaje = mensaje }));
                continue;
            }

            result.FilasValidas.Add(new PrecioImportRowValida
            {
                ProveedorCodigo = fila.ProveedorCodigo,
                ProductoProveedCodigo = fila.ProductoProveedCodigo,
                CategoriaCodigo = fila.CategoriaCodigo,
                PrecioUnitario = precio,
                VigenciaDesde = vigenciaDesde,
                VigenciaHasta = vigenciaHasta,
            });
        }

        return result;
    }

    // RF-01, RF-02
    private static bool ValidarProveedor(PrecioImportRow fila, HashSet<string> proveedoresExistentes, List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(fila.ProveedorCodigo))
        {
            errores.Add("Falta código de proveedor");
            return false;
        }

        if (!proveedoresExistentes.Contains(fila.ProveedorCodigo))
        {
            errores.Add("Código de proveedor inexistente");
            return false;
        }

        return true;
    }

    // RF-03, RF-04
    private static bool ValidarProducto(PrecioImportRow fila, bool proveedorOk, HashSet<(string Proveedor, string Producto)> productosExistentes, List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(fila.ProductoProveedCodigo))
        {
            errores.Add("Falta código de producto");
            return false;
        }

        if (proveedorOk && !productosExistentes.Contains((fila.ProveedorCodigo, fila.ProductoProveedCodigo)))
        {
            errores.Add($"Código de producto inexistente para el proveedor {fila.ProveedorCodigo}");
            return false;
        }

        return true;
    }

    // RF-05, RF-06
    private static bool ValidarCategoria(PrecioImportRow fila, HashSet<string> categoriasExistentes, List<string> errores)
    {
        if (string.IsNullOrWhiteSpace(fila.CategoriaCodigo))
        {
            errores.Add("Falta familia");
            return false;
        }

        if (!categoriasExistentes.Contains(fila.CategoriaCodigo))
        {
            errores.Add("Familia inexistente");
            return false;
        }

        return true;
    }

    // RF-07, RF-08, RF-09
    private static bool ValidarPrecio(PrecioImportRow fila, List<string> errores, out decimal precio)
    {
        precio = 0;

        if (string.IsNullOrWhiteSpace(fila.PrecioTexto))
        {
            errores.Add("Falta Precio");
            return false;
        }

        if (!TryParsePrecio(fila.PrecioTexto, out precio))
        {
            errores.Add("Precio no es un número válido");
            return false;
        }

        if (!TieneExactamenteDosDecimales(fila.PrecioTexto))
        {
            errores.Add("El precio debe tener exactamente dos decimales");
            return false;
        }

        return true;
    }

    // RF-10, RF-11, RF-12
    private static bool ValidarVigenciaDesde(PrecioImportRow fila, DateOnly hoy, List<string> errores, out DateOnly vigenciaDesde)
    {
        vigenciaDesde = default;

        if (string.IsNullOrWhiteSpace(fila.VigenciaDesdeTexto))
        {
            errores.Add("Falta Vigencia desde");
            return false;
        }

        if (!TryParseFecha(fila.VigenciaDesdeTexto, out vigenciaDesde))
        {
            errores.Add("Vigencia desde no es una fecha válida");
            return false;
        }

        if (vigenciaDesde <= hoy)
        {
            errores.Add("Vigencia desde debe ser mayor a la fecha actual");
            return false;
        }

        return true;
    }

    // RF-13, RF-14
    private static bool ValidarVigenciaHasta(PrecioImportRow fila, bool vigenciaDesdeOk, DateOnly vigenciaDesde, List<string> errores, out DateOnly vigenciaHasta)
    {
        if (!TryParseFecha(fila.VigenciaHastaTexto, out vigenciaHasta))
        {
            errores.Add("Vigencia hasta no es una fecha válida");
            return false;
        }

        if (vigenciaDesdeOk && vigenciaHasta <= vigenciaDesde)
        {
            errores.Add("Vigencia hasta debe ser posterior a Vigencia desde");
            return false;
        }

        return true;
    }

    private static bool TryParsePrecio(string texto, out decimal valor)
    {
        var normalizado = NormalizarNumero(texto);
        return decimal.TryParse(normalizado, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out valor);
    }

    private static bool TieneExactamenteDosDecimales(string texto)
    {
        var normalizado = NormalizarNumero(texto);
        var indiceSeparador = normalizado.IndexOf('.');
        if (indiceSeparador < 0)
        {
            return false;
        }

        return normalizado.Length - indiceSeparador - 1 == 2;
    }

    private static string NormalizarNumero(string texto)
    {
        texto = texto.Trim();
        var tieneComa = texto.Contains(',');
        var tienePunto = texto.Contains('.');

        if (tieneComa && tienePunto)
        {
            // Formato es-AR con miles: 1.234,56
            return texto.Replace(".", string.Empty).Replace(',', '.');
        }

        if (tieneComa)
        {
            return texto.Replace(',', '.');
        }

        return texto;
    }

    private static bool TryParseFecha(string texto, out DateOnly fecha)
    {
        fecha = default;
        if (string.IsNullOrWhiteSpace(texto))
        {
            return false;
        }

        if (DateTime.TryParse(texto, CulturaArgentina, DateTimeStyles.None, out var valor) ||
            DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.None, out valor))
        {
            fecha = DateOnly.FromDateTime(valor);
            return true;
        }

        return false;
    }
}
