using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using GitViewer.GitStorage.Models;

namespace GitViewer.Desktop.Models
{
    public partial class SelectableCommit : ObservableObject
    {
        [ObservableProperty]
        bool _isChecked = false;

        [ObservableProperty]
        Commit? _commit;
    }
}
