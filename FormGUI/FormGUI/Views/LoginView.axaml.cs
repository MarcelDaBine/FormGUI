using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;

namespace FormGUI.Views;

public partial class LoginView : UserControl
{
    
    public LoginView()
    {
        InitializeComponent();
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
            Debug.Assert(SubmitButton.Command != null, "SubmitButton.Command != null");
            SubmitButton.Command.Execute(null);
        }
    }
}