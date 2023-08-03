using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitViewer.GitHubClient.Configs;
using Octokit;

namespace GitViewer.GitHubClient.Operators
{
    public class GitHubOperator
    {
        readonly Octokit.GitHubClient _client = new(new ProductHeaderValue(ClientConfig.AppName));

        public string GetLoginUrl()
        {
            var request = new OauthLoginRequest(ClientConfig.ClientID)
            {
                Scopes = { "user", "repo", "workflow" },
                //Login = "Ocakypa"
            };
            var loginUrl = _client.Oauth.GetGitHubLoginUrl(request);
            var link = loginUrl.ToString();

            return link;
        }

        public void SetOauthToken(string token)
        {
            _client.Credentials = new(token);
        }

        public async Task<List<string>> GetRepos(string userName)
        {
            var repos = await _client.Repository.GetAllForUser(userName);
            var commits = (await _client.Repository.Commit.GetAll(repos[0].Id)).Select(item => item.Commit.Message).ToList();
            //return repos.Select(item => item.FullName).ToList();
            return commits;
        }
    }
}
