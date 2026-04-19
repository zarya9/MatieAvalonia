using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class Request
{
    public int IdRequest { get; set; }

    public int? UserId { get; set; }

    public int? QuialifId { get; set; }

    public int? StatusId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Qualification? Quialif { get; set; }

    public virtual RequestStatus? Status { get; set; }

    public virtual User? User { get; set; }
}
