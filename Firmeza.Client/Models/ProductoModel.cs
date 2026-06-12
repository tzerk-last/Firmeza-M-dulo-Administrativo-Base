namespace Firmeza.Client.Models;

public class ProductoModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

public class CarritoItem
{
    public ProductoModel Producto { get; set; } = new();
    public int Cantidad { get; set; }
    public decimal Subtotal => Producto.Precio * Cantidad;
}
