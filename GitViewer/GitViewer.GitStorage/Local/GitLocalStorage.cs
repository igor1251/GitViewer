using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitViewer.GitStorage.Models;

using Microsoft.EntityFrameworkCore;

namespace GitViewer.GitStorage.Local
{
    public class GitLocalStorage
    {
        readonly GitLocalStorageDbContext _dbContext;

        public GitLocalStorage(GitLocalStorageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCommitAsync(Commit commit)
        {
            if (commit.Author == null) throw new Exception("author cant't be null");
            await _dbContext.Commits.AddAsync(commit);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddCommitAsync(List<Commit> commits)
        {
            await _dbContext.Commits.AddRangeAsync(commits);
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task AddRepositoryAsync(Repository repository)
        {
            if (repository.Owner == null) throw new Exception("owner can't be null");
            await _dbContext.Repositories.AddAsync(repository);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Commit>> GetCommitsAsync(string owner, string repo, string name)
        {
            var commits = await _dbContext.Commits.Where(item =>
                item.Repository != null && item.Repository.Owner != null && item.Repository.Owner.Name == owner &&
                item.Repository.Name == repo &&
                item.Author != null && item.Author.Name == name).ToListAsync();
            return commits;
        }
        
        public async Task AddSettingsAsync(Settings settings)
        {
            if (await _dbContext.Settings.AnyAsync()) await _dbContext.Settings.ExecuteDeleteAsync();
            await _dbContext.Settings.AddAsync(settings);
            await _dbContext.SaveChangesAsync();
        }
    }
}
