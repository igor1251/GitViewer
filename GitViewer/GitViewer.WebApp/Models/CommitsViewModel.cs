using System.ComponentModel.DataAnnotations;

using GitViewer.GitStorage.Models;

namespace GitViewer.WebApp.Models
{
    public class CommitsViewModel : PagedResult<SelectableCommitEditorViewModel>
    {
        [Required(ErrorMessage = "Owner can't be null")]
        public string Owner { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repo can't be null")]
        public string Repo { get; set; } = string.Empty;

        [Required(ErrorMessage = "User login can't be null")]
        public string Login { get; set; } = string.Empty;

        public List<long> GetSelectedIds() => 
            (from commit in this.Results where commit.Selected select commit.Id).ToList();
    }
}
