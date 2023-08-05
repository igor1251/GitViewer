using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitViewer.GitStorage.Models;

using Octokit;

namespace GitViewer.GitStorage.Remote
{
    public class GitRemoteStorage
    {
        readonly GitHubClient _client = new(new ProductHeaderValue(ClientConfig.AppName));

        public string GetLoginUrl()
        {
            var request = new OauthLoginRequest(ClientConfig.ClientID)
            {
                Scopes = { "user", "repo", "workflow" },
            };
            var loginUrl = _client.Oauth.GetGitHubLoginUrl(request);
            var link = loginUrl.ToString();

            return link;
        }

        public async Task<bool> ReloginRequired()
        {
            var token = _client.Credentials.GetToken();
            if (string.IsNullOrEmpty(token))
                return true;
            var result = await _client.Authorization.CheckApplicationAuthentication(ClientConfig.ClientID, token);
            return result == null;
        }

        public void SetOauthToken(string token)
        {
            _client.Credentials = new(token);
        }

        public async Task<List<Models.Repository>> GetRepositoriesAsync(string owner)
        {
            var remoteRepos = await _client.Repository.GetAllForUser(owner);
            var repos = new List<Models.Repository>();
            
            foreach (var item in remoteRepos)
            {
                var localRepo = new Models.Repository
                {
                    Name = item.Name,
                    Owner = new Models.User() { Name = item.Owner.Login },
                    Commits = await GetCommitsAsync(owner, item.Name)
                };
                repos.Add(localRepo);
            }
            
            return repos;
        }

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
