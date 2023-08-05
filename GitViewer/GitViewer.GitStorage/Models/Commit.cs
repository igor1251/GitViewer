using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitViewer.GitStorage.Models
{
    public class Commit
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public User? Author { get; set; }
        public Repository? Repository { get; set; }
    }
}
