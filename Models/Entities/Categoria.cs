﻿using System;
using System.Collections.Generic;

namespace ApiBRD.Models.Entities;

public partial class Categoria
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}