using MejorPrecio.Models.Consulta;
using MejorPrecio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MejorPrecio.Pages.Precios;

public class ConsultarModel : PageModel
{
    private readonly PrecioConsultaService _consultaService;

    public ConsultarModel(PrecioConsultaService consultaService)
    {
        _consultaService = consultaService;
    }

    // RF-18: la fecha siempre filtra (con hoy como valor por defecto).
    [BindProperty(SupportsGet = true)]
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    // RF-19: null/vacío equivale a "Todos los proveedores".
    [BindProperty(SupportsGet = true)]
    public string? ProveedorCodigo { get; set; }

    // RF-20: null/vacío equivale a "Todas las familias".
    [BindProperty(SupportsGet = true)]
    public string? CategoriaCodigo { get; set; }

    // RF-21: vacío no aplica filtro por descripción.
    [BindProperty(SupportsGet = true)]
    public string? DescripcionProducto { get; set; }

    public List<ConsultaPrecioRow> Resultados { get; private set; } = new();
    public List<(string Codigo, string Descripcion)> Proveedores { get; private set; } = new();
    public List<(string Codigo, string Descripcion)> Categorias { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Proveedores = await _consultaService.ObtenerProveedoresAsync();
        Categorias = await _consultaService.ObtenerCategoriasAsync();

        Resultados = await _consultaService.BuscarAsync(new ConsultaPrecioFiltro
        {
            Fecha = Fecha,
            ProveedorCodigo = ProveedorCodigo,
            CategoriaCodigo = CategoriaCodigo,
            DescripcionProducto = DescripcionProducto,
        });
    }
}
