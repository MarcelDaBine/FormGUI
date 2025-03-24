using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering.SceneGraph;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using SquadGUI.ViewModels;

namespace SquadGUI.Views;

public partial class CustomTitleBar : UserControl
{
    private SplitView? _splitView;

    public SplitView? SplitView
    {
        get => _splitView;
        set => _splitView = value;
    }
    
    public CustomTitleBar()
    {
        InitializeComponent();
        _splitView = null;
    }

    private void Close_OnClick(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window window)
        {
            window.Close();
        }
    }

    private void Tabs_OnClick(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window window)
        {
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; 
        }
    }

    private void Minimize_OnClick(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window window)
        {
            window.WindowState = WindowState.Minimized;
        }
    }
    private void Rectangle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            if (VisualRoot is Window window)
            {
                window.BeginMoveDrag(e);
            }
        }
    }
}