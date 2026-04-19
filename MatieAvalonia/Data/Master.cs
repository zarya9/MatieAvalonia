using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class Master
{
    public int IdMaster { get; set; }

    public int? UserId { get; set; }

    public int? QualifId { get; set; }

    public virtual Qualification? Qualif { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User? User { get; set; }
}
