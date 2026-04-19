using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class ModeratorServiceFormPage : UserControl
{
    private readonly int? _editServiceId;
    private string? _imgPath;
    private Bitmap? _previewBitmap;

    public ModeratorServiceFormPage()
        : this(null)
    {
    }

    public ModeratorServiceFormPage(int? editServiceId)
    {
        _editServiceId = editServiceId;
        InitializeComponent();
        FormInputSanitizer.Wire(TxtName, s => FormInputSanitizer.PlainLine(s, DbFieldLimits.ServiceName));
        FormInputSanitizer.Wire(TxtDescription, s => FormInputSanitizer.PlainLine(s, DbFieldLimits.ServiceDescription));
        FormInputSanitizer.Wire(TxtPrice, FormInputSanitizer.PriceMoney72);
        Loaded += OnLoaded;
        Unloaded += (_, _) => ClearPreview();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        LoadCollections();
        if (_editServiceId.HasValue)
        {
            TxtTitle.Text = "Услуга — редактирование";
            LoadService(_editServiceId.Value);
        }
        else
        {
            TxtUpdatedAt.Text = "Обновлено: —";
        }
    }

    private void LoadCollections()
    {
        try
        {
            var items = ConnectionClass.connect.Collections
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CollectionListItem { IdCollection = c.IdCollection, Name = c.Name ?? "" })
                .ToList();
            CmbCollection.ItemsSource = items;
            if (items.Count == 1)
                CmbCollection.SelectedIndex = 0;
        }
        catch
        {
            SetFormError("Не удалось загрузить коллекции.");
        }
    }

    private void LoadService(int id)
    {
        try
        {
            var s = ConnectionClass.connect.Services.AsNoTracking().FirstOrDefault(x => x.IdService == id);
            if (s == null)
            {
                SetFormError("Услуга не найдена.");
                return;
            }

            TxtName.Text = s.Name ?? "";
            TxtDescription.Text = s.Description ?? "";
            TxtPrice.Text = s.Price.HasValue
                ? s.Price.Value.ToString("0.##", CultureInfo.CurrentCulture)
                : "";
            _imgPath = s.ImgPath;
            ApplyPreviewFromPath();
            TxtUpdatedAt.Text = $"Обновлено: {s.UpdatedAt?.ToString("g", CultureInfo.CurrentCulture) ?? "—"}";

            if (CmbCollection.ItemsSource is System.Collections.IEnumerable list)
            {
                foreach (var o in list)
                {
                    if (o is CollectionListItem li && li.IdCollection == s.CollectionId)
                    {
                        CmbCollection.SelectedItem = o;
                        break;
                    }
                }
            }
        }
        catch
        {
            SetFormError("Ошибка при загрузке услуги.");
        }
    }

    private void SetFormError(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            TxtFormError.Text = "";
            TxtFormError.IsVisible = false;
        }
        else
        {
            TxtFormError.Text = message.Trim();
            TxtFormError.IsVisible = true;
        }
    }

    private void ClearPreview()
    {
        ImgPreview.Source = null;
        _previewBitmap?.Dispose();
        _previewBitmap = null;
    }

    private void ApplyPreviewFromPath()
    {
        ClearPreview();
        try
        {
            _previewBitmap = ServiceImageLoader.LoadDisplayBitmap(_imgPath);
            ImgPreview.Source = _previewBitmap;
            ImgPreview.IsVisible = true;
            TxtPreviewPlaceholder.IsVisible = false;
        }
        catch
        {
            ImgPreview.IsVisible = false;
            TxtPreviewPlaceholder.IsVisible = true;
        }
    }

    private async void BtnPickImage_OnClick(object? sender, RoutedEventArgs e)
    {
        var top = TopLevel.GetTopLevel(this);
        if (top?.StorageProvider is not { } sp)
            return;

        var files = await sp.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите изображение",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Изображения")
                {
                    Patterns = ["*.png", "*.jpg", "*.jpeg", "*.webp", "*.bmp"]
                }
            ]
        });

        if (files.Count == 0)
            return;

        var path = files[0].TryGetLocalPath();
        if (string.IsNullOrEmpty(path))
        {
            SetFormError("Не удалось получить локальный путь к файлу.");
            return;
        }

        if (!File.Exists(path))
        {
            SetFormError("Файл не найден.");
            return;
        }

        try
        {
            _imgPath = ServiceImagePaths.CopyExternalFileIntoResources(path);
        }
        catch
        {
            SetFormError("Не удалось сохранить файл в папку Resources.");
            return;
        }

        SetFormError(null);
        ApplyPreviewFromPath();
    }

    private void BtnSave_OnClick(object? sender, RoutedEventArgs e)
    {
        SetFormError(null);
        var name = (TxtName.Text ?? "").Trim();
        var description = (TxtDescription.Text ?? "").Trim();
        if (string.IsNullOrEmpty(name))
        {
            SetFormError("Укажите название услуги.");
            return;
        }

        if (name.Length > 50 || description.Length > 50)
        {
            SetFormError("Название и описание — не более 50 символов.");
            return;
        }

        if (CmbCollection.SelectedItem is not CollectionListItem col)
        {
            SetFormError("Выберите коллекцию.");
            return;
        }

        var priceText = (TxtPrice.Text ?? "").Trim();
        if (!FormInputSanitizer.TryParsePriceMoney72(priceText, out var price))
        {
            SetFormError($"Введите цену от 0 до {DbFieldLimits.ServicePriceMax.ToString("0.##", CultureInfo.CurrentCulture)} (до двух знаков после запятой).");
            return;
        }

        string? imgForDb = null;
        if (!string.IsNullOrWhiteSpace(_imgPath))
        {
            var t = _imgPath.Trim();
            if (Path.IsPathRooted(t))
            {
                try
                {
                    imgForDb = ServiceImagePaths.CopyExternalFileIntoResources(t);
                }
                catch (Exception ex)
                {
                    SetFormError(DbSaveExceptionFormatter.Format(ex, "Не удалось скопировать изображение."));
                    return;
                }
            }
            else
                imgForDb = t;
        }

        Service? addedEntity = null;
        Service? editEntity = null;
        try
        {
            if (_editServiceId.HasValue)
            {
                editEntity = ConnectionClass.connect.Services.Find(_editServiceId.Value);
                if (editEntity == null)
                {
                    SetFormError("Услуга не найдена.");
                    return;
                }

                editEntity.Name = name;
                editEntity.Description = string.IsNullOrEmpty(description) ? null : description;
                editEntity.Price = price;
                editEntity.CollectionId = col.IdCollection;
                editEntity.ImgPath = imgForDb;
                editEntity.UpdatedAt = DbTimestamp.Now;
            }
            else
            {
                addedEntity = new Service
                {
                    Name = name,
                    Description = string.IsNullOrEmpty(description) ? null : description,
                    Price = price,
                    CollectionId = col.IdCollection,
                    ImgPath = imgForDb,
                    UpdatedAt = DbTimestamp.Now
                };
                ConnectionClass.connect.Services.Add(addedEntity);
            }

            ConnectionClass.connect.SaveChanges();
            GoBackToServiceList();
        }
        catch (Exception ex)
        {
            if (addedEntity != null)
                ConnectionClass.TryDetach(addedEntity);
            if (editEntity != null)
            {
                try
                {
                    ConnectionClass.connect.Entry(editEntity).Reload();
                }
                catch
                {
                    // услуга удалена в БД или контекст в неконсистентном состоянии
                }
            }

            SetFormError(DbSaveExceptionFormatter.Format(ex, "Не удалось сохранить."));
        }
    }

    private void BtnCancel_OnClick(object? sender, RoutedEventArgs e) => GoBackToServiceList();

    private void GoBackToServiceList()
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ModeratorServicesPage());
    }
}
