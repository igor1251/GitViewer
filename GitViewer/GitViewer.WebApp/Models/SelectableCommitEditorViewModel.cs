using GitViewer.GitStorage.Models;

namespace GitViewer.WebApp.Models
{
    public class SelectableCommitEditorViewModel
    {
        public bool Selected { get; set; } = false;
        public Commit? Commit { get; set; }
    }
}
