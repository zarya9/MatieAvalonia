using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.ApplicationLifetimes;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia;

public partial class AuthPage : Window
{
    private bool _skipCloseConfirmation;

    public AuthPage()
    {
        InitializeComponent();
        FormInputSanitizer.Wire(TxtLogin, s => FormInputSanitizer.Login(s, DbFieldLimits.UserLogin));
        FormInputSanitizer.Wire(TxtPassword, s => FormInputSanitizer.Password(s, DbFieldLimits.UserPassword));
        RegisterRoot.NavigateBack += OnRegisterNavigateBack;
        Closing += AuthPage_OnClosing;
    }

    private async void AuthPage_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_skipCloseConfirmation)
            return;

        e.Cancel = true;
        var dlg = new ExitConfirmWindow();
        dlg.SetMessage("Завершить работу приложения?");
        var ok = await dlg.ShowDialog<bool>(this);
        if (ok)
        {
            _skipCloseConfirmation = true;
            Close();
        }
    }

    private void OnRegisterNavigateBack(object? sender, System.EventArgs e)
    {
        RegisterRoot.IsVisible = false;
        LoginRoot.IsVisible = true;
    }

    private void SetAuthError(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            TxtAuthError.Text = "";
            TxtAuthError.IsVisible = false;
        }
        else
        {
            TxtAuthError.Text = message.Trim();
            TxtAuthError.IsVisible = true;
        }
    }

    private void BtnAuth_Click(object? sender, RoutedEventArgs e)
    {
        SetAuthError(null);
        var login = (TxtLogin.Text ?? "").Trim();
        var password = (TxtPassword.Text ?? "").Trim();
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            SetAuthError("Введите логин и пароль.");
            return;
        }

        try
        {
            var user = ConnectionClass.connect.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                SetAuthError("Неверный логин или пароль.");
                return;
            }

            Session.CurrentUser = user;
            var main = new MainWindow();
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = main;
                main.Show();
            }
            _skipCloseConfirmation = true;
            Close();
        }
        catch
        {
            SetAuthError("Не удалось подключиться к базе данных. Проверьте сервер PostgreSQL.");
        }
    }

    private void BtnReg_Click(object? sender, RoutedEventArgs e)
    {
        LoginRoot.IsVisible = false;
        RegisterRoot.IsVisible = true;
    }
}
