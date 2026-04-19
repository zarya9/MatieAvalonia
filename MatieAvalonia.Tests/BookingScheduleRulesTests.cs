using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using Xunit;

namespace MatieAvalonia.Tests;

public sealed class BookingScheduleRulesTests
{
    [Theory]
    [InlineData("Новая", true)]
    [InlineData("Отмена", false)]
    [InlineData("отменено", false)]
    [InlineData("Canceled", false)]
    public void CountsForSchedule_RespectsCancellationNames(string name, bool expected)
    {
        var st = new BookingStatus { Name = name };
        Assert.Equal(expected, BookingScheduleRules.CountsForSchedule(st));
    }

    [Fact]
    public void SlotsOverlap_Adjacent_NoOverlap()
    {
        var slot = TimeSpan.FromMinutes(60);
        var a = new DateTime(2026, 4, 1, 10, 0, 0);
        var b = new DateTime(2026, 4, 1, 11, 0, 0);
        Assert.False(BookingScheduleRules.SlotsOverlap(a, b, slot));
    }

    [Fact]
    public void SlotsOverlap_SameStart_Overlaps()
    {
        var slot = TimeSpan.FromMinutes(60);
        var t = new DateTime(2026, 4, 1, 10, 0, 0);
        Assert.True(BookingScheduleRules.SlotsOverlap(t, t, slot));
    }
}
