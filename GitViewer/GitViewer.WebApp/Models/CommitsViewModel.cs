using System.ComponentModel.DataAnnotations;

namespace GitViewer.WebApp.Models
{
    public class CommitsViewModel
    {
        [Required(ErrorMessage = "Owner can't be null")]
        public string Owner { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repo can't be null")]
        public string Repo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required user login can't be null")]
        public string Login { get; set; } = string.Empty;

        public List<(string timestamp, string author, string comment, string description)> Commits { get; set; } = new();
    }
}
