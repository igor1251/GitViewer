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

        #region add

        /// <summary>
        /// Добавляет коммит в БД
        /// </summary>
        /// <param name="commit">коммит для добавления</param>
        /// <returns>Task</returns>
        /// <exception cref="Exception">если коммит был NULL</exception>
        public async Task AddCommitAsync(Commit commit)
        {
            if (commit.Author == null) throw new Exception("author cant't be null");
            await _dbContext.Commits.AddAsync(commit);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Добавляет коммит в БД
        /// </summary>
        /// <param name="commits">список коммитов для добавления</param>
        /// <returns>Task</returns>
        /// <exception cref="Exception">если репозиторий или автор были NULL</exception>
        public async Task AddCommitAsync(List<Commit> commits)
        {
            foreach (var commit in commits)
            {
                commit.Repository = await AddRepositoryAsync(commit.Repository ?? throw new Exception("Repository can't be null"));
                commit.Author = await AddUserAsync(commit.Author ?? throw new Exception("Author can't be null"));
            }

            await _dbContext.Commits.AddRangeAsync(commits);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Добавляет пользователя в БД
        /// </summary>
        /// <param name="user">пользователь</param>
        /// <returns>добавленный пользователь из БД</returns>
        async Task<User> AddUserAsync(User user)
        {
            if (await _dbContext.Users.AnyAsync(item => item.Name == user.Name))
                return await _dbContext.Users.FirstAsync(item => item.Name == user.Name);

            var addedEntity = await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return addedEntity.Entity;
        }

        /// <summary>
        /// Добавляет репозиторий в БД
        /// </summary>
        /// <param name="repository">репозиторий</param>
        /// <returns>добавленный репозиторий из БД</returns>
        /// <exception cref="Exception">если owner NULL</exception>
        async Task<Repository> AddRepositoryAsync(Repository repository)
        {
            repository.Owner = await AddUserAsync(repository.Owner ?? throw new Exception("Owner can't be null"));
            
            if (await _dbContext.Repositories.AnyAsync(item => item.RemoteId == repository.RemoteId))
                return await _dbContext.Repositories.FirstAsync(item => item.RemoteId == repository.RemoteId);
            
            var addedEntity = await _dbContext.Repositories.AddAsync(repository);
            await _dbContext.SaveChangesAsync();
            
            return addedEntity.Entity;
        }

        /// <summary>
        /// Добавляет конфигурацию подключения к API
        /// </summary>
        /// <param name="config">конфигурация</param>
        /// <returns>Task</returns>
        public async Task AddRemoteStorageConfigAsync(RemoteStorageConfig config)
        {
            await _dbContext.RemoteStorageConfigs.ExecuteDeleteAsync();
            await _dbContext.RemoteStorageConfigs.AddAsync(config);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region get

        /// <summary>
        /// Получает коммиты из локальной БД
        /// </summary>
        /// <param name="owner">владелец репозитория</param>
        /// <param name="repo">репозиторий</param>
        /// <param name="name">автор коммита</param>
        /// <returns>список коммитов</returns>
        public async Task<List<Commit>> GetCommitsAsync(string owner, string repo, string name)
        {
            var commits = await _dbContext.Commits.Include(c => c.Author).Include(c => c.Repository).Where(item =>
                item.Repository != null && item.Repository.Owner != null && item.Repository.Owner.Name == owner &&
                item.Repository.Name == repo &&
                item.Author != null && item.Author.Name == name).ToListAsync();
            return commits ?? new();
        }

        /// <summary>
        /// Загружает конфигурацию подключения к удаленному репозиторию
        /// </summary>
        /// <returns>конфигурация подключения</returns>
        public async Task<RemoteStorageConfig?> GetRemoteStorageConfigAsync() =>
            await _dbContext.RemoteStorageConfigs.FirstOrDefaultAsync();

        #endregion

        #region delete/update

        /// <summary>
        /// Обновляет конфигурацию подключения к удаленному репозиторию
        /// </summary>
        /// <param name="config">новая конфигурация</param>
        /// <returns>Task</returns>
        public async Task UpdateRemoteStorageConfigAsync(RemoteStorageConfig config)
        {
            var currenctConfig = await _dbContext.RemoteStorageConfigs.FirstOrDefaultAsync();
            if (currenctConfig != null)
            {
                currenctConfig.Login = config.Login;
                currenctConfig.ClientSecret = config.ClientSecret;
                currenctConfig.Token = config.Token;
                currenctConfig.ClientId = config.ClientId;
            }
            else await _dbContext.RemoteStorageConfigs.AddAsync(config);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Очищает базу коммитов
        /// </summary>
        /// <returns>Task</returns>
        public async Task ClearCommitsAsync()
        {
            _dbContext.Commits.RemoveRange(_dbContext.Commits);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет коммиты
        /// </summary>
        /// <param name="commitsIds">ID коммитов для удаления</param>
        /// <returns></returns>
        public async Task DeleteCommitsAsync(List<long> commitsIds)
        {
            _dbContext.Commits.RemoveRange(_dbContext.Commits.Where(item => commitsIds.Contains(item.Id)));
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
