using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class VentasModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public VentasModel(ApplicationDbContext context) => _context = context;

        public List<Venta> Ventas { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                Ventas = await _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Detalles)
                    .OrderByDescending(v => v.Fecha)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                var venta = await _context.Ventas.Include(v => v.Detalles).FirstOrDefaultAsync(v => v.Id == id);
                if (venta != null)
                {
                    _context.DetallesVenta.RemoveRange(venta.Detalles);
                    _context.Ventas.Remove(venta);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }
            return RedirectToPage();
        }
    }
}
