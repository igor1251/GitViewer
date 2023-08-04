using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitViewer.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace GitViewer.Storage
{
    public class GitStorage
    {
        readonly GitStorageDbContext _dbContext;

        public GitStorage(GitStorageDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<User>> GetUsersAsync() => await _dbContext.Users.ToListAsync();
        public async Task<List<Commit>> GetCommitsAsync() => await _dbContext.Commits.ToListAsync();
        public async Task<List<Repository>> GetRepositoriesAsync() => await _dbContext.Repositories.ToListAsync();
        public async Task<Settings?> GetSettingsAsync() => await _dbContext.Settings.LastOrDefaultAsync();

        public List<User> SearchUsers(Func<User, bool> predicate) => _dbContext.Users.Where(predicate).ToList();
        public List<Commit> SearchCommits(Func<Commit, bool> predicate) => _dbContext.Commits.Where(predicate).ToList();
        public List<Repository> SearchRepositories(Func<Repository, bool> predicate) => _dbContext.Repositories.Where(predicate).ToList();

        public async Task RemoveUsersAsync(Func<User, bool> predicate)
        {
            await _dbContext.Users.Where(predicate).AsQueryable().ExecuteDeleteAsync();
            await _dbContext.SaveChangesAsync();
        }
        public async Task RemoveCommitsAsync(Func<Commit, bool> predicate)
        {
            await _dbContext.Commits.Where(predicate).AsQueryable().ExecuteDeleteAsync();
            await _dbContext.SaveChangesAsync();
        }
        public async Task RemoveRepositoriesAsync(Func<Repository, bool> predicate)
        {
            await _dbContext.Repositories.Where(predicate).AsQueryable().ExecuteDeleteAsync();
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCommitAsync(Commit commit)
        {
            await _dbContext.Commits.Where(item => item.Id == commit.Id).AsQueryable().ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.Name, commit.Name)
                .SetProperty(c => c.Description, commit.Description));
            await _dbContext.SaveChangesAsync();
        }

        async Task CheckUserExists(User user)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(item => item.Name == user.Name);
            if (result == null)
            {
                await AddUserAsync(user);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task AddUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddCommitAsync(Commit commit)
        {
            if (commit.Author == null) throw new Exception("author cant't be null");
            await CheckUserExists(commit.Author);
            await _dbContext.Commits.AddAsync(commit);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddRepository(Repository repository)
        {
            if (repository.Owner == null) throw new Exception("owner can't be null");
            await CheckUserExists(repository.Owner);
            await _dbContext.Repositories.AddAsync(repository);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddSettingsAsync(Settings settings)
        {
            if (await _dbContext.Settings.AnyAsync()) await _dbContext.Settings.ExecuteDeleteAsync();
            await _dbContext.Settings.AddAsync(settings);
            await _dbContext.SaveChangesAsync();
        }
    }
}
