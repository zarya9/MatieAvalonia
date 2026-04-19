using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MatieAvalonia;

public partial class ExitConfirmWindow : Window
{
    public ExitConfirmWindow()
    {
        InitializeComponent();
    }

    public void SetMessage(string text) => TxtMessage.Text = text;

    private void BtnYes_OnClick(object? sender, RoutedEventArgs e) => Close(true);

    private void BtnNo_OnClick(object? sender, RoutedEventArgs e) => Close(false);
}
