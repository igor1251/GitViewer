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

        /// <summary>
        /// Устанавливает размер страницы
        /// </summary>
        /// <param name="pageSize">размер страницы</param>
        public void SetPageSize(int pageSize = 20) => _pageSize = pageSize;

        /// <summary>
        /// Устанавливает параметры поиска коммитов
        /// </summary>
        /// <param name="owner">владелец репозитория</param>
        /// <param name="repo">репозиторий</param>
        /// <param name="login">создатель коммита</param>
        public void SetSearchParameters(string owner, string repo, string login)
        {
            _owner = owner;
            _repo = repo;
            _login = login;
        }

        /// <summary>
        /// Отдает установленные параметры поиска коммитов
        /// </summary>
        /// <returns>кортеж с указанными параметрами</returns>
        public (string owner, string repo, string login) GetSearchParameters() => 
            new(_owner, _repo, _login);

        /// <summary>
        /// Отдает информацию о размерах страницы, номере текущей и их кол-ве
        /// </summary>
        /// <returns>кортеж с указанными данными</returns>
        public (int currentPage, int pageCount, int pageSize) GetPaginationInfo() =>
            new(_currentPage, _pageCount, _pageSize);

        /// <summary>
        /// Поиск коммитов в соответствие с установленными данными
        /// </summary>
        /// <param name="page">страница выдачи</param>
        /// <returns>список коммитов на указанной странице</returns>
        public async Task<List<Commit>> SearchCommitsAsync(int page = 1)
        {
            var result = await GetCommitsAsync(_owner, _repo, _login);

            _rowCount = result.Count;
            _pageCount = (int)Math.Ceiling((double)(_rowCount / _pageSize));
            _currentPage = page > _pageCount ? 1 : page;

            var skip = (_currentPage - 1) * _pageSize;

            return result.Skip(skip).Take(_pageSize).ToList();
        }

        /// <summary>
        /// Получает ссылку на страницу авторизации GitHub
        /// </summary>
        /// <returns>ссылка</returns>
        /// <exception cref="Exception">если config не заполнен</exception>
        public async Task<string> GetLoginUrlAsync()
        {
            _remoteStorage.SetConfig(await _localStorage.GetRemoteStorageConfigAsync() ?? throw new Exception("GitHubAPI config was null"));
            return _remoteStorage.GetLoginUrl();
        }

        /// <summary>
        /// Настроена ли конфигурация подключения к API
        /// </summary>
        /// <returns>false - если нет</returns>
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
            var response = await _localStorage.GetCommitsAsync(owner, repo, login);
            if (!response.Any())
            {
                var result = await FetchCommitsAsync(owner, repo, login);
                if (result) return await GetCommitsAsync(owner, repo, login);
            }
            return response ?? new();
        }

        /// <summary>
        /// Загружает список коммитов
        /// </summary>
        /// <param name="owner">владелец репозитория, в котором ищем коммиты</param>
        /// <param name="repo">имя репозитория, в котором ищем коммиты</param>
        /// <param name="login">автор коммита</param>
        /// <returns>task для ожидания</returns>
        public async Task<bool> FetchCommitsAsync(string owner, string repo, string login)
        {            
            SetSearchParameters(owner, repo, login);
            
            var remoteCommits = await _remoteStorage.GetCommitsAsync(owner, repo, login);
            if (remoteCommits?.Any() ?? false)
            {
                await _localStorage.ClearCommitsAsync();
                await _localStorage.AddCommitAsync(remoteCommits);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Добавляет конфигурацию подключения к API
        /// </summary>
        /// <param name="config">конфигурация</param>
        /// <returns>Task</returns>
        public async Task AddRemoteStorageConfig(RemoteStorageConfig config) => 
            await _localStorage.AddRemoteStorageConfigAsync(config);

        /// <summary>
        /// Обновляет конфигурацию подключения к API
        /// </summary>
        /// <param name="config">новая конфигурация</param>
        /// <returns>Task</returns>
        /// <exception cref="Exception">если config null</exception>
        public async Task UpdateRemoteStorageConfig(RemoteStorageConfig config)
        {
            await _localStorage.UpdateRemoteStorageConfigAsync(config);
            _remoteStorage.SetConfig(await _localStorage.GetRemoteStorageConfigAsync() ?? throw new Exception("GitHubAPI config was null"));
        }

        /// <summary>
        /// Получает конфигурацию подкелючения к API из БД
        /// </summary>
        /// <returns>конфигурация</returns>
        public async Task<RemoteStorageConfig?> GetRemoteStorageConfigAsync() =>
            await _localStorage.GetRemoteStorageConfigAsync();

        /// <summary>
        /// Удаляет коммиты из БД
        /// </summary>
        /// <param name="commitsIds">ID коммитов для удаления</param>
        /// <returns>Task</returns>
        public async Task DeleteCommitsAsync(List<long> commitsIds) =>
            await _localStorage.DeleteCommitsAsync(commitsIds);

        /// <summary>
        /// Настраивает конфигурацию соединения с API GitHub
        /// </summary>
        /// <returns>task</returns>
        public async Task PrepareAsync()
        {
            var config = await _localStorage.GetRemoteStorageConfigAsync();
            _remoteStorage.SetConfig(config ?? new());
        }
    }
}
