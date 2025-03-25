using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SquadGUI.Views;

public partial class SidePaneButtons : UserControl
{
    public static readonly StyledProperty<ICommand?> TogglePaneCommandProperty =
        AvaloniaProperty.Register<SidePaneButtons, ICommand?>(nameof(TogglePaneCommand));
    
    public static readonly StyledProperty<ICommand?> OpenFileCommandProperty =
        AvaloniaProperty.Register<SidePaneButtons, ICommand?>(nameof(OpenFileCommand));

    public ICommand? TogglePaneCommand
    {
        get => GetValue(TogglePaneCommandProperty);
        set => SetValue(TogglePaneCommandProperty, value);
    }
    public ICommand? OpenFileCommand
    {
        get => GetValue(OpenFileCommandProperty);
        set => SetValue(OpenFileCommandProperty, value);
    }

// Repeat for Reports, Settings, etc.


    public SidePaneButtons()
    {
        InitializeComponent();
    }
}
