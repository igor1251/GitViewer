using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GitViewer.Desktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        string _clientID = string.Empty;

        [ObservableProperty]
        string _clientSecret = string.Empty;

        [ObservableProperty]
        string _login = string.Empty;

        [ObservableProperty]
        string _password = string.Empty;

        IRelayCommand? _signInCommand, _cancelCommand;

        public IRelayCommand SignInCommand => _signInCommand ??= new RelayCommand<Window>(win =>
        {
            if (win != null)
            {
                win.DialogResult = true;
            }
        }, win => !string.IsNullOrEmpty(ClientID));

        public IRelayCommand CancelCommand => _cancelCommand ??= new RelayCommand<Window>(win =>
        {
            if (win != null)
            {
                win.DialogResult = false;
            }
        });

        public (string clientID, string clientSecret, string login, string password) GetUserInput()
        {
            return (ClientID, ClientSecret, Login, Password);
        }
    }
}
