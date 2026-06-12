using AutoMapper;
using Firmeza.API.DTOs;
using Firmeza.Web.Data;
using Firmeza.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VentasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public VentasController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<VentaDto>>> GetAll()
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles).ThenInclude(d => d.Producto)
            .ToListAsync();
        return Ok(_mapper.Map<List<VentaDto>>(ventas));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VentaDto>> GetById(int id)
    {
        var venta = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (venta == null) return NotFound();
        return Ok(_mapper.Map<VentaDto>(venta));
    }

    [HttpPost]
    [Authorize(Roles = "Cliente,Admin")]
    public async Task<ActionResult<VentaDto>> Create(CrearVentaDto dto)
    {
        var venta = new Venta
        {
            ClienteId = dto.ClienteId,
            Fecha = DateTime.UtcNow,
            Detalles = new List<DetalleVenta>()
        };

        foreach (var item in dto.Detalles)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto == null) return BadRequest($"Producto {item.ProductoId} no encontrado");
            venta.Detalles.Add(new DetalleVenta
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync();

        await _context.Entry(venta).Reference(v => v.Cliente).LoadAsync();
        foreach (var d in venta.Detalles)
            await _context.Entry(d).Reference(x => x.Producto).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = venta.Id }, _mapper.Map<VentaDto>(venta));
    }
}
