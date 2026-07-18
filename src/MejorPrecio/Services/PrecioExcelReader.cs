using ClosedXML.Excel;
using MejorPrecio.Models.Import;

namespace MejorPrecio.Services;

public class PrecioExcelReader
{
    private const int PrimeraFilaDatos = 2;

    public List<PrecioImportRow> Leer(Stream excelStream)
    {
        using var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheets.First();

        var filas = new List<PrecioImportRow>();
        var ultimaFila = worksheet.LastRowUsed()?.RowNumber() ?? 0;

        for (var numeroFila = PrimeraFilaDatos; numeroFila <= ultimaFila; numeroFila++)
        {
            var fila = worksheet.Row(numeroFila);
            if (fila.IsEmpty())
            {
                continue;
            }

            filas.Add(new PrecioImportRow
            {
                Fila = numeroFila,
                ProveedorCodigo = fila.Cell(1).GetString().Trim(),
                ProductoProveedCodigo = fila.Cell(2).GetString().Trim(),
                PrecioTexto = fila.Cell(3).GetString().Trim(),
                VigenciaDesdeTexto = fila.Cell(4).GetString().Trim(),
                VigenciaHastaTexto = fila.Cell(5).GetString().Trim(),
            });
        }

        return filas;
    }
}
