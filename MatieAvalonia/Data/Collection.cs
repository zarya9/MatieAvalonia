using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class Collection
{
    public int IdCollection { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
