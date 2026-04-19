using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class BookingStatus
{
    public int IdBookingStatus { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
