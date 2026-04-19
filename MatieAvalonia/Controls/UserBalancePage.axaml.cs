using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatieAvalonia;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class UserBalancePage : UserControl
{
    public UserBalancePage()
    {
        InitializeComponent();
        Loaded += (_, _) => Reload();
    }

    private void Reload()
    {
        var uid = Session.CurrentUser?.IdUser;
        if (uid == null)
        {
            TxtBalance.Text = "—";
            TxtCardsHint.Text = "Войдите в систему.";
            CardsList.ItemsSource = null;
            BtnTopUp.IsVisible = false;
            return;
        }

        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
        {
            TxtBalance.Text = "—";
            TxtCardsHint.Text = "Баланс и карты доступны только клиентам салона.";
            CardsList.ItemsSource = null;
            BtnTopUp.IsVisible = false;
            return;
        }

        BtnTopUp.IsVisible = true;
        try
        {
            var balance = ConnectionClass.connect.Users
                .AsNoTracking()
                .Where(u => u.IdUser == uid)
                .Select(u => u.Balance)
                .FirstOrDefault();
            TxtBalance.Text = (balance ?? 0m).ToString("N2", System.Globalization.CultureInfo.CurrentCulture);

            var cards = ConnectionClass.connect.CardUsers
                .AsNoTracking()
                .Where(c => c.UserId == uid)
                .OrderByDescending(c => c.DateCard)
                .ToList();
            var lines = new List<string>();
            foreach (var c in cards)
            {
                var num = c.NumberCard ?? "";
                var masked = num.Length > 4 ? $"•••• {num[^4..]}" : num;
                var exp = c.DateCard?.ToString("MM/yyyy") ?? "—";
                lines.Add($"{masked} · срок {exp}" + (c.IsPriority == true ? " · приоритетная" : ""));
            }

            CardsList.ItemsSource = lines;
            TxtCardsHint.Text = lines.Count == 0 ? "Карт нет." : $"Карт: {lines.Count}";
        }
        catch
        {
            TxtBalance.Text = "—";
            TxtCardsHint.Text = "Не удалось загрузить данные.";
            CardsList.ItemsSource = null;
        }
    }

    private void BtnTopUp_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
            return;
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new CardTopUpPage());
    }
}
