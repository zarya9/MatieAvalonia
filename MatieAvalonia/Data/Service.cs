using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class Service
{
    public int IdService { get; set; }

    public int? CollectionId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ImgPath { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Collection? Collection { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
