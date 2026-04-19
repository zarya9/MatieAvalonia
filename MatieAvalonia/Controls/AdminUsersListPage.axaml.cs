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

public partial class AdminUsersListPage : UserControl
{
    private const int PageSize = 10;
    private List<UserGridRow> _rows = new();
    private int _page = 1;
    private bool _fioAsc = true;

    public AdminUsersListPage()
    {
        InitializeComponent();
        Loaded += (_, _) => ReloadFromDb();
    }

    private void ReloadFromDb()
    {
        try
        {
            var list = ConnectionClass.connect.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .OrderBy(u => u.Fname)
                .ThenBy(u => u.Name)
                .ToList();
            _rows = list.Select(u => new UserGridRow
                {
                    IdUser = u.IdUser,
                    Fio = PersonFormat.Fio(u),
                    Login = u.Login ?? "—",
                    Role = u.Role != null ? u.Role.Name ?? "—" : "—",
                    UpdatedAt = u.UpdatedAt?.ToString("g", CultureInfo.CurrentCulture) ?? "—"
                })
                .ToList();
        }
        catch
        {
            _rows = new List<UserGridRow>();
        }

        _page = 1;
        RenderPage();
    }

    private void RenderPage()
    {
        var sorted = (_fioAsc
                ? _rows.OrderBy(r => r.Fio, StringComparer.CurrentCultureIgnoreCase)
                : _rows.OrderByDescending(r => r.Fio, StringComparer.CurrentCultureIgnoreCase))
            .ToList();
        var total = sorted.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        if (_page > totalPages) _page = totalPages;
        if (_page < 1) _page = 1;

        UsersGrid.ItemsSource = sorted.Skip((_page - 1) * PageSize).Take(PageSize).ToList();

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
        _fioAsc = true;
        _page = 1;
        RenderPage();
    }

    private void BtnSortDesc_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _fioAsc = false;
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

    private void UsersGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (UsersGrid.SelectedItem is not UserGridRow row)
            return;
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new AdminUserEditPage(row.IdUser));
    }
}
