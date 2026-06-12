namespace Firmeza.Client.Models;

public class CrearVentaRequest
{
    public int ClienteId { get; set; }
    public List<DetalleVentaRequest> Detalles { get; set; } = new();
}

public class DetalleVentaRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}

public class VentaResponse
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
