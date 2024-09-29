using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using AutoMapper;

namespace ApiBRD.Helpers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Producto, ProductoDTO>();
            CreateMap<ProductoDTO, Producto>();

        }
    }
}
