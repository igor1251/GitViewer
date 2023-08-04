using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace GitViewer.Client
{
    public class GitHubOperator
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

        public async Task<List<string>> GetRepos()
        {
            var repos = await _client.Repository.GetAllForUser(ClientConfig.UserName);
            //var commits = (await _client.Repository.Commit.GetAll(repos[0].Id)).Select(item => item.Commit.Message).ToList();
            return repos.Select(item => item.FullName).ToList();
        }

        public async Task<List<string>> GetCommits(string username, string reponame)
        {
            var repo = await _client.Repository.Get(ClientConfig.UserName, reponame);
            var commits = (await _client.Repository.Commit.GetAll(repo.Id)).Where(item => item.Commit.Author.Name == username).Select(item => item.Commit.Message).ToList();
            return commits;
        }

        public async Task<List<(string timestamp, string author, string comment, string description)>> SearchOwnerRepo(string owner, string repo, string login)
        {
            var result = await _client.Search.SearchRepo(new(repo)
            {
                User = owner,
            });

            var response = new List<(string timestamp, string author, string comment, string description)>();

            if (result != null)
            {
                foreach (var item in result.Items)
                {
                    var commits = await GetCommits(login, item.Name);
                    foreach (var commit in commits)
                        response.Add(new(DateTime.Now.ToString(), login, commit, "test"));
                }
            }
            
            return response;
        }
    }
}
