namespace ApiBRD.Models.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int IdCategoria { get; set; }
        public bool Disponible {  get; set; }
    }
}
