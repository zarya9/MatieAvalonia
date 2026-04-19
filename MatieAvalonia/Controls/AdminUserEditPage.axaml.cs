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

public partial class AdminUserEditPage : UserControl
{
    private readonly int? _preselectUserId;

    public AdminUserEditPage()
        : this(null)
    {
    }

    public AdminUserEditPage(int? preselectUserId)
    {
        InitializeComponent();
        FormInputSanitizer.Wire(TxtEditFname, s => FormInputSanitizer.PersonName(s, DbFieldLimits.UserFio));
        FormInputSanitizer.Wire(TxtEditName, s => FormInputSanitizer.PersonName(s, DbFieldLimits.UserFio));
        FormInputSanitizer.Wire(TxtEditPatronymic, s => FormInputSanitizer.PersonName(s, DbFieldLimits.UserFio));
        FormInputSanitizer.Wire(TxtEditLogin, s => FormInputSanitizer.Login(s, DbFieldLimits.UserLogin));
        FormInputSanitizer.Wire(TxtResetPassword, s => FormInputSanitizer.Password(s, DbFieldLimits.UserPassword));
        _preselectUserId = preselectUserId;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            var roles = ConnectionClass.connect.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new RoleListItem { IdRole = r.IdRole, Name = r.Name ?? "" })
                .ToList();
            CmbEditRole.ItemsSource = roles;

            var users = ConnectionClass.connect.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .OrderBy(u => u.Fname)
                .ToList()
                .Select(u => new UserPickItem
                {
                    IdUser = u.IdUser,
                    Display = $"{PersonFormat.Fio(u)} · {u.Login}"
                })
                .ToList();
            CmbPickUser.ItemsSource = users;
            if (_preselectUserId is int uid)
            {
                var pick = users.FirstOrDefault(u => u.IdUser == uid);
                if (pick != null)
                    CmbPickUser.SelectedItem = pick;
                else if (users.Count > 0)
                    CmbPickUser.SelectedIndex = 0;
            }
            else if (users.Count > 0)
                CmbPickUser.SelectedIndex = 0;
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
            TxtAdminUserError.IsVisible = false;
            TxtAdminUserError.Text = "";
        }
        else
        {
            TxtAdminUserError.Text = msg.Trim();
            TxtAdminUserError.IsVisible = true;
        }
    }

    private void CmbPickUser_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CmbPickUser.SelectedItem is not UserPickItem pick)
            return;
        try
        {
            var u = ConnectionClass.connect.Users.AsNoTracking().Include(x => x.Role).FirstOrDefault(x => x.IdUser == pick.IdUser);
            if (u == null)
                return;
            TxtEditFname.Text = u.Fname ?? "";
            TxtEditName.Text = u.Name ?? "";
            TxtEditPatronymic.Text = u.Patronymic ?? "";
            TxtEditLogin.Text = u.Login ?? "";
            TxtResetPassword.Text = "";
            TxtUserUpdatedAt.Text = $"Обновлено: {u.UpdatedAt?.ToString("g") ?? "—"}";

            if (CmbEditRole.ItemsSource is System.Collections.IEnumerable list)
            {
                foreach (var o in list)
                {
                    if (o is RoleListItem ri && ri.IdRole == u.RoleId)
                    {
                        CmbEditRole.SelectedItem = o;
                        break;
                    }
                }
            }
        }
        catch
        {
            SetError("Ошибка при загрузке пользователя.");
        }
    }

    private void BtnSaveUser_OnClick(object? sender, RoutedEventArgs e)
    {
        SetError(null);
        if (CmbPickUser.SelectedItem is not UserPickItem pick)
        {
            SetError("Выберите пользователя.");
            return;
        }

        if (CmbEditRole.SelectedItem is not RoleListItem role)
        {
            SetError("Выберите роль.");
            return;
        }

        var login = (TxtEditLogin.Text ?? "").Trim();
        if (string.IsNullOrEmpty(login))
        {
            SetError("Логин не может быть пустым.");
            return;
        }

        var entity = ConnectionClass.connect.Users.Find(pick.IdUser);
        if (entity == null)
        {
            SetError("Пользователь не найден.");
            return;
        }

        try
        {
            entity.Fname = (TxtEditFname.Text ?? "").Trim();
            entity.Name = (TxtEditName.Text ?? "").Trim();
            entity.Patronymic = string.IsNullOrWhiteSpace(TxtEditPatronymic.Text) ? null : TxtEditPatronymic.Text.Trim();
            entity.Login = login;
            entity.RoleId = role.IdRole;
            var pwd = (TxtResetPassword.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(pwd))
                entity.Password = pwd;
            entity.UpdatedAt = DbTimestamp.Now;
            ConnectionClass.connect.SaveChanges();
            TxtUserUpdatedAt.Text = $"Обновлено: {entity.UpdatedAt?.ToString("g") ?? "—"}";
        }
        catch (Exception ex)
        {
            try
            {
                ConnectionClass.connect.Entry(entity).Reload();
            }
            catch
            {
                // игнорируем
            }

            SetError(DbSaveExceptionFormatter.Format(ex, "Не удалось сохранить."));
        }
    }

    private void BtnResetPwd_OnClick(object? sender, RoutedEventArgs e)
    {
        var pwd = (TxtResetPassword.Text ?? "").Trim();
        if (string.IsNullOrEmpty(pwd))
            TxtResetPassword.Text = "123456";
    }

    private void BtnCancelAdminUser_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is MainWindow mw)
            mw.NavigateTo(new AdminUsersListPage());
    }
}
