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

public partial class BookingCreatePage : UserControl
{
    private readonly int? _presetServiceId;
    private readonly int? _presetMasterUserId;

    public BookingCreatePage()
        : this(null, null)
    {
    }

    public BookingCreatePage(int? presetServiceId, int? presetMasterUserId)
    {
        InitializeComponent();
        FormInputSanitizer.Wire(TxtTime, FormInputSanitizer.TimeHm);
        _presetServiceId = presetServiceId;
        _presetMasterUserId = presetMasterUserId;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
        {
            SetError("Запись на услугу доступна только клиентам салона (роль «Пользователь»). Администратор, модератор и мастер записывают клиентов через рабочие разделы, а не как посетитель.");
            SetBookingFormEnabled(false);
            return;
        }

        SetBookingFormEnabled(true);
        try
        {
            var services = ConnectionClass.connect.Services
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new BookingServicePickItem { IdService = s.IdService, Name = s.Name ?? "Услуга" })
                .ToList();
            CmbService.ItemsSource = services;

            var masterRows = ConnectionClass.connect.Masters
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.UserId != null)
                .OrderBy(m => m.User!.Fname)
                .ThenBy(m => m.User!.Name)
                .ToList();
            var masters = masterRows
                .Where(m => m.User != null)
                .Select(m => new BookingMasterPickItem
                {
                    MasterUserId = m.UserId!.Value,
                    Display = PersonFormat.Fio(m.User)
                })
                .ToList();
            CmbMaster.ItemsSource = masters;

            if (_presetServiceId is int sid)
            {
                var svcPick = services.FirstOrDefault(s => s.IdService == sid);
                if (svcPick != null)
                    CmbService.SelectedItem = svcPick;
            }

            if (_presetMasterUserId is int mid)
            {
                var mPick = masters.FirstOrDefault(m => m.MasterUserId == mid);
                if (mPick != null)
                    CmbMaster.SelectedItem = mPick;
            }

            if (DtVisit.SelectedDate == null)
                DtVisit.SelectedDate = DateTime.Today.AddDays(1);
        }
        catch
        {
            SetError("Не удалось загрузить списки.");
        }
    }

    private void SetBookingFormEnabled(bool enabled)
    {
        CmbService.IsEnabled = enabled;
        CmbMaster.IsEnabled = enabled;
        DtVisit.IsEnabled = enabled;
        TxtTime.IsEnabled = enabled;
        BtnSave.IsEnabled = enabled;
    }

    private void SetError(string? msg)
    {
        if (string.IsNullOrWhiteSpace(msg))
        {
            TxtBookingError.IsVisible = false;
            TxtBookingError.Text = "";
        }
        else
        {
            TxtBookingError.Text = msg.Trim();
            TxtBookingError.IsVisible = true;
        }
    }

    private static bool TryParseHm(string text, out TimeSpan timeOfDay)
    {
        timeOfDay = default;
        var parts = text.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return false;
        if (!int.TryParse(parts[0], out var h) || !int.TryParse(parts[1], out var m))
            return false;
        if (h is < 0 or > 23 || m is < 0 or > 59)
            return false;
        timeOfDay = new TimeSpan(h, m, 0);
        return true;
    }

    private void BtnCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new ServicesCatalogPage());
    }

    private void BtnSave_OnClick(object? sender, RoutedEventArgs e)
    {
        SetError(null);
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
        {
            SetError("Запись доступна только клиентам салона.");
            return;
        }

        var userId = Session.CurrentUser?.IdUser;
        if (userId == null)
        {
            SetError("Войдите в систему, чтобы создать запись.");
            return;
        }

        if (CmbService.SelectedItem is not BookingServicePickItem svc)
        {
            SetError("Выберите услугу.");
            return;
        }

        if (CmbMaster.SelectedItem is not BookingMasterPickItem mst)
        {
            SetError("Выберите мастера.");
            return;
        }

        var timeText = (TxtTime.Text ?? "").Trim();
        if (!TryParseHm(timeText, out var tod))
        {
            SetError("Введите время в формате ЧЧ:ММ (например 14:30).");
            return;
        }

        var date = (DtVisit.SelectedDate?.Date ?? DateTime.Today).Date;
        var visitWall = date + tod;
        var visitForDb = DateTime.SpecifyKind(visitWall, DateTimeKind.Unspecified);
        if (visitForDb < DateTime.Now.AddMinutes(-5))
        {
            SetError("Выберите дату и время в будущем.");
            return;
        }

        int statusId;
        try
        {
            statusId = ConnectionClass.connect.BookingStatuses
                .AsNoTracking()
                .OrderBy(s => s.IdBookingStatus)
                .Select(s => s.IdBookingStatus)
                .First();
        }
        catch
        {
            SetError("В базе нет статусов записи.");
            return;
        }

        var slot = BookingScheduleRules.SlotDuration;
        var rangeLo = visitForDb - slot;
        var rangeHi = visitForDb + slot;
        var hasConflict = ConnectionClass.connect.Bookings
            .AsNoTracking()
            .Include(b => b.Status)
            .Where(b => b.MasterId == mst.MasterUserId && b.DateTime != null)
            .Where(b => b.DateTime >= rangeLo && b.DateTime <= rangeHi)
            .Where(b => BookingScheduleRules.CountsForSchedule(b.Status))
            .AsEnumerable()
            .Any(b => BookingScheduleRules.SlotsOverlap(visitForDb, b.DateTime!.Value, slot));

        if (hasConflict)
        {
            SetError("Это время у выбранного мастера уже занято. Выберите другой слот или мастера.");
            return;
        }

        var visitDate = visitForDb.Date;
        var earlierSameDay = ConnectionClass.connect.Bookings
            .AsNoTracking()
            .Include(b => b.Status)
            .Where(b => b.MasterId == mst.MasterUserId && b.DateTime != null)
            .Where(b => BookingScheduleRules.CountsForSchedule(b.Status))
            .Count(b => b.DateTime!.Value.Date == visitDate && b.DateTime.Value < visitForDb);

        var queueNumber = BookingScheduleRules.ComputeQueueNumber(visitForDb, earlierSameDay);

        var booking = new Booking
        {
            UserId = Session.CurrentUser!.IdUser,
            MasterId = mst.MasterUserId,
            ServiceId = svc.IdService,
            DateTime = visitForDb,
            StatusId = statusId,
            TypeId = queueNumber,
            UpdatedAt = DbTimestamp.Now
        };
        try
        {
            ConnectionClass.connect.Bookings.Add(booking);
            ConnectionClass.connect.SaveChanges();
            if (TopLevel.GetTopLevel(this) is MainWindow mw)
                mw.NavigateTo(new MyBookingsPage());
        }
        catch (Exception ex)
        {
            ConnectionClass.TryDetach(booking);
            SetError(DbSaveExceptionFormatter.Format(ex, "Не удалось сохранить запись."));
        }
    }
}
