using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ProductosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProductosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Producto> Productos { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Buscar { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                var query = _context.Productos.AsQueryable();
                if (!string.IsNullOrEmpty(Buscar))
                    query = query.Where(p => p.Nombre.Contains(Buscar));
                Productos = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cargar productos: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto != null)
                {
                    _context.Productos.Remove(producto);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar: {ex.Message}");
            }
            return RedirectToPage();
        }
    }
}
