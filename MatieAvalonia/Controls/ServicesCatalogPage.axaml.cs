using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class ServicesCatalogPage : UserControl
{
    private const int PageSize = 3;
    private List<ServiceGridRow> _all = new();
    private int _page = 1;
    private bool _nameAsc = true;
    private string _search = "";
    private int? _collectionId;
    private CatalogLineFilter _line = CatalogLineFilter.All;
    private MainWindow? _shell;
    private readonly int? _presetCollectionId;
    private string _paginationSummary = "—";

    public ServicesCatalogPage()
        : this(null)
    {
    }

    public ServicesCatalogPage(int? presetCollectionId)
    {
        InitializeComponent();
        _presetCollectionId = presetCollectionId;
        Loaded += OnLoaded;
    }

    public void BindShell(MainWindow shell)
    {
        _shell = shell;
    }

    public CatalogLineFilter GetLineFilter() => _line;

    public void SetLineFilter(CatalogLineFilter line)
    {
        _line = line;
        _page = 1;
        RenderPage();
    }

    public void SetSearch(string? text)
    {
        _search = text ?? "";
        _page = 1;
        RenderPage();
    }

    public void SetCollectionFilter(int? collectionId)
    {
        _collectionId = collectionId is 0 or null ? null : collectionId;
        _page = 1;
        RenderPage();
    }

    public void ChromePrevPage()
    {
        if (_page > 1)
        {
            _page--;
            RenderPage();
        }
    }

    public void ChromeNextPage()
    {
        var filtered = BuildFilteredSorted();
        var totalPages = Math.Max(1, (int)Math.Ceiling(filtered.Count / (double)PageSize));
        if (_page < totalPages)
        {
            _page++;
            RenderPage();
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e) => ReloadFromDb();

    private void ReloadFromDb()
    {
        try
        {
            _all = ConnectionClass.connect.Services
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
            _all = new List<ServiceGridRow>();
        }

        if (_shell != null)
        {
            _search = _shell.GetCatalogSearchText();
            if (_presetCollectionId is not > 0)
                _collectionId = _shell.GetCatalogCollectionId();
        }

        if (_presetCollectionId is > 0)
            _collectionId = _presetCollectionId;

        _page = 1;
        RenderPage();
    }

    private List<ServiceGridRow> BuildFilteredSorted()
    {
        IEnumerable<ServiceGridRow> q = _all;

        if (!string.IsNullOrWhiteSpace(_search))
        {
            var s = _search.Trim();
            q = q.Where(r =>
                (r.Name ?? "").Contains(s, StringComparison.OrdinalIgnoreCase)
                || (r.Description ?? "").Contains(s, StringComparison.OrdinalIgnoreCase)
                || (r.Collection ?? "").Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        if (_collectionId is > 0)
            q = q.Where(r => r.CollectionId == _collectionId);

        switch (_line)
        {
            case CatalogLineFilter.Custom:
                q = q.Where(r => (r.Collection ?? "").Contains("кастом", StringComparison.OrdinalIgnoreCase));
                break;
            case CatalogLineFilter.Cosplay:
                q = q.Where(r =>
                    (r.Collection ?? "").Contains("косплей", StringComparison.OrdinalIgnoreCase)
                    || (r.Collection ?? "").Contains("cosplay", StringComparison.OrdinalIgnoreCase));
                break;
        }

        return (_nameAsc
                ? q.OrderBy(r => r.Name, StringComparer.CurrentCultureIgnoreCase)
                : q.OrderByDescending(r => r.Name, StringComparer.CurrentCultureIgnoreCase))
            .ToList();
    }

    private void RenderPage()
    {
        var sorted = BuildFilteredSorted();
        var total = sorted.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        if (_page > totalPages) _page = totalPages;
        if (_page < 1) _page = 1;

        var slice = sorted.Skip((_page - 1) * PageSize).Take(PageSize).ToList();
        CatalogItems.ItemsSource = slice;

        if (total == 0)
            _paginationSummary = "Нет записей";
        else
        {
            var from = (_page - 1) * PageSize + 1;
            var to = Math.Min(_page * PageSize, total);
            _paginationSummary = $"{from}–{to} из {total}";
        }

        _shell?.SyncCatalogChrome(this);
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

    public string PaginationSummary => _paginationSummary;
}
