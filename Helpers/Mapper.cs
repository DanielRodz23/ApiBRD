using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiBRD.Helpers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Producto, ProductoDTO>();
            CreateMap<ProductoDTO, Producto>();

            CreateMap<Categoria, CategoriaDTO>();
            CreateMap<CategoriaDTO, Categoria>();
            CreateMap<Categoria, CategoriaIncludeDTO>()
                  .ForMember(dest => dest.Productos, opt => opt.MapFrom(src => src.Producto));


            CreateMap<Menudeldia, MenuDelDiaDTO>()
                .ForMember(dest => dest.Producto, opt => opt.MapFrom(src => src.IdProductoNavigation));
            CreateMap<MenuDelDiaDTO, Menudeldia>();


            //CreateMap<Pago, DashPago>()
            //  .ForMember(dest => dest.JugadorNavigation, opt => opt.MapFrom(src => src.IdJugadorNavigation.Nombre))
            //  .ForMember(dest => dest.ResponsableNavigation, opt => opt.MapFrom(src => src.IdResponsableNavigation.Nombre));

        }
    }
}
