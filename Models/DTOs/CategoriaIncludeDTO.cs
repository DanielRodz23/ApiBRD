namespace ApiBRD.Models.DTOs
{
    public class CategoriaIncludeDTO:CategoriaDTO
    {
        public IEnumerable< ProductoDTO>? Productos { get; set; }
    }
}
