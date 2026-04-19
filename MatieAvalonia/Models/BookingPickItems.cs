namespace MatieAvalonia.Models;

public sealed class BookingServicePickItem
{
    public int IdService { get; init; }
    public string Name { get; init; } = "";
}

public sealed class BookingMasterPickItem
{
    /// <summary>ID пользователя-мастера (как в <c>Booking.MasterId</c>).</summary>
    public int MasterUserId { get; init; }
    public string Display { get; init; } = "";
}

public sealed class MasterListItem
{
    public int IdMaster { get; init; }
    public string Display { get; init; } = "";
}

public sealed class UserPickItem
{
    public int IdUser { get; init; }
    public string Display { get; init; } = "";
}

public sealed class RoleListItem
{
    public int IdRole { get; init; }
    public string Name { get; init; } = "";
}

public sealed class QualificationListItem
{
    public int IdQualif { get; init; }
    public string Name { get; init; } = "";
}
