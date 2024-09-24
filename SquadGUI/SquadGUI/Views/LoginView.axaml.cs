using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SquadGUI.ViewModels;

namespace SquadGUI.Views;

public partial class LoginView : UserControl
{
    private string email, password;
    
    public LoginView()
    {
        InitializeComponent();
    }
    

    private void Button_OnClick_Login(object? sender, RoutedEventArgs e)
    {
        if (EmailBox.Text?.Length == 0 || PassBox.Text?.Length == 0)
        {
            ErrorBlock.Text = "Please input your credentials.";
            return;
        }

        if (!EmailBox.Text.Contains("@cox.com"))
        {
            ErrorBlock.Text = "Please input an correct email address.";
            return;
        }
        
        var dashboard = new Dashboard();
        ErrorBlock.Text = "";
        email = EmailBox.Text;
        password = PassBox.Text;

        
    }

    private void EmailBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            PassBox.Focus();
        }
    }

    private void PassBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Button_OnClick_Login(null, null);
        }
    }
}