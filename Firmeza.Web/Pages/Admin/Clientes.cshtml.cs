using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ClientesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ClientesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Cliente> Clientes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Buscar { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                var query = _context.Clientes.AsQueryable();
                if (!string.IsNullOrEmpty(Buscar))
                    query = query.Where(c => c.Nombre.Contains(Buscar) || c.Documento.Contains(Buscar));
                Clientes = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cargar clientes: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente != null)
                {
                    _context.Clientes.Remove(cliente);
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
