using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Numerics;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using SquadGUI.Interfaces;
using SquadGUI.Services;

namespace SquadGUI.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private string _email;
    private string _password;
    private string _errorText;

    private readonly List<string> _emailSuffix = new List<string> { "@", "." };
    
    private readonly IHttpService _httpService;
    
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
    

    public LoginViewModel(Action navigateToDashboard, IHttpService httpService)
    {
        LoginCommand = ReactiveCommand.Create(LoginLogic, outputScheduler: RxApp.MainThreadScheduler);
        _navigateToDashboard = navigateToDashboard;
        
        _email = string.Empty;
        _errorText = string.Empty;
        _password = string.Empty;
        _httpService = httpService;
    }
    
    private void LoginLogic()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            ErrorText = "Please input your credentials.";
            return;
        }

        if (!_emailSuffix.All(suffix => Email.Contains(suffix)))
        {
            ErrorText = "Please input a correct email address.";
            return;
        }
        
        var authResult = Task.Run(() => _httpService.Authenticate(Email,Password));

        if (authResult.Result != "Success")
        {
            ErrorText = authResult.Result;
            return;
        }

        ErrorText = string.Empty;
        _navigateToDashboard();
    }
}