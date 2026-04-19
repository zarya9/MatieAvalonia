using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class Booking
{
    public int IdBooking { get; set; }

    public int? UserId { get; set; }

    public int? MasterId { get; set; }

    public int? ServiceId { get; set; }

    public DateTime? DateTime { get; set; }

    public int? StatusId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? TypeId { get; set; }

    public virtual User? Master { get; set; }

    public virtual Service? Service { get; set; }

    public virtual BookingStatus? Status { get; set; }

    public virtual User? User { get; set; }
}
