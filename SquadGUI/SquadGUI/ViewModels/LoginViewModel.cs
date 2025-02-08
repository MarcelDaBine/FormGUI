using System;
using ReactiveUI;
using System.Reactive;

namespace SquadGUI.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private string _email;
    private string _password;
    private string _errorText;

    private const string _emailSuffix = "a";
    
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
        get => _errorText;
        set => this.RaiseAndSetIfChanged(ref _errorText, value);
    }
    

    public LoginViewModel(Action navigateToDashboard)
    {
        LoginCommand = ReactiveCommand.Create(LoginLogic, outputScheduler: RxApp.MainThreadScheduler);
        _navigateToDashboard = navigateToDashboard;
        
        _email = string.Empty;
        _errorText = string.Empty;
        _password = string.Empty;
    }
    
    private void LoginLogic()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            ErrorText = "Please input your credentials.";
            return;
        }

        if (!Email.Contains(_emailSuffix))
        {
            ErrorText= "Please input an correct email address.";
            return;
        }

        ErrorText = string.Empty;
        _navigateToDashboard();
    }
}