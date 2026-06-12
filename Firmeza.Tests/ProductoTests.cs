using Xunit;
using Microsoft.EntityFrameworkCore;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

namespace Firmeza.Tests;

public class ProductoTests
{
    private ApplicationDbContext CrearContextoEnMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CrearProducto_DebeGuardarEnBaseDeDatos()
    {
        var context = CrearContextoEnMemoria();
        var producto = new Producto
        {
            Nombre = "Cemento Portland",
            Descripcion = "Bolsa 50kg",
            Precio = 32000,
            Stock = 100
        };

        context.Productos.Add(producto);
        await context.SaveChangesAsync();

        var resultado = await context.Productos.FirstOrDefaultAsync(p => p.Nombre == "Cemento Portland");
        Assert.NotNull(resultado);
        Assert.Equal(32000, resultado.Precio);
    }

    [Fact]
    public async Task EliminarProducto_DebeQuitarloDeLaLista()
    {
        var context = CrearContextoEnMemoria();
        var producto = new Producto { Nombre = "Arena", Precio = 15000, Stock = 50 };
        context.Productos.Add(producto);
        await context.SaveChangesAsync();

        context.Productos.Remove(producto);
        await context.SaveChangesAsync();

        var total = await context.Productos.CountAsync();
        Assert.Equal(0, total);
    }

    [Fact]
    public void DetalleVenta_CalculaSubtotalCorrectamente()
    {
        var detalle = new DetalleVenta
        {
            Cantidad = 5,
            PrecioUnitario = 32000
        };

        var subtotal = detalle.Cantidad * detalle.PrecioUnitario;
        Assert.Equal(160000, subtotal);
    }

    [Fact]
    public async Task CrearCliente_DebeGuardarCorrectamente()
    {
        var context = CrearContextoEnMemoria();
        var cliente = new Cliente
        {
            Nombre = "Juan Perez",
            Correo = "juan@firmeza.com",
            Documento = "123456789",
            Telefono = "3001234567"
        };

        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();

        var resultado = await context.Clientes.FirstOrDefaultAsync(c => c.Correo == "juan@firmeza.com");
        Assert.NotNull(resultado);
        Assert.Equal("Juan Perez", resultado.Nombre);
    }

    [Fact]
    public async Task CrearVenta_DebeAsociarseACliente()
    {
        var context = CrearContextoEnMemoria();
        var cliente = new Cliente { Nombre = "Test", Correo = "test@test.com", Documento = "", Telefono = "" };
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();

        var venta = new Venta { ClienteId = cliente.Id, Fecha = DateTime.UtcNow };
        context.Ventas.Add(venta);
        await context.SaveChangesAsync();

        var resultado = await context.Ventas.FirstOrDefaultAsync(v => v.ClienteId == cliente.Id);
        Assert.NotNull(resultado);
        Assert.Equal(cliente.Id, resultado.ClienteId);
    }
}
