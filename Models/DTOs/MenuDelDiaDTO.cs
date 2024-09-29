using ApiBRD.Models.Entities;

namespace ApiBRD.Models.DTOs
{
    public class MenuDelDiaDTO
    {
        public int Id { get; set; }

        public int? IdProducto { get; set; }

        public  ProductoDTO? Producto { get; set; }
    }
}
