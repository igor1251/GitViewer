using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GitViewer.GitStorage.Local;
using GitViewer.GitStorage.Models;
using GitViewer.GitStorage.Remote;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GitViewer.GitStorage
{
    public class GitStorageFasade
    {
        readonly GitLocalStorage _localStorage;
        readonly GitRemoteStorage _remoteStorage;
        readonly ILogger<GitStorageFasade> _logger;

        public GitStorageFasade(GitLocalStorage localStorage, GitRemoteStorage remoteStorage, ILogger<GitStorageFasade> logger)
        {
            _localStorage = localStorage;
            _remoteStorage = remoteStorage;
            _logger = logger;
        }

        /// <summary>
        /// Настройка токена для связи с GitHub API
        /// </summary>
        /// <param name="token">токен, полученный при авторизации</param>
        public void SetOauthToken(string token) => _remoteStorage.SetOauthToken(token);
        
        public string GetLoginUrl() => _remoteStorage.GetLoginUrl();

        /// <summary>
        /// Поиск коммитов
        /// </summary>
        /// <param name="owner">владелец репозитория</param>
        /// <param name="repo">имя репозитория</param>
        /// <param name="login">автор коммита</param>
        /// <returns>список коммитов</returns>
        public async Task<List<Commit>> GetCommitsAsync(string owner, string repo, string login)
        {
            _logger.LogInformation($"Search owner = {owner} repo = {repo} login = {login} in localStorage");
            var response = await _localStorage.GetCommitsAsync(owner, repo, login);
            _logger.LogInformation($"Done");
            if (!response.Any())
            {
                _logger.LogInformation($"Search result is empty. Syncing with remoteStorage");
                await FetchCommitsAsync(owner, repo, login);
                _logger.LogInformation($"Done");
                return await GetCommitsAsync(owner, repo, login);
            }
            else return response;
        }

        /// <summary>
        /// Загружает список коммитов
        /// </summary>
        /// <param name="owner">владелец репозитория, в котором ищем коммиты</param>
        /// <param name="repo">имя репозитория, в котором ищем коммиты</param>
        /// <param name="login">автор коммита</param>
        /// <returns>task для ожидания</returns>
        public async Task FetchCommitsAsync(string owner, string repo, string? login = null)
        {
            _logger.LogInformation($"Search owner = {owner} repo = {repo} login = {login} in remoteStorage");
            var remoteCommits = await _remoteStorage.GetCommitsAsync(owner, repo, login);
            _logger.LogInformation($"Done. Adding search result in localStorage");
            await _localStorage.AddCommitAsync(remoteCommits);
        }

        public async Task AddRemoteStorageConfig(RemoteStorageConfig config) => 
            await _localStorage.AddRemoteStorageConfigAsync(config);

        public async Task<RemoteStorageConfig?> GetRemoteStorageConfigAsync() =>
            await _localStorage.GetRemoteStorageConfigAsync();
    }
}
