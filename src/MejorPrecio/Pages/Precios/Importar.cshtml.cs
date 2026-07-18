using System.Text.Json;
using MejorPrecio.Models.Import;
using MejorPrecio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MejorPrecio.Pages.Precios;

public class ImportarModel : PageModel
{
    private const string SessionKeyFilasValidas = "PrecioImport:FilasValidas";

    private readonly PrecioImportService _importService;

    public ImportarModel(PrecioImportService importService)
    {
        _importService = importService;
    }

    public List<PrecioImportError> Errores { get; private set; } = new();
    public int? CantidadFilasValidas { get; private set; }
    public int? CantidadImportada { get; private set; }

    public bool MostrarConfirmacion => CantidadFilasValidas is > 0;

    public void OnGet()
    {
        HttpContext.Session.Remove(SessionKeyFilasValidas);

        if (TempData["ImportacionConfirmada"] is int cantidad)
        {
            CantidadImportada = cantidad;
        }
    }

    // RF-15/RF-16: valida el Excel completo y, si hay al menos un error, no toca la base
    // y muestra la lista completa de errores encontrados.
    public async Task<IActionResult> OnPostValidarAsync(IFormFile? archivo)
    {
        HttpContext.Session.Remove(SessionKeyFilasValidas);

        if (archivo is null || archivo.Length == 0)
        {
            Errores.Add(new PrecioImportError { Fila = 0, Mensaje = "Seleccioná un archivo Excel para continuar." });
            return Page();
        }

        await using var stream = archivo.OpenReadStream();
        var resultado = await _importService.LeerYValidarAsync(stream);

        if (!resultado.EsValido)
        {
            Errores = resultado.Errores;
            return Page();
        }

        // RF-17: se guardan las filas validadas en la sesión hasta que el usuario confirme.
        HttpContext.Session.SetString(SessionKeyFilasValidas, JsonSerializer.Serialize(resultado.FilasValidas));
        CantidadFilasValidas = resultado.FilasValidas.Count;
        return Page();
    }

    public async Task<IActionResult> OnPostConfirmarAsync()
    {
        var json = HttpContext.Session.GetString(SessionKeyFilasValidas);
        if (string.IsNullOrEmpty(json))
        {
            return RedirectToPage();
        }

        var filas = JsonSerializer.Deserialize<List<PrecioImportRowValida>>(json) ?? new();
        await _importService.GuardarAsync(filas);
        HttpContext.Session.Remove(SessionKeyFilasValidas);

        TempData["ImportacionConfirmada"] = filas.Count;
        return RedirectToPage();
    }

    public IActionResult OnPostCancelar()
    {
        HttpContext.Session.Remove(SessionKeyFilasValidas);
        return RedirectToPage();
    }
}
