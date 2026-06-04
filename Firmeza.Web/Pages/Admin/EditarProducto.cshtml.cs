using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Firmeza.Web.Data;

namespace Firmeza.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditarProductoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditarProductoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public int Id { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [BindProperty]
        public string Descripcion { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [BindProperty]
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                    return RedirectToPage("/Admin/Productos");

                Id = producto.Id;
                Nombre = producto.Nombre;
                Descripcion = producto.Descripcion;
                Precio = producto.Precio;
                Stock = producto.Stock;
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                    return Page();

                var producto = await _context.Productos.FindAsync(Id);
                if (producto == null)
                    return RedirectToPage("/Admin/Productos");

                producto.Nombre = Nombre;
                producto.Descripcion = Descripcion;
                producto.Precio = Precio;
                producto.Stock = Stock;

                await _context.SaveChangesAsync();
                return RedirectToPage("/Admin/Productos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                return Page();
            }
        }
    }
}
