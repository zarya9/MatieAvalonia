using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class CardUser
{
    public int IdCard { get; set; }

    public int? UserId { get; set; }

    public string? NumberCard { get; set; }

    public DateTime? DateCard { get; set; }

    public string? Cvv { get; set; }

    public bool? IsPriority { get; set; }

    public virtual User? User { get; set; }
}
