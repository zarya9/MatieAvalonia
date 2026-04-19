using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class Qualification
{
    public int IdQualif { get; set; }

    public string? Name { get; set; }

    public int? Index { get; set; }

    public virtual ICollection<Master> Masters { get; set; } = new List<Master>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
