using System;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Models;

namespace MatieAvalonia.Controls;

public partial class CatalogServiceCard : UserControl
{
    private Bitmap? _bitmap;

    public CatalogServiceCard()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => ApplyRow();
        Unloaded += (_, _) => ClearImage();
    }

    private void ClearImage()
    {
        ImgPhoto.Source = null;
        _bitmap?.Dispose();
        _bitmap = null;
    }

    private void ApplyRow()
    {
        ClearImage();
        ImgPhoto.IsVisible = false;
        TxtPhotoPlaceholder.IsVisible = true;

        if (DataContext is not ServiceGridRow row)
            return;

        TxtName.Text = string.IsNullOrWhiteSpace(row.Name) ? "—" : row.Name;
        TxtDesc.Text = string.IsNullOrWhiteSpace(row.Description) ? "—" : row.Description;
        TxtMeta.Text = $"Цена: {row.Price} · {row.Collection}";
        TxtUpdated.Text = $"Обновлено: {row.UpdatedAt}";

        try
        {
            _bitmap = ServiceImageLoader.LoadDisplayBitmap(row.ImgPath);
            ImgPhoto.Source = _bitmap;
            ImgPhoto.IsVisible = true;
            TxtPhotoPlaceholder.IsVisible = false;
        }
        catch
        {
            ClearImage();
        }
    }

    private void Border_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not ServiceGridRow row)
            return;
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ServiceDetailPage(row.IdService));
    }
}
