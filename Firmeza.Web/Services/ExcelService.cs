using OfficeOpenXml;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

namespace Firmeza.Web.Services
{
    public class ExcelService
    {
        private readonly ApplicationDbContext _context;

        public ExcelService(ApplicationDbContext context)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Firmeza");
        }

        public ExcelService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            ExcelPackage.License.SetNonCommercialPersonal("Firmeza");
        }

        // ── IMPORTAR ──────────────────────────────────────────────
        public async Task<(int productos, int clientes, List<string> errores)> ImportarDesdeExcelAsync(IFormFile archivo)
        {
            var errores = new List<string>();
            int totalProductos = 0, totalClientes = 0;

            using var stream = new MemoryStream();
            await archivo.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var hoja = package.Workbook.Worksheets.FirstOrDefault();
            if (hoja == null)
            {
                errores.Add("El archivo no contiene hojas.");
                return (0, 0, errores);
            }

            int filas = hoja.Dimension?.Rows ?? 0;

            for (int i = 2; i <= filas; i++)
            {
                try
                {
                    string tipo = hoja.Cells[i, 1].Text.Trim().ToLower();

                    if (tipo == "producto")
                    {
                        string nombre = hoja.Cells[i, 2].Text.Trim();
                        string descripcion = hoja.Cells[i, 3].Text.Trim();
                        string precioStr = hoja.Cells[i, 4].Text.Trim();
                        string stockStr = hoja.Cells[i, 5].Text.Trim();

                        if (string.IsNullOrEmpty(nombre))
                        { errores.Add($"Fila {i}: Nombre de producto vacío."); continue; }

                        if (!decimal.TryParse(precioStr, out decimal precio))
                        { errores.Add($"Fila {i}: Precio inválido '{precioStr}'."); continue; }

                        if (!int.TryParse(stockStr, out int stock))
                        { errores.Add($"Fila {i}: Stock inválido '{stockStr}'."); continue; }

                        _context.Productos.Add(new Producto
                        {
                            Nombre = nombre,
                            Descripcion = descripcion,
                            Precio = precio,
                            Stock = stock
                        });
                        totalProductos++;
                    }
                    else if (tipo == "cliente")
                    {
                        string nombre = hoja.Cells[i, 2].Text.Trim();
                        string documento = hoja.Cells[i, 3].Text.Trim();
                        string correo = hoja.Cells[i, 4].Text.Trim();
                        string telefono = hoja.Cells[i, 5].Text.Trim();

                        if (string.IsNullOrEmpty(nombre))
                        { errores.Add($"Fila {i}: Nombre de cliente vacío."); continue; }

                        if (string.IsNullOrEmpty(correo))
                        { errores.Add($"Fila {i}: Correo vacío en fila {i}."); continue; }

                        _context.Clientes.Add(new Cliente
                        {
                            Nombre = nombre,
                            Documento = documento,
                            Correo = correo,
                            Telefono = telefono
                        });
                        totalClientes++;
                    }
                    else
                    {
                        errores.Add($"Fila {i}: Tipo desconocido '{tipo}'. Use 'producto' o 'cliente'.");
                    }
                }
                catch (Exception ex)
                {
                    errores.Add($"Fila {i}: Error inesperado – {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            return (totalProductos, totalClientes, errores);
        }

        // ── EXPORTAR PRODUCTOS ────────────────────────────────────
        public byte[] ExportarProductos(List<Producto> productos)
        {
            using var package = new ExcelPackage();
            var hoja = package.Workbook.Worksheets.Add("Productos");

            hoja.Cells[1, 1].Value = "ID";
            hoja.Cells[1, 2].Value = "Nombre";
            hoja.Cells[1, 3].Value = "Descripción";
            hoja.Cells[1, 4].Value = "Precio";
            hoja.Cells[1, 5].Value = "Stock";

            for (int i = 0; i < productos.Count; i++)
            {
                hoja.Cells[i + 2, 1].Value = productos[i].Id;
                hoja.Cells[i + 2, 2].Value = productos[i].Nombre;
                hoja.Cells[i + 2, 3].Value = productos[i].Descripcion;
                hoja.Cells[i + 2, 4].Value = productos[i].Precio;
                hoja.Cells[i + 2, 5].Value = productos[i].Stock;
            }

            hoja.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        // ── EXPORTAR CLIENTES ─────────────────────────────────────
        public byte[] ExportarClientes(List<Cliente> clientes)
        {
            using var package = new ExcelPackage();
            var hoja = package.Workbook.Worksheets.Add("Clientes");

            hoja.Cells[1, 1].Value = "ID";
            hoja.Cells[1, 2].Value = "Nombre";
            hoja.Cells[1, 3].Value = "Documento";
            hoja.Cells[1, 4].Value = "Correo";
            hoja.Cells[1, 5].Value = "Teléfono";

            for (int i = 0; i < clientes.Count; i++)
            {
                hoja.Cells[i + 2, 1].Value = clientes[i].Id;
                hoja.Cells[i + 2, 2].Value = clientes[i].Nombre;
                hoja.Cells[i + 2, 3].Value = clientes[i].Documento;
                hoja.Cells[i + 2, 4].Value = clientes[i].Correo;
                hoja.Cells[i + 2, 5].Value = clientes[i].Telefono;
            }

            hoja.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
}
