using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class ModeratorMasterQualificationPage : UserControl
{
    public ModeratorMasterQualificationPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            var masters = ConnectionClass.connect.Masters
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.User != null)
                .OrderBy(m => m.User!.Fname)
                .ToList()
                .Select(m => new MasterListItem { IdMaster = m.IdMaster, Display = PersonFormat.Fio(m.User) })
                .ToList();
            CmbModMaster.ItemsSource = masters;
            if (masters.Count > 0)
                CmbModMaster.SelectedIndex = 0;

            var quals = ConnectionClass.connect.Qualifications
                .AsNoTracking()
                .OrderBy(q => q.Name)
                .Select(q => new QualificationListItem { IdQualif = q.IdQualif, Name = q.Name ?? "" })
                .ToList();
            CmbModQual.ItemsSource = quals;
            if (quals.Count > 0)
                CmbModQual.SelectedIndex = 0;
        }
        catch
        {
            SetError("Не удалось загрузить списки.");
        }
    }

    private void SetError(string? msg)
    {
        if (string.IsNullOrWhiteSpace(msg))
        {
            TxtModQualError.IsVisible = false;
            TxtModQualError.Text = "";
        }
        else
        {
            TxtModQualError.Text = msg.Trim();
            TxtModQualError.IsVisible = true;
        }
    }

    private void BtnApplyQual_OnClick(object? sender, RoutedEventArgs e)
    {
        SetError(null);
        if (CmbModMaster.SelectedItem is not MasterListItem m)
        {
            SetError("Выберите мастера.");
            return;
        }

        if (CmbModQual.SelectedItem is not QualificationListItem q)
        {
            SetError("Выберите квалификацию.");
            return;
        }

        try
        {
            var entity = ConnectionClass.connect.Masters.Find(m.IdMaster);
            if (entity == null)
            {
                SetError("Мастер не найден.");
                return;
            }

            entity.QualifId = q.IdQualif;
            ConnectionClass.connect.SaveChanges();
            SetError(null);
        }
        catch
        {
            SetError("Не удалось сохранить.");
        }
    }
}
