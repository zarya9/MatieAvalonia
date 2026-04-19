using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class Review
{
    public int IdReview { get; set; }

    public int? UserId { get; set; }

    public int? ServiceId { get; set; }

    public int? MasterId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Master? Master { get; set; }

    public virtual Service? Service { get; set; }

    public virtual User? User { get; set; }
}
