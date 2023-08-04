using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GitViewer.Storage.Models;

using Microsoft.EntityFrameworkCore;

namespace GitViewer.Storage
{
    public class GitStorageDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Commit> Commits { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Settings> Settings { get; set; }

        public GitStorageDbContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=gitdb;Trusted_Connection=True;");
        }
    }
}
