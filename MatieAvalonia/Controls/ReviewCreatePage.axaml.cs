using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class ReviewCreatePage : UserControl
{
    private readonly int? _presetServiceId;

    public ReviewCreatePage()
        : this(null)
    {
    }

    public ReviewCreatePage(int? presetServiceId)
    {
        InitializeComponent();
        FormInputSanitizer.Wire(TxtComment, s => FormInputSanitizer.ReviewComment(s, DbFieldLimits.ReviewComment));
        _presetServiceId = presetServiceId;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
        {
            SetError("Отзывы оставляют только клиенты салона (роль «Пользователь»).");
            SetReviewFormEnabled(false);
            return;
        }

        SetReviewFormEnabled(true);
        SetError(null);
        try
        {
            var services = ConnectionClass.connect.Services
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new BookingServicePickItem { IdService = s.IdService, Name = s.Name ?? "Услуга" })
                .ToList();
            CmbService.ItemsSource = services;
            if (_presetServiceId is int ps)
            {
                var svcPick = services.FirstOrDefault(s => s.IdService == ps);
                if (svcPick != null)
                    CmbService.SelectedItem = svcPick;
            }

            var masters = ConnectionClass.connect.Masters
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.User != null)
                .OrderBy(m => m.User!.Fname)
                .ToList()
                .Select(m => new MasterListItem { IdMaster = m.IdMaster, Display = PersonFormat.Fio(m.User) })
                .ToList();
            var masterItems = new List<MasterListItem>
            {
                new() { IdMaster = 0, Display = "— без мастера" }
            };
            masterItems.AddRange(masters);
            CmbMaster.ItemsSource = masterItems;
            CmbMaster.SelectedIndex = 0;

            CmbRating.ItemsSource = new[] { 1, 2, 3, 4, 5 };
            CmbRating.SelectedItem = 5;
        }
        catch
        {
            SetError("Не удалось загрузить списки.");
        }
    }

    private void SetReviewFormEnabled(bool enabled)
    {
        CmbService.IsEnabled = enabled;
        CmbMaster.IsEnabled = enabled;
        CmbRating.IsEnabled = enabled;
        TxtComment.IsEnabled = enabled;
        BtnSubmitReview.IsEnabled = enabled;
    }

    private void SetError(string? msg)
    {
        if (string.IsNullOrWhiteSpace(msg))
        {
            TxtReviewError.IsVisible = false;
            TxtReviewError.Text = "";
        }
        else
        {
            TxtReviewError.Text = msg.Trim();
            TxtReviewError.IsVisible = true;
        }
    }

    private void BtnCancelReview_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ServicesCatalogPage());
    }

    private void BtnSubmitReview_OnClick(object? sender, RoutedEventArgs e)
    {
        SetError(null);
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
        {
            SetError("Отзыв доступен только клиентам салона.");
            return;
        }

        var uid = Session.CurrentUser?.IdUser;
        if (uid == null)
        {
            SetError("Войдите в систему.");
            return;
        }

        if (CmbService.SelectedItem is not BookingServicePickItem svc)
        {
            SetError("Выберите услугу.");
            return;
        }

        if (CmbRating.SelectedItem is not int rating)
        {
            SetError("Выберите оценку.");
            return;
        }

        var comment = (TxtComment.Text ?? "").Trim();
        if (string.IsNullOrEmpty(comment))
        {
            SetError("Введите комментарий.");
            return;
        }

        int? masterPk = null;
        if (CmbMaster.SelectedItem is MasterListItem mi && mi.IdMaster > 0)
            masterPk = mi.IdMaster;

        var review = new Review
        {
            UserId = Session.CurrentUser!.IdUser,
            ServiceId = svc.IdService,
            MasterId = masterPk,
            Rating = rating,
            Comment = comment,
            CreatedAt = DbTimestamp.Now
        };
        try
        {
            ConnectionClass.connect.Reviews.Add(review);
            ConnectionClass.connect.SaveChanges();
            if (TopLevel.GetTopLevel(this) is MainWindow mw)
                mw.NavigateTo(new ServicesCatalogPage());
        }
        catch (Exception ex)
        {
            ConnectionClass.TryDetach(review);
            SetError(DbSaveExceptionFormatter.Format(ex, "Не удалось сохранить отзыв."));
        }
    }
}
