using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class RequestStatus
{
    public int IdStatus { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
