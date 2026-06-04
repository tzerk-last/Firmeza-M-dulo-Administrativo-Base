using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Data;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalProductos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalVentas { get; set; }

        public async Task OnGetAsync()
        {
            TotalProductos = await _context.Productos.CountAsync();
            TotalClientes = await _context.Clientes.CountAsync();
            TotalVentas = await _context.Ventas.CountAsync();
        }
    }
}
