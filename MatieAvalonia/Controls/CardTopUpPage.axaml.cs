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

public partial class CardTopUpPage : UserControl
{
    private bool _suppressCardCombo;

    public CardTopUpPage()
    {
        InitializeComponent();
        FormInputSanitizer.Wire(TxtNumber, FormInputSanitizer.CardNumberSpaced);
        FormInputSanitizer.Wire(TxtExpiry, FormInputSanitizer.CardExpiry);
        FormInputSanitizer.Wire(TxtCvv, s => FormInputSanitizer.CardDigits(s, DbFieldLimits.CardCvv));
        FormInputSanitizer.Wire(TxtAmount, FormInputSanitizer.BalanceMoney202);
        Loaded += (_, _) => RefreshCardPicker();
    }

    private int GetSelectedCardId() =>
        CmbCard.SelectedItem is CardPickListItem li ? li.IdCard : 0;

    private void RefreshCardPicker()
    {
        SetError(null);
        var uid = Session.CurrentUser?.IdUser;
        if (uid == null)
        {
            CmbCard.ItemsSource = null;
            PanelNewCardDetails.IsVisible = true;
            BtnSave.Content = "Сохранить карту";
            return;
        }

        try
        {
            var cards = ConnectionClass.connect.CardUsers
                .AsNoTracking()
                .Where(c => c.UserId == uid)
                .OrderByDescending(c => c.IdCard)
                .ToList();

            var items = new List<CardPickListItem>();
            foreach (var c in cards)
            {
                var num = c.NumberCard ?? "";
                var tail = num.Length > 4 ? num[^4..] : num;
                var exp = c.DateCard?.ToString("MM/yyyy", CultureInfo.InvariantCulture) ?? "—";
                items.Add(new CardPickListItem { IdCard = c.IdCard, Display = $"•••• {tail} · до {exp}" });
            }

            items.Add(new CardPickListItem { IdCard = 0, Display = "Привязать новую карту…" });
            CmbCard.ItemsSource = items;
            _suppressCardCombo = true;
            CmbCard.SelectedItem = cards.Count > 0 ? items[0] : items[^1];
            _suppressCardCombo = false;
            ApplyCardModeUi();
        }
        catch
        {
            CmbCard.ItemsSource = null;
            PanelNewCardDetails.IsVisible = true;
            BtnSave.Content = "Сохранить карту";
        }
    }

