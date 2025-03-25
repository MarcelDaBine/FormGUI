using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using SquadGUI.Interfaces;
using SquadGUI.Services;

namespace SquadGUI.ViewModels;


public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private ViewModelBase _previousViewModel;
    private readonly ViewModelBase _dashboardViewModel;
    private ViewModelBase _openReportsViewModel;
    
    private IFileIo _fileIo;
    private IHttpService _httpService;
    
    private bool _isPaneOpen = false;
    private SplitViewDisplayMode _sidePaneMode;

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }

    public SplitViewDisplayMode SidePaneMode
    {
        get => _sidePaneMode;
        set => this.RaiseAndSetIfChanged(ref _sidePaneMode, value);
    }


    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (value is LoginViewModel)
            {
                SidePaneMode = SplitViewDisplayMode.Inline;
                IsPaneOpen = false;
            }
            else
            {
                SidePaneMode = SplitViewDisplayMode.CompactOverlay;
            }
            this.RaiseAndSetIfChanged( ref _currentViewModel, value);
        }
    }

    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowReportsCommand { get; }
    
    public ICommand TogglePaneCommand { get; }
    
    public ICommand BackCommand { get; }

    public MainViewModel(Window window)
    {
        _fileIo = new JsonFileIo(window);
        _httpService = new HttpService();
        
        _dashboardViewModel = new DashboardViewModel(_fileIo, _httpService);
        _openReportsViewModel = new OpenReportsViewModel(_httpService);

        ShowDashboardCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentViewModel is not DashboardViewModel)
            {
                _previousViewModel = CurrentViewModel;
                CurrentViewModel = _dashboardViewModel;
            }
        });

        ShowReportsCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentViewModel is not OpenReportsViewModel)
            {
                _previousViewModel = CurrentViewModel;
                CurrentViewModel = _openReportsViewModel;
            }
        });
        
        BackCommand = ReactiveCommand.Create(() =>
        {
            if (_previousViewModel != null)
            {
                var temp = CurrentViewModel;
                CurrentViewModel = _previousViewModel;
                _previousViewModel = temp;
            }
        });
        
        TogglePaneCommand = ReactiveCommand.Create( () => 
            IsPaneOpen = !IsPaneOpen);

        //CurrentViewModel = new DashboardViewModel(_fileIo, _httpService);
        CurrentViewModel = new LoginViewModel(SwitchToDashboard, _httpService);
    }

    private void SwitchToDashboard()
    {
        CurrentViewModel = _dashboardViewModel;
    }
}
