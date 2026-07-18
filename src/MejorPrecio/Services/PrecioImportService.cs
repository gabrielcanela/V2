using MejorPrecio.Data;
using MejorPrecio.Data.Entities;
using MejorPrecio.Models.Import;

namespace MejorPrecio.Services;

public class PrecioImportService
{
    private readonly PrecioExcelReader _reader;
    private readonly PrecioImportValidator _validator;
    private readonly MejorPrecioContext _db;

    public PrecioImportService(PrecioExcelReader reader, PrecioImportValidator validator, MejorPrecioContext db)
    {
        _reader = reader;
        _validator = validator;
        _db = db;
    }

    public async Task<PrecioImportResult> LeerYValidarAsync(Stream excelStream, CancellationToken ct = default)
    {
        var filas = _reader.Leer(excelStream);
        return await _validator.ValidarAsync(filas, ct);
    }

    // RF-15/RF-17: solo se llama después de confirmar, con filas ya validadas sin errores.
    // Un único SaveChanges deja la persistencia atómica (todo o nada).
    public async Task GuardarAsync(List<PrecioImportRowValida> filas, CancellationToken ct = default)
    {
        // Region y ComedorLP son NOT NULL en PrecioProveedor pero el Excel de importación
        // no los captura todavía: Region se fija en "BUE" y ComedorLP queda en blanco.
        var entidades = filas.Select(f => new PrecioProveedor
        {
            Proveedor = f.ProveedorCodigo,
            Producto = f.ProductoProveedCodigo,
            Region = "BUE",
            FchDDe = f.VigenciaDesde,
            FchHta = f.VigenciaHasta,
            Precio = f.PrecioUnitario,
            ComedorLP = string.Empty,
        });

        _db.PreciosProveedor.AddRange(entidades);
        await _db.SaveChangesAsync(ct);
    }
}
