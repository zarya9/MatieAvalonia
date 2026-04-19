using System.Linq;
using MatieAvalonia.Data;

namespace MatieAvalonia.Models;

public static class PersonFormat
{
    public static string Fio(User? u)
    {
        if (u == null) return "—";
        var parts = new[] { u.Fname, u.Name, u.Patronymic }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!.Trim());
        var s = string.Join(" ", parts);
        return string.IsNullOrEmpty(s) ? "—" : s;
    }
}

public sealed class ServiceGridRow
{
    public int IdService { get; set; }
    public int? CollectionId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Price { get; set; } = "";
    public string Collection { get; set; } = "";
    public string ImgPath { get; set; } = "";
    public string UpdatedAt { get; set; } = "";
}

public sealed class BookingGridRow
{
    public int IdBooking { get; set; }
    public int? ServiceId { get; set; }
    public int? MasterUserId { get; set; }
    public string Service { get; set; } = "";
    public string Master { get; set; } = "";
    public string Client { get; set; } = "";
    public string DateTime { get; set; } = "";
    public string Status { get; set; } = "";
    public string Queue { get; set; } = "";
    public string UpdatedAt { get; set; } = "";
}

public sealed class UserGridRow
{
    public int IdUser { get; set; }
    public string Fio { get; set; } = "";
    public string Login { get; set; } = "";
    public string Role { get; set; } = "";
    public string UpdatedAt { get; set; } = "";
}

public sealed class CollectionGridRow
{
    public int IdCollection { get; set; }
    public string Name { get; set; } = "";
    public int ServicesCount { get; set; }
}

public sealed class MasterServiceBindingRow
{
    public string Master { get; set; } = "";
    public string Service { get; set; } = "";
    public int BookingsCount { get; set; }
}

public sealed class StaffGridRow
{
    public int IdUser { get; set; }
    public string Fio { get; set; } = "";
    public string Login { get; set; } = "";
    public string Role { get; set; } = "";
    public string Qualification { get; set; } = "";
}
