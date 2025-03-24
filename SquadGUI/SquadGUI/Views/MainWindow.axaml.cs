using Avalonia.Controls;
using Avalonia.Input;
using SquadGUI.Interfaces;
using SquadGUI.ViewModels;

namespace SquadGUI.Views;

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