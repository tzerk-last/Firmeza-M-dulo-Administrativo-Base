using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CrearProductoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CrearProductoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [BindProperty]
        public string Descripcion { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                    return Page();

                var producto = new Producto
                {
                    Nombre = Nombre,
                    Descripcion = Descripcion,
                    Precio = Precio,
                    Stock = Stock
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Admin/Productos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                return Page();
            }
        }
    }
}
