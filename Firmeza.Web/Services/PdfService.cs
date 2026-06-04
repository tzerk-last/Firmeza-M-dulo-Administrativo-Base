using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Firmeza.Web.Models;

namespace Firmeza.Web.Services
{
    public class PdfService
    {
        private readonly IWebHostEnvironment _env;

        public PdfService(IWebHostEnvironment env)
        {
            _env = env;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public string GenerarRecibo(Venta venta)
        {
            var carpeta = Path.Combine(_env.WebRootPath, "recibos");
            Directory.CreateDirectory(carpeta);
            var archivo = Path.Combine(carpeta, $"recibo_{venta.Id}.pdf");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text($"🏗️ Firmeza – Recibo de Venta #{venta.Id}")
                        .SemiBold().FontSize(18).FontColor(Colors.Black);

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Cliente: {venta.Cliente?.Nombre}");
                        col.Item().Text($"Fecha: {venta.Fecha:dd/MM/yyyy HH:mm}");

                        col.Item().LineHorizontal(1);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Producto").SemiBold();
                                header.Cell().Text("Cant.").SemiBold();
                                header.Cell().Text("Precio Unit.").SemiBold();
                                header.Cell().Text("Subtotal").SemiBold();
                            });

                            foreach (var d in venta.Detalles)
                            {
                                table.Cell().Text(d.Producto?.Nombre ?? "");
                                table.Cell().Text(d.Cantidad.ToString());
                                table.Cell().Text($"${d.PrecioUnitario:N2}");
                                table.Cell().Text($"${d.Cantidad * d.PrecioUnitario:N2}");
                            }
                        });

                        col.Item().LineHorizontal(1);

                        var subtotal = venta.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
                        var iva = subtotal * 0.19m;
                        var total = subtotal + iva;

                        col.Item().AlignRight().Text($"Subtotal: ${subtotal:N2}");
                        col.Item().AlignRight().Text($"IVA (19%): ${iva:N2}");
                        col.Item().AlignRight().Text($"Total: ${total:N2}").SemiBold().FontSize(14);
                    });

                    page.Footer().AlignCenter()
                        .Text("Firmeza – Materiales de Construcción")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                });
            }).GeneratePdf(archivo);

            return $"/recibos/recibo_{venta.Id}.pdf";
        }
    }
}