    private void CmbCard_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressCardCombo)
            return;
        ApplyCardModeUi();
    }

    private void ApplyCardModeUi()
    {
        var isNew = GetSelectedCardId() == 0;
        PanelNewCardDetails.IsVisible = isNew;
        BtnSave.Content = isNew ? "Сохранить карту" : "Пополнить баланс";
    }

    private void SetError(string? msg)
    {
        if (string.IsNullOrWhiteSpace(msg))
        {
            TxtCardError.IsVisible = false;
            TxtCardError.Text = "";
        }
        else
        {
            TxtCardError.Text = msg.Trim();
            TxtCardError.IsVisible = true;
        }
    }

    private void BtnCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new UserBalancePage());
    }

    private void BtnSave_OnClick(object? sender, RoutedEventArgs e)
    {
        SetError(null);
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
        {
            SetError("Пополнение доступно только клиентам салона.");
            return;
        }

        var uid = Session.CurrentUser?.IdUser;
        if (uid == null)
        {
            SetError("Войдите в систему.");
            return;
        }

        if (!FormInputSanitizer.TryParseBalanceTopUp((TxtAmount.Text ?? "").Trim(), out var topUp))
        {
            SetError($"Сумма от 0 до {DbFieldLimits.BalanceMonetaryMax.ToString("N2", CultureInfo.CurrentCulture)} (до двух знаков после запятой).");
            return;
        }

        var user = ConnectionClass.connect.Users.FirstOrDefault(u => u.IdUser == uid.Value);
        if (user == null)
        {
            SetError("Пользователь не найден.");
            return;
        }

        var balanceBefore = user.Balance ?? 0m;
        var newBalance = balanceBefore + topUp;
        if (newBalance > DbFieldLimits.BalanceMonetaryMax)
        {
            SetError("После пополнения баланс превысит допустимый максимум. Уменьшите сумму.");
            return;
        }

        var selectedId = GetSelectedCardId();
        CardUser? cardToAdd = null;

        if (selectedId != 0)
        {
            if (topUp <= 0)
            {
                SetError("Укажите сумму пополнения.");
                return;
            }

            if (!ConnectionClass.connect.CardUsers.AsNoTracking().Any(c => c.IdCard == selectedId && c.UserId == uid))
            {
                SetError("Выбранная карта не найдена. Обновите страницу.");
                return;
            }

            try
            {
                user.Balance = newBalance;
                user.UpdatedAt = DbTimestamp.Now;
                ConnectionClass.connect.SaveChanges();
                if (Session.CurrentUser != null && Session.CurrentUser.IdUser == user.IdUser)
                    Session.CurrentUser.Balance = newBalance;
                if (TopLevel.GetTopLevel(this) is MainWindow mw)
                {
                    mw.ShellHeader.RefreshSessionLabel();
                    mw.NavigateTo(new UserBalancePage());
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ConnectionClass.connect.Entry(user).Reload();
                }
                catch
                {
                    // игнорируем
                }

                SetError(DbSaveExceptionFormatter.Format(ex, "Не удалось пополнить баланс."));
            }

            return;
        }

        var digits = new string((TxtNumber.Text ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length is < 16 or > 19)
        {
            SetError("Номер карты: от 16 до 19 цифр.");
            return;
        }

        var exp = (TxtExpiry.Text ?? "").Trim();
        if (exp.Length != 5 || exp[2] != '/')
        {
            SetError("Срок укажите как ММ/ГГ.");
            return;
        }

        if (!int.TryParse(exp[..2], NumberStyles.None, CultureInfo.InvariantCulture, out var month)
            || !int.TryParse(exp[3..], NumberStyles.None, CultureInfo.InvariantCulture, out var year2)
            || month is < 1 or > 12)
        {
            SetError("Некорректный срок.");
            return;
        }

        var year = 2000 + year2;
        var lastDay = DateTime.DaysInMonth(year, month);
        var dateCard = new DateTime(year, month, lastDay, 23, 59, 59, DateTimeKind.Unspecified);

        var cvv = (TxtCvv.Text ?? "").Trim();
        if (cvv.Length != 3 || !cvv.All(char.IsDigit))
        {
            SetError("CVV — 3 цифры.");
            return;
        }

        cardToAdd = new CardUser
        {
            UserId = Session.CurrentUser!.IdUser,
            NumberCard = digits,
            DateCard = dateCard,
            Cvv = cvv,
            IsPriority = false
        };
        try
        {
            user.Balance = newBalance;
            user.UpdatedAt = DbTimestamp.Now;
            ConnectionClass.connect.CardUsers.Add(cardToAdd);
            ConnectionClass.connect.SaveChanges();
            if (Session.CurrentUser != null && Session.CurrentUser.IdUser == user.IdUser)
                Session.CurrentUser.Balance = newBalance;
            if (TopLevel.GetTopLevel(this) is MainWindow mw)
            {
                mw.ShellHeader.RefreshSessionLabel();
                mw.NavigateTo(new UserBalancePage());
            }
        }
        catch (Exception ex)
        {
            if (cardToAdd != null)
                ConnectionClass.TryDetach(cardToAdd);
            try
            {
                ConnectionClass.connect.Entry(user).Reload();
            }
            catch
            {
                // игнорируем, если сущность уже отсоединена
            }

            SetError(DbSaveExceptionFormatter.Format(ex, "Не удалось сохранить карту."));
        }
    }
}
