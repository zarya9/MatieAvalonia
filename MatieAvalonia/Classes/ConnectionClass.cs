using System;
using MatieAvalonia.Data;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Classes;

public static class ConnectionClass
{
    public static AppDbContext connect = new();

    /// <summary>После неудачного SaveChanges сбрасывает «зависшую» Added-сущность со статического контекста.</summary>
    public static void TryDetach(object entity)
    {
        try
        {
            var entry = connect.Entry(entity);
            if (entry.State != EntityState.Detached)
                entry.State = EntityState.Detached;
        }
        catch (InvalidOperationException)
        {
            // сущность не отслеживается
        }
    }
}
