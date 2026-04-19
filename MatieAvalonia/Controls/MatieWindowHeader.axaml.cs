using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MatieAvalonia.Classes;
using MatieAvalonia.Models;

namespace MatieAvalonia.Controls;

public partial class MatieWindowHeader : UserControl
{
    public MatieWindowHeader()
    {
        InitializeComponent();
    }

    public void RefreshSessionLabel()
    {
        var u = Session.CurrentUser;
        if (u == null)
        {
            TxtSessionInfo.Text = "";
            TxtSessionInfo.IsVisible = false;
            return;
        }

        var fio = PersonFormat.Fio(u);
        var role = u.Role?.Name?.Trim();
        if (string.IsNullOrEmpty(role))
            role = "—";
        TxtSessionInfo.Text = $"{fio} · {role}";
        TxtSessionInfo.IsVisible = true;
    }

    private void DragZone_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (VisualRoot is not Window window)
            return;
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;
        window.BeginMoveDrag(e);
    }

    private void BtnMinimize_OnClick(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window window)
            window.WindowState = WindowState.Minimized;
    }

    private void BtnClose_OnClick(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window window)
            window.Close();
    }
}
