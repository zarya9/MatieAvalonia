using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class ModeratorMasterServiceBindingsPage : UserControl
{
    public ModeratorMasterServiceBindingsPage()
    {
        InitializeComponent();
        Loaded += (_, _) => Reload();
    }

    private void Reload()
    {
        try
        {
            var rows = ConnectionClass.connect.Bookings
                .AsNoTracking()
                .Where(b => b.MasterId != null && b.ServiceId != null)
                .Include(b => b.Master)
                .Include(b => b.Service)
                .AsEnumerable()
                .GroupBy(b => new { M = b.MasterId!.Value, S = b.ServiceId!.Value })
                .Select(g =>
                {
                    var first = g.First();
                    return new MasterServiceBindingRow
                    {
                        Master = PersonFormat.Fio(first.Master),
                        Service = first.Service?.Name ?? "—",
                        BookingsCount = g.Count()
                    };
                })
                .OrderBy(r => r.Master)
                .ThenBy(r => r.Service)
                .ToList();
            BindingsGrid.ItemsSource = rows;
        }
        catch
        {
            BindingsGrid.ItemsSource = new List<MasterServiceBindingRow>();
        }
    }
}
