using System;
using System.Collections.Generic;

namespace ApiBRD.Models.Entities;

public partial class Menudeldia
{
    public int Id { get; set; }

    public int? IdProducto { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
