using System;

namespace MatieAvalonia.Classes;

/// <summary>Сообщения об ошибках сохранения в БД без «глухого» catch.</summary>
public static class DbSaveExceptionFormatter
{
    public static string Format(Exception ex, string prefix)
    {
        var cur = ex;
        while (cur.InnerException != null)
            cur = cur.InnerException;

        var msg = (cur.Message ?? "").Trim();
        if (string.IsNullOrEmpty(msg))
            msg = (ex.Message ?? "").Trim();
        if (string.IsNullOrEmpty(msg))
            return prefix.EndsWith('.') ? prefix : prefix + ".";

        if (msg.Length > 220)
            msg = msg[..220] + "…";

        var p = prefix.TrimEnd();
        return p.EndsWith(':') ? $"{p} {msg}" : $"{p}: {msg}";
    }
}
