using Avalonia.Controls;
using Avalonia.Input;
using SquadGUI.ViewModels;

namespace SquadGUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        BeginMoveDrag(e);
    }


}