using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatieAvalonia.Classes;
using MatieAvalonia.Data;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia.Controls;

public partial class RegisterPage : UserControl
{
    public event EventHandler? NavigateBack;

    public RegisterPage()
    {
        InitializeComponent();
        FormInputSanitizer.Wire(TxtRegFname, s => FormInputSanitizer.PersonName(s, DbFieldLimits.UserFio));
        FormInputSanitizer.Wire(TxtRegName, s => FormInputSanitizer.PersonName(s, DbFieldLimits.UserFio));
        FormInputSanitizer.Wire(TxtRegPatronymic, s => FormInputSanitizer.PersonName(s, DbFieldLimits.UserFio));
        FormInputSanitizer.Wire(TxtRegLogin, s => FormInputSanitizer.Login(s, DbFieldLimits.UserLogin));
        FormInputSanitizer.Wire(TxtRegPassword, s => FormInputSanitizer.Password(s, DbFieldLimits.UserPassword));
    }

    private void BtnBack_Click(object? sender, RoutedEventArgs e) => NavigateBack?.Invoke(this, EventArgs.Empty);

    private void SetRegError(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            TxtRegError.Text = "";
            TxtRegError.IsVisible = false;
        }
        else
        {
            TxtRegError.Text = message.Trim();
            TxtRegError.IsVisible = true;
        }
    }

    private void BtnSubmitRegister_Click(object? sender, RoutedEventArgs e)
    {
        SetRegError(null);
        var fname = (TxtRegFname.Text ?? "").Trim();
        var name = (TxtRegName.Text ?? "").Trim();
        var patronymic = (TxtRegPatronymic.Text ?? "").Trim();
        var login = (TxtRegLogin.Text ?? "").Trim();
        var password = (TxtRegPassword.Text ?? "").Trim();
        if (string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            SetRegError("Заполните фамилию, имя, логин и пароль.");
            return;
        }

        if (ConnectionClass.connect.Users.Any(u => u.Login == login))
        {
            SetRegError("Пользователь с таким логином уже существует.");
            return;
        }

        var role = ConnectionClass.connect.Roles.AsNoTracking()
            .FirstOrDefault(r => r.Name == "Пользователь" || (r.Name != null && r.Name.Contains("Пользователь", StringComparison.OrdinalIgnoreCase)));
        if (role == null)
        {
            SetRegError("В базе не найдена роль «Пользователь». Добавьте роли в таблицу Role.");
            return;
        }

        var user = new User
        {
            Fname = fname,
            Name = name,
            Patronymic = string.IsNullOrEmpty(patronymic) ? null : patronymic,
            Login = login,
            Password = password,
            RoleId = role.IdRole,
            UpdatedAt = DbTimestamp.Now
        };

        try
        {
            ConnectionClass.connect.Users.Add(user);
            ConnectionClass.connect.SaveChanges();

            TxtRegFname.Text = "";
            TxtRegName.Text = "";
            TxtRegPatronymic.Text = "";
            TxtRegLogin.Text = "";
            TxtRegPassword.Text = "";
            NavigateBack?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ConnectionClass.TryDetach(user);
            SetRegError(DbSaveExceptionFormatter.Format(ex, "Не удалось сохранить данные."));
        }
    }
}
