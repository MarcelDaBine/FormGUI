using Avalonia.Controls;
using Avalonia.Input;
using FormGUI.Interfaces;
using FormGUI.ViewModels;

namespace FormGUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, __) =>
        {
            DataContext = new MainViewModel(this);
        };
    }
}