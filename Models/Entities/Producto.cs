using System;
using System.Collections.Generic;

namespace ApiBRD.Models.Entities;

public partial class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public int IdCategoria { get; set; }

    public bool Disponible { get; set; }

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Menudeldia> Menudeldia { get; set; } = new List<Menudeldia>();
}
