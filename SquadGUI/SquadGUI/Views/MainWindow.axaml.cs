using Avalonia.Controls;
using SquadGUI.ViewModels;

namespace SquadGUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}