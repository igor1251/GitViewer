using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitViewer.GitStorage.Models;

using Octokit;

namespace GitViewer.GitStorage.Remote
{
    public class GitRemoteStorage
    {
        readonly GitHubClient _client = new(new ProductHeaderValue(Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName ?? "yeah")));

        RemoteStorageConfig? _config;

        string CLIENT_ID => _config?.ClientId ?? throw new Exception("ClientId can't be null");

        /// <summary>
        /// Настраивает config для подключения к API
        /// </summary>
        /// <param name="config">конфигурация</param>
        public void SetConfig(RemoteStorageConfig config)
        {
            _config = config;
            if (_config != null)
            {
                if (!_config.OnlyTokenAuthAllowed)
                    _client.Credentials = new(_config.Login, _config.Password);
                else if (!_config.TokenNotSet) 
                    _client.Credentials = new(_config.Token);
            }
        }

        /// <summary>
        /// Получает ссылку на страницу авторизации GitHub
        /// </summary>
        /// <returns>ссылка</returns>
        public string GetLoginUrl()
        {
            var request = new OauthLoginRequest(CLIENT_ID)
            {
                Scopes = { "user", "repo", "workflow" },
            };
            var loginUrl = _client.Oauth.GetGitHubLoginUrl(request);
            var link = loginUrl.ToString();

            return link;
        }

        /// <summary>
        /// Устанавливает токен для обращения к ресурсам GitHub
        /// </summary>
        /// <param name="token">токен</param>
        public void SetOauthToken(string token)
        {
            _client.Credentials = new(token);
        }

        /// <summary>
        /// Получает список коммитов по указанным критериям
        /// </summary>
        /// <param name="owner">владелец репозитория</param>
        /// <param name="repo">репозиторий</param>
        /// <param name="login">создатель коммита</param>
        /// <returns>список коммитов</returns>
        public async Task<List<Models.Commit>> GetCommitsAsync(string owner, string repo, string? login = null)
        {
            try
            {
                var remoteRepo = await _client.Repository.Get(owner, repo);

                var request = new CommitRequest();

                if (!string.IsNullOrEmpty(login))
                    request.Author = login;

                var commits = (await _client.Repository.Commit.GetAll(remoteRepo.Id, request)).Select(item => new Models.Commit()
                {
                    Name = item.Commit.Message,
                    Date = item.Commit.Author.Date.DateTime,
                    Author = new Models.User() { Name = item.Commit.Author.Name },
                    Repository = new Models.Repository()
                    {
                        RemoteId = remoteRepo.Id,
                        Name = remoteRepo.Name,
                        Owner = new Models.User() { Name = remoteRepo.Owner.Login },
                    }
                }).ToList();

                return commits;
            }
            catch (NotFoundException)
            {
                return new();
            }
        }
    }
}
