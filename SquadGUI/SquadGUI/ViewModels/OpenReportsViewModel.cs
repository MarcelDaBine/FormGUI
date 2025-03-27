using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using SquadGUI.Assets.Templates;
using SquadGUI.Interfaces;

namespace SquadGUI.ViewModels;

public class OpenReportsViewModel : ViewModelBase, INavigable
{
    private readonly IHttpService _httpService;
    private readonly IFileIo _fileIo;

    private bool _isPaneOpen;
    private int _pageNumber;
    private string _pageNumberString = "0";
    private readonly Action<ReportModel> _navigateToDashboard;

    public ObservableCollection<ProjectSummaryItem> Summaries { get; } = new();
    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }

    public string PageNumberString
    {
        get => _pageNumberString;
        set => this.RaiseAndSetIfChanged(ref _pageNumberString, value);
    }

    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            this.RaiseAndSetIfChanged(ref _pageNumber, value);
            this.RaiseAndSetIfChanged(ref _pageNumberString, value.ToString());
            this.RaisePropertyChanged(nameof(PageNumberString));
        }
    }
    
    public ReactiveCommand<Unit, Unit> TogglePaneCommand { get; }
    public ReactiveCommand<Unit,Task> OpenLocalCommand { get; }
    public ReactiveCommand<Unit, Task> PreviousPageCommand { get; }
    public ReactiveCommand<Unit, Task> NextPageCommand { get; }
    private void TogglePane() => IsPaneOpen = !IsPaneOpen;
    
    
    public OpenReportsViewModel(IHttpService httpService, IFileIo fileIo, Action<ReportModel> navigateToDashboard)
    {
        _httpService = httpService;
        _fileIo = fileIo;
        _pageNumber = 0;
        
        _navigateToDashboard = navigateToDashboard;
        
        //_openProject = openProject;
        TogglePaneCommand = ReactiveCommand.Create(TogglePane);
        OpenLocalCommand = ReactiveCommand.Create(Deserialize);
        PreviousPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (PageNumber != 0)
            {
                PageNumber--;
                await LoadReportsAsync(PageNumber);
            }
            return Task.CompletedTask;
        });
        
        NextPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if(Summaries.Count < 20)
            {
                return Task.CompletedTask;
            }
            
            PageNumber++;
            await LoadReportsAsync(PageNumber);
            return Task.CompletedTask;
        });
    }
    
    private async Task LoadReportsAsync(int pageNumber = 0)
    {
        try
        {
            var command = ReactiveCommand.Create<ProjectSummaryItem>(item =>
            {
                var reportModel = _httpService.GetFormByIdAsync(item.Model.Id).Result;
                reportModel.Id = item.Model.Id;
                _navigateToDashboard(reportModel);
            });
            var reports = await _httpService.GetSummariesAsync(pageNumber);
            
            Summaries.Clear();
            foreach (var report in reports.OrderBy((a) => { return a.Id;}))
                Summaries.Add(new ProjectSummaryItem(report, command));        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OpenReports] Failed to load summaries: {ex.Message}");
        }
    }
    
    private async Task Deserialize()
    {
        try
        {
            var report = await _fileIo.OpenJsonAsync<ReportModel>();
            _navigateToDashboard(report);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async void OnNavigatedTo()
    {
        await LoadReportsAsync();
    }
}