namespace ApiBRD.Models.DTOs
{
    public class ProductoIncludeDTO:ProductoDTO
    {
        public CategoriaDTO Categoria { get; set; } = null!;
    }
}
