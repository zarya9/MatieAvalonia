using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class MasterQualificationRequestPage : UserControl
{
    public MasterQualificationRequestPage()
    {
        InitializeComponent();
        Loaded += (_, _) => LoadForm();
    }

    private void LoadForm()
    {
        SetError(null);
        var uid = Session.CurrentUser?.IdUser;
        if (uid == null)
        {
            TxtCurrentQual.Text = "Войдите в систему.";
            return;
        }

        try
        {
            var master = ConnectionClass.connect.Masters
                .AsNoTracking()
                .Include(m => m.Qualif)
                .FirstOrDefault(m => m.UserId == uid);
            if (master == null)
            {
                TxtCurrentQual.Text = "Для вашей учётной записи не найдена запись мастера.";
                return;
            }

            TxtCurrentQual.Text =
                $"Текущая квалификация: {master.Qualif?.Name ?? "—"}";

            var currentId = master.QualifId;
            var quals = ConnectionClass.connect.Qualifications
                .AsNoTracking()
                .OrderBy(q => q.Name)
                .Where(q => q.IdQualif != currentId)
                .Select(q => new QualificationListItem { IdQualif = q.IdQualif, Name = q.Name ?? "" })
                .ToList();
            CmbTargetQual.ItemsSource = quals;
            if (quals.Count > 0)
                CmbTargetQual.SelectedIndex = 0;

            var last = ConnectionClass.connect.Requests
                .AsNoTracking()
                .Include(r => r.Status)
                .Where(r => r.UserId == uid)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault();
            TxtLastRequest.Text = last == null
                ? "Последняя заявка: нет"
                : $"Последняя заявка: {last.Status?.Name ?? "—"} · {last.CreatedAt?.ToString("g") ?? ""}";
        }
        catch
        {
            SetError("Не удалось загрузить данные.");
        }
    }

    private void SetError(string? msg)
    {
        if (string.IsNullOrWhiteSpace(msg))
        {
            TxtRequestError.IsVisible = false;
            TxtRequestError.Text = "";
        }
        else
        {
            TxtRequestError.Text = msg.Trim();
            TxtRequestError.IsVisible = true;
        }
    }

    private void BtnCancelRequest_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ServicesCatalogPage());
    }

    private void BtnSubmitRequest_OnClick(object? sender, RoutedEventArgs e)
    {
        SetError(null);
        var uid = Session.CurrentUser?.IdUser;
        if (uid == null)
        {
            SetError("Войдите в систему.");
            return;
        }

        if (CmbTargetQual.SelectedItem is not QualificationListItem q)
        {
            SetError("Выберите целевую квалификацию.");
            return;
        }

        try
        {
            var statusId = ConnectionClass.connect.RequestStatuses
                .AsNoTracking()
                .OrderBy(s => s.IdStatus)
                .Select(s => s.IdStatus)
                .First();

            var req = new Request
            {
                UserId = uid,
                QuialifId = q.IdQualif,
                StatusId = statusId,
                CreatedAt = DbTimestamp.Now
            };
            ConnectionClass.connect.Requests.Add(req);
            ConnectionClass.connect.SaveChanges();
            LoadForm();
        }
        catch
        {
            SetError("Не удалось отправить заявку.");
        }
    }
}
