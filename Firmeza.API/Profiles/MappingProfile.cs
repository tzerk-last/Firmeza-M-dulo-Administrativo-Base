using AutoMapper;
using Firmeza.API.DTOs;
using Firmeza.Web.Models;

namespace Firmeza.API.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Producto, ProductoDto>();
        CreateMap<CrearProductoDto, Producto>();

        CreateMap<Cliente, ClienteDto>();
        CreateMap<CrearClienteDto, Cliente>();

        CreateMap<Venta, VentaDto>()
            .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente.Nombre));

        CreateMap<DetalleVenta, DetalleVentaDto>()
            .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre));
    }
}
