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
        this.Opened += (_, __) =>
        {
            DataContext = new MainViewModel(this);
        };
    }
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        BeginMoveDrag(e);
    }
}