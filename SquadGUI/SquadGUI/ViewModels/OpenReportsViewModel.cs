using System.Reactive;
using ReactiveUI;
using SquadGUI.Interfaces;

namespace SquadGUI.ViewModels;

public class OpenReportsViewModel : ViewModelBase
{
    private IHttpService _httpService;
    
    private bool _isPaneOpen;

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }
    
    public ReactiveCommand<Unit, Unit> TogglePaneCommand { get; }
    
    public OpenReportsViewModel(IHttpService httpService)
    {
        _httpService = httpService;
        TogglePaneCommand = ReactiveCommand.Create(TogglePane);
    }

    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}