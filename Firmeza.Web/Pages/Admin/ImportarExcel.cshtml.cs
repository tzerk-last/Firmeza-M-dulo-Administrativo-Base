using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Data;
using Firmeza.Web.Services;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ImportarExcelModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelService _excelService;

        public ImportarExcelModel(ApplicationDbContext context, ExcelService excelService)
        {
            _context = context;
            _excelService = excelService;
        }

        public List<string> Errores { get; set; } = new();
        public string? Mensaje { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnGetExportarProductosAsync()
        {
            var productos = await _context.Productos.ToListAsync();
            var bytes = _excelService.ExportarProductos(productos);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "productos.xlsx");
        }

        public async Task<IActionResult> OnGetExportarClientesAsync()
        {
            var clientes = await _context.Clientes.ToListAsync();
            var bytes = _excelService.ExportarClientes(clientes);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "clientes.xlsx");
        }

        public async Task<IActionResult> OnPostImportarAsync(IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                {
                    Errores.Add("Debes seleccionar un archivo .xlsx");
                    return Page();
                }

                var (productos, clientes, errores) = await _excelService.ImportarDesdeExcelAsync(archivo);
                Errores = errores;
                Mensaje = $"Importación completada: {productos} productos y {clientes} clientes agregados.";
            }
            catch (Exception ex)
            {
                Errores.Add($"Error al procesar el archivo: {ex.Message}");
            }

            return Page();
        }
    }
}
