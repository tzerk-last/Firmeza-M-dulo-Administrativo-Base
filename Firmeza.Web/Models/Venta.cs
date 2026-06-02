namespace Firmeza.Web.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public List<DetalleVenta> Detalles { get; set; } = new();
    }
}
