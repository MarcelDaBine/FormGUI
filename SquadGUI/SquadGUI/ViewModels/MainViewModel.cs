using System;
using Avalonia.Animation;
using ReactiveUI;

namespace SquadGUI.ViewModels;


public class MainViewModel: ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private CrossFade _crossFade;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public CrossFade CrossFade
    {
        get => _crossFade;
        set => this.RaiseAndSetIfChanged(ref _crossFade, value);
    }

    public MainViewModel()
    {
        CurrentViewModel = new LoginViewModel(SwitchToDashboard);
    }

    private void SwitchToDashboard()
    {
        CrossFade = new CrossFade(TimeSpan.FromMilliseconds(500));
        CurrentViewModel = new DashboardViewModel();
    }
}