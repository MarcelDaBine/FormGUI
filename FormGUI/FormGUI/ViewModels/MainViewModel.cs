using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using FormGUI.Interfaces;
using FormGUI.Services;

namespace FormGUI.ViewModels;


public class MainViewModel: ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private readonly IFileIo _fileIo;

    //CurrentViewModel holds the viewmodel that is displayed
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainViewModel(Window window)
    {
        _fileIo = new JsonFileIo(window);
        
        _currentViewModel = new DashboardViewModel(_fileIo);
        
        //_currentViewModel = new LoginViewModel(SwitchToDashboard);
    }

    private void SwitchToDashboard()
    { 
        CurrentViewModel = new DashboardViewModel(_fileIo);
    }
}