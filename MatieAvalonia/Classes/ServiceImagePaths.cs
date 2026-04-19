using System;
using System.IO;

namespace MatieAvalonia.Classes;

/// <summary>
/// Изображения услуг хранятся в папке <c>Resources</c> рядом с исполняемым файлом (копируется при сборке).
/// В БД в <see cref="Data.Service.ImgPath"/> сохраняется относительный путь вида <c>Resources\имяфайла.png</c>.
/// </summary>
public static class ServiceImagePaths
{
    public const string ResourcesFolderName = "Resources";

    public static string GetResourcesRoot()
    {
        var root = Path.Combine(AppContext.BaseDirectory, ResourcesFolderName);
        Directory.CreateDirectory(root);
        return root;
    }

    /// <summary>Убирает ошибочный префикс вида «2 » в начале пути из старых записей БД.</summary>
    private static string TrimLeadingDigitIdPrefix(string relativePath)
    {
        var i = 0;
        while (i < relativePath.Length && char.IsDigit(relativePath[i]))
            i++;
        if (i == 0)
            return relativePath;
        if (i < relativePath.Length && char.IsWhiteSpace(relativePath[i]))
            i++;
        return i < relativePath.Length ? relativePath[i..] : relativePath;
    }

    private static string? TryUnderBaseDir(string baseDir, string relative)
    {
        if (string.IsNullOrWhiteSpace(relative))
            return null;
        var norm = relative.Replace('/', Path.DirectorySeparatorChar).Trim();
        if (norm.Length == 0)
            return null;
        if (Path.IsPathRooted(norm))
            return File.Exists(norm) ? norm : null;

        var combined = Path.GetFullPath(Path.Combine(baseDir, norm));
        var baseTrim = baseDir.TrimEnd(Path.DirectorySeparatorChar);
        var ok = string.Equals(combined.TrimEnd(Path.DirectorySeparatorChar), baseTrim, StringComparison.OrdinalIgnoreCase)
                 || combined.StartsWith(baseTrim + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        if (!ok)
            return null;
        return File.Exists(combined) ? combined : null;
    }

    /// <summary>
    /// Полный путь к файлу для отображения; <paramref name="storedPath"/> — как в БД (относительный или старый абсолютный).
    /// Поддерживаются «/» и «\», лишний префикс «цифры + пробел», поиск по имени файла в <see cref="GetResourcesRoot"/>.
    /// </summary>
    public static string? ResolveFullPath(string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return null;

        var trimmed = storedPath.Trim();
        if (Path.IsPathRooted(trimmed))
            return File.Exists(trimmed) ? trimmed : null;

        var baseDir = Path.GetFullPath(AppContext.BaseDirectory);

        foreach (var candidate in new[] { trimmed, TrimLeadingDigitIdPrefix(trimmed) })
        {
            var hit = TryUnderBaseDir(baseDir, candidate);
            if (hit != null)
                return hit;
        }

        var fileName = Path.GetFileName(trimmed.Replace('/', Path.DirectorySeparatorChar));
        if (!string.IsNullOrEmpty(fileName) && fileName.IndexOf('.') > 0)
        {
            var inResources = Path.Combine(GetResourcesRoot(), fileName);
            if (File.Exists(inResources))
                return inResources;
        }

        return null;
    }

    /// <summary>
    /// Копирует выбранный пользователем файл в <c>Resources</c> и возвращает относительный путь для сохранения в БД.
    /// </summary>
    public static string CopyExternalFileIntoResources(string sourcePath)
    {
        var ext = Path.GetExtension(sourcePath);
        if (string.IsNullOrWhiteSpace(ext))
            ext = ".png";

        var destName = $"{Guid.NewGuid():N}{ext}";
        var destFull = Path.Combine(GetResourcesRoot(), destName);
        File.Copy(sourcePath, destFull, overwrite: true);
        return Path.Combine(ResourcesFolderName, destName);
    }
}
