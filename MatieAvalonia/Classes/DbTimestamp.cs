using System;

namespace MatieAvalonia.Classes;

/// <summary>
/// Время для колонок PostgreSQL <c>timestamp without time zone</c> и меток «последнее изменение» (<c>Updated_At</c> и т.п.):
/// Npgsql не записывает <see cref="DateTimeKind.Utc"/> (и не смешивает виды в одном сохранении).
/// Берём локальные дату/время в виде «наивного» времени — как ожидает тип без таймзоны.
/// </summary>
public static class DbTimestamp
{
    /// <summary>Локальные «текущие» часы; в unit-тестах подменяется фиксированным значением.</summary>
    public static Func<DateTime> LocalClock { get; set; } = () => DateTime.Now;

    /// <summary>Метка времени для сохранения в БД (вид <see cref="DateTimeKind.Unspecified"/>).</summary>
    public static DateTime Now => DateTime.SpecifyKind(LocalClock(), DateTimeKind.Unspecified);

    /// <summary>Сброс подмены часов после тестов.</summary>
    public static void UseSystemLocalClock() => LocalClock = () => DateTime.Now;
}
