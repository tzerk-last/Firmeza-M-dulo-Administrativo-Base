namespace Firmeza.API.DTOs;

public class VentaDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public List<DetalleVentaDto> Detalles { get; set; } = new();
    public decimal Total => Detalles.Sum(d => d.Subtotal);
}

public class DetalleVentaDto
{
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}

public class CrearVentaDto
{
    public int ClienteId { get; set; }
    public List<CrearDetalleVentaDto> Detalles { get; set; } = new();
}

public class CrearDetalleVentaDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}
