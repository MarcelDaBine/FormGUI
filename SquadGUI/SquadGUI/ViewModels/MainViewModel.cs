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
    private ViewModelBase _dashboardViewModel;
    private readonly ViewModelBase _openReportsViewModel;
    private readonly ViewModelBase _homeViewModel;
    
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
    
    public ICommand ShowReportsCommand { get; }
    public ICommand TogglePaneCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand LogOutCommand { get; }
    public ICommand NewFileCommand { get; }
    public ICommand HomeCommand { get; }

    public MainViewModel(Window window)
    {
        _fileIo = new JsonFileIo(window);
        _httpService = new HttpService();
        
        _openReportsViewModel = new OpenReportsViewModel(_httpService, _fileIo, LoadDashboard);
        _homeViewModel = new HomeViewModel();
        
        //CurrentViewModel = _dashboardViewModel;
        CurrentViewModel = new LoginViewModel(SwitchToHomeFromLogin, _httpService);

        ShowReportsCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentViewModel is not OpenReportsViewModel)
            {
                ((INavigable)_openReportsViewModel).OnNavigatedTo();
                _previousViewModel = CurrentViewModel;
                CurrentViewModel = _openReportsViewModel;
            }
        });
        
        BackCommand = ReactiveCommand.Create(() =>
        {
            if (_previousViewModel != null && _previousViewModel != CurrentViewModel && _previousViewModel is not DashboardViewModel)
            {
                var temp = CurrentViewModel;
                CurrentViewModel = _previousViewModel;
                _previousViewModel = temp;
            }
        });

        HomeCommand = ReactiveCommand.Create(() =>
        {
            if (_currentViewModel is not HomeViewModel)
            {
                _previousViewModel = CurrentViewModel;
                CurrentViewModel = _homeViewModel;
            }
        });

        LogOutCommand = ReactiveCommand.Create(Logout);
        
        NewFileCommand = ReactiveCommand.Create(NewFile);
        
        TogglePaneCommand = ReactiveCommand.Create( () => 
            IsPaneOpen = !IsPaneOpen);
    }

    private void SwitchToHomeFromLogin()
    {
        CurrentViewModel = _homeViewModel;
    }

    private void Logout()
    {
        CurrentViewModel = new LoginViewModel(SwitchToHomeFromLogin, _httpService);
    }

    private void NewFile()
    {
        var id = _httpService.CreateNewFileId().Result;
        _dashboardViewModel = new DashboardViewModel(_fileIo, _httpService, id);
        CurrentViewModel = _dashboardViewModel;
    }
    
    private void LoadDashboard(ReportModel reportModel)
    {
        CurrentViewModel = new DashboardViewModel(_fileIo, _httpService, reportModel.Id, reportModel);
    }
}
