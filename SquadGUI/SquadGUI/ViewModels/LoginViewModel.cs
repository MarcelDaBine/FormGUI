using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SquadGUI.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private string _email;
    private string _password;
    private string _errortext;
    
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    
    private readonly Action _navigateToDashboard;
    
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string Password {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ErrorText
    {
        get => _errortext;
        set => this.RaiseAndSetIfChanged(ref _errortext, value);
    }
    

    public LoginViewModel(Action navigateToDashboard)
    {
        LoginCommand = ReactiveCommand.Create(LoginLogic, outputScheduler: RxApp.MainThreadScheduler);
        _navigateToDashboard = navigateToDashboard;
        
        _email = string.Empty;
        _errortext = string.Empty;
        _password = string.Empty;
    }
    
    private void LoginLogic()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            ErrorText = "Please input your credentials.";
            return;
        }

        if (!Email.Contains("@cox.com"))
        {
            ErrorText= "Please input an correct email address.";
            return;
        }

        ErrorText = string.Empty;
        _navigateToDashboard();
    }
}