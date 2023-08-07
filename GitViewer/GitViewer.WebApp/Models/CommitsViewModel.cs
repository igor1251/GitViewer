using System.ComponentModel.DataAnnotations;
using GitViewer.GitStorage.Models;

namespace GitViewer.WebApp.Models
{
    public class CommitsViewModel
    {
        [Required(ErrorMessage = "Owner can't be null")]
        public string Owner { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repo can't be null")]
        public string Repo { get; set; } = string.Empty;

        [Required(ErrorMessage = "User login can't be null")]
        public string Login { get; set; } = string.Empty;

        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public List<SelectableCommitEditorViewModel> Results { get; set; } = new();

        public List<long> GetSelectedIds() => 
            (from commit in this.Results where commit.Selected select commit.Id).ToList();
    }
}
