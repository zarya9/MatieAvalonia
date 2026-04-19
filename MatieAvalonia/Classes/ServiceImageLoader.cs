using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace MatieAvalonia.Classes;

/// <summary>Загрузка фото услуги: файл по <see cref="ServiceImagePaths.ResolveFullPath"/> или встроенная заглушка.</summary>
public static class ServiceImageLoader
{
    private static readonly Uri PlaceholderUri = new("avares://MatieAvalonia/Assets/ServicePlaceholder.png", UriKind.Absolute);

    /// <summary>Всегда возвращает растровое изображение; при ошибке пути — встроенный placeholder.</summary>
    public static Bitmap LoadDisplayBitmap(string? imgPathFromDb)
    {
        var full = ServiceImagePaths.ResolveFullPath(imgPathFromDb);
        if (!string.IsNullOrEmpty(full) && File.Exists(full))
        {
            try
            {
                return new Bitmap(full);
            }
            catch
            {
                // повреждённый файл — заглушка
            }
        }

        using var stream = AssetLoader.Open(PlaceholderUri);
        return new Bitmap(stream);
    }
}
