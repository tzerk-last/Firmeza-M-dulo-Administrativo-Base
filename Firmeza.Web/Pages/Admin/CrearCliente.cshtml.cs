using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CrearClienteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public CrearClienteModel(ApplicationDbContext context) => _context = context;

        [BindProperty][Required] public string Nombre { get; set; } = string.Empty;
        [BindProperty][Required] public string Documento { get; set; } = string.Empty;
        [BindProperty][Required][EmailAddress] public string Correo { get; set; } = string.Empty;
        [BindProperty][Required][Phone] public string Telefono { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid) return Page();
                _context.Clientes.Add(new Cliente { Nombre = Nombre, Documento = Documento, Correo = Correo, Telefono = Telefono });
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
