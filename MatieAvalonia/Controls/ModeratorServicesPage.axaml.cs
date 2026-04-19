using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class ModeratorServicesPage : UserControl
{
    private const int PageSize = 10;
    private List<ServiceGridRow> _rows = new();
    private int _page = 1;
    private bool _nameAsc = true;

    public ModeratorServicesPage()
    {
        InitializeComponent();
        Loaded += (_, _) => ReloadFromDb();
    }

    private void SetModeratorMsg(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            TxtModeratorMsg.IsVisible = false;
            TxtModeratorMsg.Text = "";
        }
        else
        {
            TxtModeratorMsg.Text = text.Trim();
            TxtModeratorMsg.IsVisible = true;
        }
    }

    private void ReloadFromDb()
    {
        try
        {
            _rows = ConnectionClass.connect.Services
                .AsNoTracking()
                .Include(s => s.Collection)
                .OrderBy(s => s.Name)
                .Select(s => new ServiceGridRow
                {
                    IdService = s.IdService,
                    CollectionId = s.CollectionId,
                    Name = s.Name ?? "",
                    Description = s.Description ?? "",
                    Price = s.Price.HasValue
                        ? s.Price.Value.ToString("0.##", CultureInfo.InvariantCulture)
                        : "—",
                    Collection = s.Collection != null ? s.Collection.Name ?? "—" : "—",
                    ImgPath = s.ImgPath ?? "",
                    UpdatedAt = s.UpdatedAt.HasValue
                        ? s.UpdatedAt.Value.ToString("g", CultureInfo.CurrentCulture)
                        : "—"
                })
                .ToList();
        }
        catch
        {
            _rows = new List<ServiceGridRow>();
        }

        SetModeratorMsg(null);
        _page = 1;
        RenderPage();
    }

    private void RenderPage()
    {
        var sorted = (_nameAsc
                ? _rows.OrderBy(r => r.Name, StringComparer.CurrentCultureIgnoreCase)
                : _rows.OrderByDescending(r => r.Name, StringComparer.CurrentCultureIgnoreCase))
            .ToList();
        var total = sorted.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        if (_page > totalPages) _page = totalPages;
        if (_page < 1) _page = 1;

        ServicesGrid.ItemsSource = sorted.Skip((_page - 1) * PageSize).Take(PageSize).ToList();

        if (total == 0)
            TxtPageInfo.Text = "Нет записей";
        else
        {
            var from = (_page - 1) * PageSize + 1;
            var to = Math.Min(_page * PageSize, total);
            TxtPageInfo.Text = $"{from}–{to} из {total}";
        }
    }

    private void BtnSortAsc_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _nameAsc = true;
        _page = 1;
        RenderPage();
    }

    private void BtnSortDesc_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _nameAsc = false;
        _page = 1;
        RenderPage();
    }

    private void BtnPrevPage_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_page > 1)
        {
            _page--;
            RenderPage();
        }
    }

    private void BtnNextPage_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var totalPages = Math.Max(1, (int)Math.Ceiling(_rows.Count / (double)PageSize));
        if (_page < totalPages)
        {
            _page++;
            RenderPage();
        }
    }

    private void BtnAddService_OnClick(object? sender, RoutedEventArgs e) =>
        NavigateToServiceForm(null);

    private async void BtnDeleteService_OnClick(object? sender, RoutedEventArgs e)
    {
        SetModeratorMsg(null);
        if (ServicesGrid.SelectedItem is not ServiceGridRow row)
        {
            SetModeratorMsg("Выберите услугу в таблице.");
            return;
        }

        if (TopLevel.GetTopLevel(this) is not Window parent)
            return;

        var dlg = new ExitConfirmWindow();
        dlg.SetMessage(
            $"Удалить услугу «{row.Name}» (ID {row.IdService})? Связанные записи и отзывы будут удалены.");
        var ok = await dlg.ShowDialog<bool>(parent);
        if (!ok)
            return;

        try
        {
            var db = ConnectionClass.connect;
            var bookings = db.Bookings.Where(b => b.ServiceId == row.IdService).ToList();
            db.Bookings.RemoveRange(bookings);
            var reviews = db.Reviews.Where(r => r.ServiceId == row.IdService).ToList();
            db.Reviews.RemoveRange(reviews);
            var entity = await db.Services.FindAsync(row.IdService);
            if (entity != null)
                db.Services.Remove(entity);
            await db.SaveChangesAsync();
            ReloadFromDb();
        }
        catch
        {
            SetModeratorMsg("Не удалось удалить услугу.");
        }
    }

    private void ServicesGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (ServicesGrid.SelectedItem is ServiceGridRow row)
            NavigateToServiceForm(row.IdService);
    }

    private void NavigateToServiceForm(int? serviceId)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ModeratorServiceFormPage(serviceId));
    }
}
