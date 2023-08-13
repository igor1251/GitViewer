using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitViewer.GitStorage.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Commit> Commits { get; set; } = new();
        public List<Repository> Repositories { get; set; } = new();
    }
}
