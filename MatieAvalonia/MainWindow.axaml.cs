using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Controls.ApplicationLifetimes;
using MatieAvalonia.Classes;
using MatieAvalonia.Controls;
using MatieAvalonia.Data;
using MatieAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace MatieAvalonia;

public partial class MainWindow : Window
{
    private bool _skipCloseConfirmation;
    private bool _suppressCatalogCombo;

    public MainWindow()
    {
        InitializeComponent();
        FormInputSanitizer.Wire(TxtCatalogSearch, s => FormInputSanitizer.PlainLine(s, DbFieldLimits.CatalogSearch));
        Closing += MainWindow_OnClosing;
        Opened += (_, _) => ApplyRoleVisibility();
        SetPage(new ServicesCatalogPage(null));
    }

    public void NavigateTo(Control page) => SetPage(page);

    public string GetCatalogSearchText() => TxtCatalogSearch.Text ?? "";

    public int? GetCatalogCollectionId()
    {
        if (CmbCatalogCollection.SelectedItem is CollectionListItem li && li.IdCollection != 0)
            return li.IdCollection;
        return null;
    }

    private void SetPage(Control page)
    {
        PageHost.Content = page;
        OnPageHostChanged();
    }

    private void OnPageHostChanged()
    {
        if (PageHost.Content is ServicesCatalogPage cat)
        {
            cat.BindShell(this);
            PanelCatalogChrome.IsEnabled = true;
            FillCatalogCollectionCombo();
            SyncCatalogChrome(cat);
        }
        else
        {
            PanelCatalogChrome.IsEnabled = false;
            ResetCatalogTabBorders();
            SetFooterForPage(PageHost.Content);
        }

        ApplyRoleVisibility();
    }

    public void NavigateToCatalog(int? collectionId = null)
    {
        TxtCatalogSearch.Text = "";
        ResetCatalogTabBorders();
        SetPage(new ServicesCatalogPage(collectionId));
        if (collectionId is int cid && cid > 0 && CmbCatalogCollection.ItemsSource is IEnumerable<CollectionListItem> list)
        {
            _suppressCatalogCombo = true;
            var match = list.FirstOrDefault(i => i.IdCollection == cid);
            if (match != null)
                CmbCatalogCollection.SelectedItem = match;
            _suppressCatalogCombo = false;
        }
    }

    private void SetFooterForPage(object? content)
    {
        TxtMainFooter.Text = content switch
        {
            CollectionsPage => "Коллекции · двойной щелчок — услуги этой коллекции",
            MyBookingsPage => "Мои записи · двойной щелчок — карточка услуги; «Повторить» — новая запись",
            MasterClientsPage => "Клиенты мастера · двойной щелчок — карточка услуги",
            BookingCreatePage => "Новая запись",
            ReviewCreatePage => "Новый отзыв",
            ServiceDetailPage => "Карточка услуги",
            UserBalancePage => "Баланс",
            CardTopUpPage => "Пополнение карты",
            ModeratorServicesPage => "Список услуг",
            ModeratorServiceFormPage => "Форма услуги",
            ModeratorMasterServiceBindingsPage => "Привязки мастер–услуга",
            ModeratorMasterQualificationPage => "Квалификации",
            AdminUsersListPage => "Пользователи · двойной щелчок — правка",
            AdminUserEditPage => "Правка пользователя",
            AdminStaffPage => "Сотрудники и мастера",
            MasterQualificationRequestPage => "Заявка на квалификацию",
            _ => "Лавка «Матье»"
        };
    }

    public void SyncCatalogChrome(ServicesCatalogPage page)
    {
        TxtMainFooter.Text = page.PaginationSummary;
        UpdateCatalogTabBorders(page.GetLineFilter());
    }

    private void ResetCatalogTabBorders()
    {
        BorderTabCustom.Background = Brushes.White;
        BorderTabCosplay.Background = Brushes.White;
    }

    private void UpdateCatalogTabBorders(CatalogLineFilter line)
    {
        var active = new SolidColorBrush(Color.Parse("#FFE8E8E8"));
        var inactive = Brushes.White;
        BorderTabCustom.Background = line == CatalogLineFilter.Custom ? active : inactive;
        BorderTabCosplay.Background = line == CatalogLineFilter.Cosplay ? active : inactive;
    }

    private void ApplyCatalogLineFilter(CatalogLineFilter line)
    {
        if (PageHost.Content is not ServicesCatalogPage cat)
        {
            NavigateToCatalog(null);
            cat = (ServicesCatalogPage)PageHost.Content!;
        }

        cat.SetLineFilter(line);
    }

    private void BorderTabCustom_OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
        ApplyCatalogLineFilter(CatalogLineFilter.Custom);

    private void BorderTabCosplay_OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
        ApplyCatalogLineFilter(CatalogLineFilter.Cosplay);

