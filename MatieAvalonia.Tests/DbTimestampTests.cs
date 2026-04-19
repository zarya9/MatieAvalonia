using MatieAvalonia.Classes;
using Xunit;

namespace MatieAvalonia.Tests;

/// <summary>Unit-тесты логики метки времени последнего изменения (сохранение в PostgreSQL <c>timestamp without time zone</c>).</summary>
public sealed class DbTimestampTests
{
    [Fact]
    public void Now_Kind_IsUnspecified()
    {
        try
        {
            DbTimestamp.LocalClock = () => new DateTime(2026, 6, 15, 14, 30, 45, DateTimeKind.Local);
            var t = DbTimestamp.Now;
            Assert.Equal(DateTimeKind.Unspecified, t.Kind);
        }
        finally
        {
            DbTimestamp.UseSystemLocalClock();
        }
    }

    [Fact]
    public void Now_PreservesComponents_FromLocalClock()
    {
        try
        {
            var src = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Local);
            DbTimestamp.LocalClock = () => src;
            var t = DbTimestamp.Now;
            Assert.Equal(2024, t.Year);
            Assert.Equal(1, t.Month);
            Assert.Equal(2, t.Day);
            Assert.Equal(3, t.Hour);
            Assert.Equal(4, t.Minute);
            Assert.Equal(5, t.Second);
        }
        finally
        {
            DbTimestamp.UseSystemLocalClock();
        }
    }

    [Fact]
    public void Now_DoesNotReturnUtcKind()
    {
        try
        {
            DbTimestamp.LocalClock = () => new DateTime(2025, 5, 5, 12, 0, 0, DateTimeKind.Utc);
            var t = DbTimestamp.Now;
            Assert.NotEqual(DateTimeKind.Utc, t.Kind);
            Assert.Equal(DateTimeKind.Unspecified, t.Kind);
        }
        finally
        {
            DbTimestamp.UseSystemLocalClock();
        }
    }

    [Fact]
    public void UseSystemLocalClock_AfterOverride_UsesRealClock()
    {
        try
        {
            DbTimestamp.LocalClock = () => new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
            DbTimestamp.UseSystemLocalClock();
            var before = DateTime.Now.AddSeconds(-2);
            var t = DbTimestamp.Now;
            var after = DateTime.Now.AddSeconds(2);
            Assert.InRange(t.Ticks, before.Ticks, after.Ticks);
        }
        finally
        {
            DbTimestamp.UseSystemLocalClock();
        }
    }

    /// <summary>Метка «последнее изменение» для БД должна сохранять доли секунды (Npgsql timestamp).</summary>
    [Fact]
    public void Now_PreservesMilliseconds_ForLastModifiedStamp()
    {
        try
        {
            DbTimestamp.LocalClock = () => new DateTime(2026, 3, 10, 8, 15, 30, 123, DateTimeKind.Local);
            var t = DbTimestamp.Now;
            Assert.Equal(123, t.Millisecond);
        }
        finally
        {
            DbTimestamp.UseSystemLocalClock();
        }
    }
}
