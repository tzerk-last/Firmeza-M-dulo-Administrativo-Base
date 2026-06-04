using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DetalleVentaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DetalleVentaModel(ApplicationDbContext context) => _context = context;

        public Venta? Venta { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (Venta == null) return RedirectToPage("/Admin/Ventas");
            return Page();
        }
    }
}
