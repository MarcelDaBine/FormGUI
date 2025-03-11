using System;
using Avalonia.Animation;
using ReactiveUI;

namespace SquadGUI.ViewModels;


public class MainViewModel: ViewModelBase
{
    private ViewModelBase _currentViewModel;

    //CurrentViewModel holds the viewmodel that is displayed
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainViewModel()
    {
        //CurrentViewModel = new LoginViewModel(SwitchToDashboard);
        CurrentViewModel = new DashboardViewModel();
    }

    private void SwitchToDashboard()
    { 
        CurrentViewModel = new DashboardViewModel();
    }
}