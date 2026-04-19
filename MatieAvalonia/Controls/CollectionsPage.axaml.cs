using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class CollectionsPage : UserControl
{
    private const int PageSize = 10;
    private List<CollectionGridRow> _rows = new();
    private int _page = 1;
    private bool _nameAsc = true;

    public CollectionsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => ReloadFromDb();
    }

    private void ReloadFromDb()
    {
        try
        {
            _rows = ConnectionClass.connect.Collections
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CollectionGridRow
                {
                    IdCollection = c.IdCollection,
                    Name = c.Name ?? "",
                    ServicesCount = c.Services.Count
                })
                .ToList();
        }
        catch
        {
            _rows = new List<CollectionGridRow>();
        }

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

        CollectionsGrid.ItemsSource = sorted.Skip((_page - 1) * PageSize).Take(PageSize).ToList();

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

    private void CollectionsGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (CollectionsGrid.SelectedItem is not CollectionGridRow row)
            return;
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateToCatalog(row.IdCollection);
    }
}
