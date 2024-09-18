using System;
using System.Collections.Generic;

namespace ApiBRD.Models.Entities;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
}
