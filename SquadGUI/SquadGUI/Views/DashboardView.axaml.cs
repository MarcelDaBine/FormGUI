using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SquadGUI.Assets;
using SquadGUI.ViewModels;


namespace SquadGUI.Views;


public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }
}