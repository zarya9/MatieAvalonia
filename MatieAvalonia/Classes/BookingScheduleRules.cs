using System;
using MatieAvalonia.Data;

namespace MatieAvalonia.Classes;

/// <summary>Правила занятости мастера и номера очереди на день (длительность слота без отдельного поля в БД).</summary>
public static class BookingScheduleRules
{
    /// <summary>Длительность приёма по умолчанию для проверки пересечений (мин.), пока у услуги нет своей длительности.</summary>
    public const int SlotMinutes = 60;

    /// <summary>Запись участвует в занятости и в очереди, если статус не похож на отмену.</summary>
    public static bool CountsForSchedule(BookingStatus? status)
    {
        if (status?.Name == null)
            return true;
        var n = status.Name.Trim();
        if (n.Length == 0)
            return true;
        if (n.Contains("отмен", StringComparison.OrdinalIgnoreCase))
            return false;
        if (n.Contains("cancel", StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
    }

    public static TimeSpan SlotDuration => TimeSpan.FromMinutes(SlotMinutes);

    /// <summary>Пересечение интервалов [a, a+slot) и [b, b+slot).</summary>
    public static bool SlotsOverlap(DateTime aStart, DateTime bStart, TimeSpan slot)
    {
        var aEnd = aStart + slot;
        var bEnd = bStart + slot;
        return aStart < bEnd && bStart < aEnd;
    }

    /// <summary>Номер в очереди на календарный день мастера: 1 + число активных записей с тем же днём, раньше по времени.</summary>
    public static int ComputeQueueNumber(DateTime visitStart, int earlierSameDayActiveCount)
        => 1 + earlierSameDayActiveCount;
}
