using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        ObservableCollection<SelectableCommit> _commits = new ObservableCollection<SelectableCommit>();

        IRelayCommand? _fetchCommand, _searchCommand, _deleteCommand, _settingsCommand, _appearanceCommand;

        bool CanFetchOrSearch => !string.IsNullOrEmpty(Owner) && !string.IsNullOrEmpty(Repo) && !string.IsNullOrEmpty(Author);

        public IRelayCommand FetchCommand => _fetchCommand ??= new RelayCommand(async () =>
        {

        }, () => CanFetchOrSearch);

        public IRelayCommand SearchCommand => _searchCommand ??= new RelayCommand(async () =>
        {

        }, () => CanFetchOrSearch);

        public IRelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(async () =>
        {

        });

        public IRelayCommand SettingsCommand => _settingsCommand ??= new RelayCommand(() =>
        {
            var dialog = new SettingsWindow();
            if (dialog.ShowDialog() ?? false)
            {
                var result = dialog.GetUserInput();
            }
        });

        public IRelayCommand AppearanceCommand => _appearanceCommand ??= new RelayCommand(() => new AppearanceWindow().ShowDialog());
    }
}
