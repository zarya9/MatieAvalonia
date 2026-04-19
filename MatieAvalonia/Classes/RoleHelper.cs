using System;
using MatieAvalonia.Data;

namespace MatieAvalonia.Classes;

public static class RoleHelper
{
    private static bool NameContains(User? user, string fragment) =>
        !string.IsNullOrEmpty(user?.Role?.Name) &&
        user.Role.Name.Contains(fragment, StringComparison.OrdinalIgnoreCase);

    public static bool IsAdmin(User? user) =>
        NameContains(user, "админ") || NameContains(user, "admin");

    public static bool IsModerator(User? user) =>
        NameContains(user, "модератор") || NameContains(user, "moderator");

    public static bool IsMaster(User? user) =>
        NameContains(user, "мастер") || NameContains(user, "master");

    public static bool CanSeeModeratorMenu(User? user) =>
        IsModerator(user) || IsAdmin(user);

    public static bool CanSeeAdminMenu(User? user) =>
        IsAdmin(user);

    public static bool CanSeeMasterMenu(User? user) =>
        IsMaster(user) || IsAdmin(user);

    /// <summary>Клиент салона: запись, свои записи, отзыв, баланс и карты. Не администратор, не модератор и не мастер.</summary>
    public static bool IsSalonClient(User? user) =>
        user != null && !IsAdmin(user) && !IsModerator(user) && !IsMaster(user);
}
