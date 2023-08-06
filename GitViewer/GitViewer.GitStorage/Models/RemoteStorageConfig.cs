using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace GitViewer.GitStorage.Models
{
    [PrimaryKey(nameof(ClientId))]
    public class RemoteStorageConfig
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string Token { get; set; } = string.Empty;
        
        public bool OnlyTokenAuthAllowed => string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password);

        public bool Empty => string.IsNullOrEmpty(Token) && string.IsNullOrEmpty(ClientId);
        public bool TokenNotSet => string.IsNullOrEmpty(Token);
    }
}
