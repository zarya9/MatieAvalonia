using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class MyBookingsPage : UserControl
{
    private const int PageSize = 10;
    private List<BookingGridRow> _rows = new();
    private int _page = 1;
    private bool _serviceAsc = true;

    public MyBookingsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => ReloadFromDb();
    }

    private void ReloadFromDb()
    {
        _rows = new List<BookingGridRow>();
        var uid = Session.CurrentUser?.IdUser;
        if (uid == null)
        {
            RenderPage();
            return;
        }

        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
        {
            _rows = new List<BookingGridRow>();
            _page = 1;
            RenderPage();
            return;
        }

        try
        {
            var list = ConnectionClass.connect.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == uid)
                .Include(b => b.Service)
                .Include(b => b.Master)
                .Include(b => b.Status)
                .OrderBy(b => b.Service!.Name)
                .ToList();
            _rows = list.Select(b => new BookingGridRow
                {
                    IdBooking = b.IdBooking,
                    ServiceId = b.ServiceId,
                    MasterUserId = b.MasterId,
                    Service = b.Service != null ? b.Service.Name ?? "—" : "—",
                    Master = PersonFormat.Fio(b.Master),
                    Client = "",
                    DateTime = b.DateTime?.ToString("g", CultureInfo.CurrentCulture) ?? "—",
                    Status = b.Status != null ? b.Status.Name ?? "—" : "—",
                    Queue = b.TypeId.HasValue ? b.TypeId.Value.ToString(CultureInfo.InvariantCulture) : "—",
                    UpdatedAt = b.UpdatedAt?.ToString("g", CultureInfo.CurrentCulture) ?? "—"
                })
                .ToList();
        }
        catch
        {
            _rows = new List<BookingGridRow>();
        }

        _page = 1;
        RenderPage();
    }

    private void RenderPage()
    {
        if (Session.CurrentUser != null && !RoleHelper.IsSalonClient(Session.CurrentUser))
        {
            BookingsGrid.ItemsSource = new List<BookingGridRow>();
            TxtPageInfo.Text = "Раздел только для клиентов салона.";
            return;
        }

        var sorted = (_serviceAsc
                ? _rows.OrderBy(r => r.Service, StringComparer.CurrentCultureIgnoreCase)
                : _rows.OrderByDescending(r => r.Service, StringComparer.CurrentCultureIgnoreCase))
            .ToList();
        var total = sorted.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        if (_page > totalPages) _page = totalPages;
        if (_page < 1) _page = 1;

        BookingsGrid.ItemsSource = sorted.Skip((_page - 1) * PageSize).Take(PageSize).ToList();

        if (total == 0)
            TxtPageInfo.Text = Session.CurrentUser == null ? "Войдите в систему" : "Нет записей";
        else
        {
            var from = (_page - 1) * PageSize + 1;
            var to = Math.Min(_page * PageSize, total);
            TxtPageInfo.Text = $"{from}–{to} из {total}";
        }
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

    private void BtnSortAsc_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _serviceAsc = true;
        _page = 1;
        RenderPage();
    }

    private void BtnSortDesc_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _serviceAsc = false;
        _page = 1;
        RenderPage();
    }

    private void BookingsGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (BookingsGrid.SelectedItem is not BookingGridRow row || row.ServiceId is not int sid)
            return;
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ServiceDetailPage(sid));
    }

    private void BtnRepeatBooking_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (BookingsGrid.SelectedItem is not BookingGridRow row)
            return;
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new BookingCreatePage(row.ServiceId, row.MasterUserId));
    }
}
