using MatieAvalonia.Data;

namespace MatieAvalonia.Classes;

public static class Session
{
    public static User? CurrentUser { get; set; }

    public static void Clear() => CurrentUser = null;
}
