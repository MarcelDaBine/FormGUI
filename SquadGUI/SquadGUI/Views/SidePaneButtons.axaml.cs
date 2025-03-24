using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SquadGUI.Views;

public partial class SidePaneButtons : UserControl
{
    public static readonly StyledProperty<ICommand?> TogglePaneCommandProperty =
        AvaloniaProperty.Register<SidePaneButtons, ICommand?>(nameof(TogglePaneCommand));

    public ICommand? TogglePaneCommand
    {
        get => GetValue(TogglePaneCommandProperty);
        set => SetValue(TogglePaneCommandProperty, value);
    }

    public SidePaneButtons()
    {
        InitializeComponent();
    }
}