    private void FillCatalogCollectionCombo()
    {
        _suppressCatalogCombo = true;
        try
        {
            var items = new List<CollectionListItem>
            {
                new() { IdCollection = 0, Name = "Все коллекции" }
            };
            items.AddRange(
                ConnectionClass.connect.Collections
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new CollectionListItem { IdCollection = c.IdCollection, Name = c.Name ?? "" }));
            CmbCatalogCollection.ItemsSource = items;
            CmbCatalogCollection.SelectedIndex = 0;
        }
        catch
        {
            CmbCatalogCollection.ItemsSource = new List<CollectionListItem>
            {
                new() { IdCollection = 0, Name = "Все коллекции" }
            };
            CmbCatalogCollection.SelectedIndex = 0;
        }
        finally
        {
            _suppressCatalogCombo = false;
        }
    }

    private void TxtCatalogSearch_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (PageHost.Content is ServicesCatalogPage cat)
            cat.SetSearch(TxtCatalogSearch.Text);
    }

    private void CmbCatalogCollection_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressCatalogCombo)
            return;
        if (PageHost.Content is not ServicesCatalogPage cat)
            return;
        if (CmbCatalogCollection.SelectedItem is not CollectionListItem li)
            return;
        cat.SetCollectionFilter(li.IdCollection == 0 ? null : li.IdCollection);
    }

    private void BtnCatalogChromePrev_OnClick(object? sender, RoutedEventArgs e)
    {
        if (PageHost.Content is ServicesCatalogPage cat)
            cat.ChromePrevPage();
    }

    private void BtnCatalogChromeNext_OnClick(object? sender, RoutedEventArgs e)
    {
        if (PageHost.Content is ServicesCatalogPage cat)
            cat.ChromeNextPage();
    }

    private void ApplyRoleVisibility()
    {
        var u = Session.CurrentUser;
        var mod = RoleHelper.CanSeeModeratorMenu(u);
        var adm = RoleHelper.CanSeeAdminMenu(u);
        var mst = RoleHelper.CanSeeMasterMenu(u);
        var any = mod || adm || mst;
        var salonClient = RoleHelper.IsSalonClient(u);

        LblClientSections.IsVisible = salonClient;
        PanelClientMenu.IsVisible = salonClient;

        SepBeforeRoleMenus.IsVisible = any;
        LblRoleMenus.IsVisible = any;
        PanelModeratorMenu.IsVisible = mod;
        PanelAdminMenu.IsVisible = adm;
        PanelMasterMenu.IsVisible = mst;

        ShellHeader.RefreshSessionLabel();
    }

    private async void MainWindow_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_skipCloseConfirmation)
            return;

        e.Cancel = true;
        var dlg = new ExitConfirmWindow();
        dlg.SetMessage("Закрыть окно «Матье»?");
        var ok = await dlg.ShowDialog<bool>(this);
        if (ok)
        {
            _skipCloseConfirmation = true;
            Close();
        }
    }

    private void BtnCatalog_Click(object? sender, RoutedEventArgs e) => NavigateToCatalog(null);

    private void BtnCollections_Click(object? sender, RoutedEventArgs e) => SetPage(new CollectionsPage());

    private void BtnBalance_Click(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
            return;
        SetPage(new UserBalancePage());
    }

    private void BtnTopUp_Click(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
            return;
        SetPage(new CardTopUpPage());
    }

    private void BtnBook_Click(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
            return;
        SetPage(new BookingCreatePage());
    }

    private void BtnMyBookings_Click(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
            return;
        SetPage(new MyBookingsPage());
    }

    private void BtnReview_Click(object? sender, RoutedEventArgs e)
    {
        if (!RoleHelper.IsSalonClient(Session.CurrentUser))
            return;
        SetPage(new ReviewCreatePage());
    }

    private void BtnModeratorServices_Click(object? sender, RoutedEventArgs e) => SetPage(new ModeratorServicesPage());

    private void BtnModeratorServiceForm_Click(object? sender, RoutedEventArgs e) => SetPage(new ModeratorServiceFormPage());

    private void BtnModeratorBindings_Click(object? sender, RoutedEventArgs e) => SetPage(new ModeratorMasterServiceBindingsPage());

    private void BtnModeratorQual_Click(object? sender, RoutedEventArgs e) => SetPage(new ModeratorMasterQualificationPage());

    private void BtnAdminUsers_Click(object? sender, RoutedEventArgs e) => SetPage(new AdminUsersListPage());

    private void BtnAdminUserEdit_Click(object? sender, RoutedEventArgs e) => SetPage(new AdminUserEditPage());

    private void BtnAdminStaff_Click(object? sender, RoutedEventArgs e) => SetPage(new AdminStaffPage());

    private void BtnMasterClients_Click(object? sender, RoutedEventArgs e) => SetPage(new MasterClientsPage());

    private void BtnMasterQualRequest_Click(object? sender, RoutedEventArgs e) => SetPage(new MasterQualificationRequestPage());

    private void BtnServiceDetail_Click(object? sender, RoutedEventArgs e) => SetPage(new ServiceDetailPage());

    private void BtnLogout_Click(object? sender, RoutedEventArgs e)
    {
        _skipCloseConfirmation = true;
        Session.Clear();
        var auth = new AuthPage();
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = auth;
            auth.Show();
        }
        Close();
    }
}
