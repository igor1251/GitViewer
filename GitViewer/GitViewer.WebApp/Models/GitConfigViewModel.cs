using System.ComponentModel.DataAnnotations;

namespace GitViewer.WebApp.Models
{
    public class GitConfigViewModel
    {
        public string? Login { get; set; }

        public string? Password { get; set; }

        [Required(ErrorMessage = "ClientId can't be empty")]
        public string ClientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "ClientSecret can't be empty")]
        public string ClientSecret { get; set; } = string.Empty;
    }
}
