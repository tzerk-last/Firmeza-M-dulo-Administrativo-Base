using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Firmeza.Web.Data;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditarClienteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditarClienteModel(ApplicationDbContext context) => _context = context;

        [BindProperty] public int Id { get; set; }
        [BindProperty][Required] public string Nombre { get; set; } = string.Empty;
        [BindProperty] public string Documento { get; set; } = string.Empty;
        [BindProperty][Required][EmailAddress] public string Correo { get; set; } = string.Empty;
        [BindProperty] public string Telefono { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return RedirectToPage("/Admin/Clientes");
            Id = cliente.Id; Nombre = cliente.Nombre; Documento = cliente.Documento;
            Correo = cliente.Correo; Telefono = cliente.Telefono;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid) return Page();
                var cliente = await _context.Clientes.FindAsync(Id);
                if (cliente == null) return RedirectToPage("/Admin/Clientes");
                cliente.Nombre = Nombre; cliente.Documento = Documento;
                cliente.Correo = Correo; cliente.Telefono = Telefono;
                await _context.SaveChangesAsync();
                return RedirectToPage("/Admin/Clientes");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return Page();
            }
        }
    }
}
