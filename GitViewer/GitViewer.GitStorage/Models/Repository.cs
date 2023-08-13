using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitViewer.GitStorage.Models
{
    public class Repository
    {
        public long Id { get; set; }
        public long RemoteId { get; set; }
        public string Name { get; set; } = string.Empty;
        public User? Owner { get; set; }
        public List<Commit> Commits { get; set; } = new();
    }
}
