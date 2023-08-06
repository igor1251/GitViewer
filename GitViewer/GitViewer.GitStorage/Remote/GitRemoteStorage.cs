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

        public void SetOauthToken(string token)
        {
            _client.Credentials = new(token);
        }

        public void SetRemoteStorageConfig(RemoteStorageConfig config) => 
            _config = config;

        public async Task<List<Models.Commit>> GetCommitsAsync(string owner, string repo, string? login = null)
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
    }
}
