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


        string _repo = string.Empty;
        string _owner = string.Empty;
        string _login = string.Empty;

        int _pageSize = 20;
        int _rowCount = 0;
        int _pageCount = 0;
        int _currentPage = 0;

        public GitStorageFasade(GitLocalStorage localStorage, GitRemoteStorage remoteStorage, ILogger<GitStorageFasade> logger)
        {
            _localStorage = localStorage;
            _remoteStorage = remoteStorage;
            _logger = logger;
        }

        public void SetPageSize(int pageSize = 20) => _pageSize = pageSize;

        public void SetSearchParameters(string owner, string repo, string login)
        {
            _owner = owner;
            _repo = repo;
            _login = login;
        }

        public (string owner, string repo, string login) GetSearchParameters() => 
            new(_owner, _repo, _login);

        public (int currentPage, int pageCount, int pageSize) GetPaginationInfo() =>
            new(_currentPage, _pageCount, _pageSize);

        public async Task<List<Commit>> SearchCommitsAsync(int page = 1)
        {
            var result = await GetCommitsAsync(_owner, _repo, _login);

            _rowCount = result.Count;
            _pageCount = (int)Math.Ceiling((double)(_rowCount / _pageSize));
            _currentPage = page > _pageCount ? 1 : page;

            var skip = (_currentPage - 1) * _pageSize;

            return result.Skip(skip).Take(_pageSize).ToList();
        }

        public async Task<string> GetLoginUrlAsync()
        {
            _remoteStorage.SetConfig(await _localStorage.GetRemoteStorageConfigAsync() ?? throw new Exception("GitHubAPI config was null"));
            return _remoteStorage.GetLoginUrl();
        }

        public async Task<bool> CheckRemoteStorageConfig()
        {
            var config = await _localStorage.GetRemoteStorageConfigAsync();
            return !config?.TokenNotSet ?? false;
        }

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
        public async Task FetchCommitsAsync(string owner, string repo, string login)
        {
            _logger.LogInformation($"Search owner = {owner} repo = {repo} login = {login} in remoteStorage");
            
            SetSearchParameters(owner, repo, login);
            
            var remoteCommits = await _remoteStorage.GetCommitsAsync(owner, repo, login);
            if (remoteCommits?.Any() ?? false)
            {
                _logger.LogInformation($"Done. Adding search result in localStorage");
                await _localStorage.ClearCommitsAsync();
                await _localStorage.AddCommitAsync(remoteCommits);
            }
        }

        public async Task AddRemoteStorageConfig(RemoteStorageConfig config) => 
            await _localStorage.AddRemoteStorageConfigAsync(config);

        public async Task UpdateRemoteStorageConfig(RemoteStorageConfig config)
        {
            await _localStorage.UpdateRemoteStorageConfigAsync(config);
            _remoteStorage.SetConfig(await _localStorage.GetRemoteStorageConfigAsync() ?? throw new Exception("GitHubAPI config was null"));
        }

        public async Task<RemoteStorageConfig?> GetRemoteStorageConfigAsync() =>
            await _localStorage.GetRemoteStorageConfigAsync();

        public async Task DeleteCommitsAsync(List<long> commitsIds) =>
            await _localStorage.DeleteCommitsAsync(commitsIds);
    }
}
