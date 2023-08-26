using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AdonisUI;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GitViewer.Desktop.Models;
using GitViewer.Desktop.Views;
using GitViewer.GitStorage.Models;

namespace GitViewer.Desktop.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        bool _isBusy = false;

        [ObservableProperty]
        bool _isDark = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FetchCommand))]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        string _owner = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FetchCommand))]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        string _repo = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FetchCommand))]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        string _author = string.Empty;

        [ObservableProperty]
        SelectableCommit? _selectedCommit;

        [ObservableProperty]
        ObservableCollection<SelectableCommit> _commits = new();

        bool CanFetchOrSearch => !string.IsNullOrEmpty(Owner) && !string.IsNullOrEmpty(Repo) && !string.IsNullOrEmpty(Author);

        [RelayCommand(CanExecute = nameof(CanFetchOrSearch))]
        async Task FetchAsync()
        {

        }

        [RelayCommand(CanExecute = nameof(CanFetchOrSearch))]
        async Task SearchAsync()
        {

        }

        [RelayCommand]
        async Task DeleteAsync()
        {

        }

        [RelayCommand]
        void Settings()
        {
            var dialog = new SettingsWindow();
            if (dialog.ShowDialog() ?? false)
            {
                var result = dialog.GetUserInput();
            }
        }

        partial void OnIsDarkChanged(bool value)
        {
            ResourceLocator.SetColorScheme(Application.Current.Resources, value ? ResourceLocator.DarkColorScheme : ResourceLocator.LightColorScheme);
        }
    }
}
