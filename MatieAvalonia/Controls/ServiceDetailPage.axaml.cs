using System;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class ServiceDetailPage : UserControl
{
    private Bitmap? _photoBitmap;
    private readonly int? _preselectServiceId;

    public ServiceDetailPage()
        : this(null)
    {
    }

    public ServiceDetailPage(int? preselectServiceId)
    {
        InitializeComponent();
        _preselectServiceId = preselectServiceId;
        Loaded += OnLoaded;
        Unloaded += (_, _) => ClearPhoto();
    }

    private void ClearPhoto()
    {
        ImgServicePhoto.Source = null;
        _photoBitmap?.Dispose();
        _photoBitmap = null;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            var list = ConnectionClass.connect.Services
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new BookingServicePickItem { IdService = s.IdService, Name = s.Name ?? "Услуга" })
                .ToList();
            CmbPickService.ItemsSource = list;
            if (_preselectServiceId is int ps)
            {
                var found = list.FirstOrDefault(x => x.IdService == ps);
                if (found != null)
                    CmbPickService.SelectedItem = found;
                else if (list.Count > 0)
                    CmbPickService.SelectedIndex = 0;
            }
            else if (list.Count > 0)
                CmbPickService.SelectedIndex = 0;

            ApplySalonClientButtons();
        }
        catch
        {
            TxtServiceTitle.Text = "Ошибка загрузки";
        }
    }

    private void ApplySalonClientButtons()
    {
        var ok = RoleHelper.IsSalonClient(Session.CurrentUser);
        BtnBookFromDetail.IsVisible = ok;
        BtnReviewFromDetail.IsVisible = ok;
    }

    private void CmbPickService_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CmbPickService.SelectedItem is not BookingServicePickItem pick)
            return;
        try
        {
            var s = ConnectionClass.connect.Services.AsNoTracking().FirstOrDefault(x => x.IdService == pick.IdService);
            if (s == null)
                return;

            TxtServiceTitle.Text = s.Name ?? "—";
            TxtServiceDescription.Text = string.IsNullOrWhiteSpace(s.Description) ? "—" : s.Description!;
            TxtServicePrice.Text = s.Price.HasValue
                ? $"Цена: {s.Price.Value.ToString("0.##", CultureInfo.CurrentCulture)} ₽"
                : "Цена: —";
            TxtServiceUpdated.Text =
                $"Обновлено: {s.UpdatedAt?.ToString("g", CultureInfo.CurrentCulture) ?? "—"}";

            ClearPhoto();
            try
            {
                _photoBitmap = ServiceImageLoader.LoadDisplayBitmap(s.ImgPath);
                ImgServicePhoto.Source = _photoBitmap;
            }
            catch
            {
                // оставляем без фото только при сбое и самой заглушки
            }
        }
        catch
        {
            // ignore
        }
    }

    private void BtnBackDetail_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ServicesCatalogPage());
    }

    private void BtnBookFromDetail_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
            return;
        if (CmbPickService.SelectedItem is not BookingServicePickItem pick)
            return;
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new BookingCreatePage(pick.IdService, null));
    }

    private void BtnReviewFromDetail_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
            return;
        if (CmbPickService.SelectedItem is not BookingServicePickItem pick)
            return;
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ReviewCreatePage(pick.IdService));
    }
}
