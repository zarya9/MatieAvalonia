using System;
using System.Collections.Generic;

namespace MatieAvalonia.Data;

public partial class User
{
    public int IdUser { get; set; }

    public string? Fname { get; set; }

    public string? Name { get; set; }

    public string? Patronymic { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? Balance { get; set; }

    public virtual ICollection<Booking> BookingMasters { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingUsers { get; set; } = new List<Booking>();

    public virtual ICollection<CardUser> CardUsers { get; set; } = new List<CardUser>();

    public virtual ICollection<Master> Masters { get; set; } = new List<Master>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }
}
