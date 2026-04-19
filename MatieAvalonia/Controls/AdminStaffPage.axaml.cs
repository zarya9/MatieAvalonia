using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class AdminStaffPage : UserControl
{
    public AdminStaffPage()
    {
        InitializeComponent();
        Loaded += (_, _) => ReloadAll();
    }

    private void ReloadAll()
    {
        ReloadGrid();
        ReloadCandidates();
    }

    private void ReloadGrid()
    {
        try
        {
            var list = ConnectionClass.connect.Masters
                .AsNoTracking()
                .Include(m => m.User)
                .ThenInclude(u => u!.Role)
                .Include(m => m.Qualif)
                .Where(m => m.User != null)
                .OrderBy(m => m.User!.Fname)
                .ToList();
            var rows = list.Select(m => new StaffGridRow
                {
                    IdUser = m.UserId!.Value,
                    Fio = PersonFormat.Fio(m.User),
                    Login = m.User!.Login ?? "—",
                    Role = m.User!.Role != null ? m.User.Role.Name ?? "—" : "—",
                    Qualification = m.Qualif != null ? m.Qualif.Name ?? "—" : "—"
                })
                .ToList();
            StaffGrid.ItemsSource = rows;
        }
        catch
        {
            StaffGrid.ItemsSource = Array.Empty<StaffGridRow>();
        }
    }

    private void ReloadCandidates()
    {
        try
        {
            var masterUserIds = ConnectionClass.connect.Masters.AsNoTracking()
                .Where(m => m.UserId != null)
                .Select(m => m.UserId!.Value)
                .ToHashSet();
            var candidates = ConnectionClass.connect.Users
                .AsNoTracking()
                .Where(u => !masterUserIds.Contains(u.IdUser))
                .OrderBy(u => u.Fname)
                .Select(u => new UserPickItem
                {
                    IdUser = u.IdUser,
                    Display = $"{PersonFormat.Fio(u)} · {u.Login}"
                })
                .ToList();
            CmbStaffCandidate.ItemsSource = candidates;
            if (candidates.Count > 0)
                CmbStaffCandidate.SelectedIndex = 0;
        }
        catch
        {
            CmbStaffCandidate.ItemsSource = null;
        }
    }

    private void BtnAddMasterStaff_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CmbStaffCandidate.SelectedItem is not UserPickItem pick)
            return;

        try
        {
            var defaultQual = ConnectionClass.connect.Qualifications.AsNoTracking()
                .OrderBy(q => q.IdQualif)
                .Select(q => q.IdQualif)
                .First();

            if (ConnectionClass.connect.Masters.Any(m => m.UserId == pick.IdUser))
                return;

            ConnectionClass.connect.Masters.Add(new Master
            {
                UserId = pick.IdUser,
                QualifId = defaultQual
            });
            ConnectionClass.connect.SaveChanges();
            ReloadAll();
        }
        catch
        {
            // ignore
        }
    }
}
