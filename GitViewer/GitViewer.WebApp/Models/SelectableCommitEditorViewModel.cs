using GitViewer.GitStorage.Models;

namespace GitViewer.WebApp.Models
{
    public class SelectableCommitEditorViewModel
    {
        public bool Selected { get; set; } = false;
        public long Id { get; set; }
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public string? Author { get; set; }
    }
}
