using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitViewer.GitStorage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GitViewer.GitStorage.Local
{
    public class GitLocalStorageDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Commit> Commits { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<RemoteStorageConfig> RemoteStorageConfigs { get; set; }

        readonly ILogger<GitLocalStorageDbContext> _logger;

        public GitLocalStorageDbContext(ILogger<GitLocalStorageDbContext> logger)
        {
            _logger = logger;

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=gitcommitsdb;Trusted_Connection=True;");
            optionsBuilder.LogTo(msg => _logger.LogInformation(msg));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commit>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Commits);

            modelBuilder.Entity<Commit>()
                .HasOne(c => c.Repository)
                .WithMany(r => r.Commits);

            modelBuilder.Entity<Repository>()
                .HasOne(r => r.Owner)
                .WithMany(u => u.Repositories);

            modelBuilder.Entity<Repository>()
                .HasMany(r => r.Commits)
                .WithOne(c => c.Repository);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Repositories)
                .WithOne(r => r.Owner);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Commits)
                .WithOne(c => c.Author);
        }
    }
}
