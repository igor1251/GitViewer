using System.Diagnostics;

using GitViewer.Client;
using GitViewer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;
        readonly GitHubOperator _operator = new();

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        [Route("authenticate")]
        public IActionResult Authenticate(string code)
        {
            _operator.SetOauthToken(code);
            return Ok($"Recieved code {code}");
        }

        [Route("")]
        [Route("repos")]
        public async Task<IActionResult> GetUserRepos()
        {
            var repoList = await _operator.GetRepos();
            return Ok(string.Join(";\n", repoList));
        }

        [Route("commits")]
        public async Task<OkObjectResult> GetUserCommits(string username, string reponame)
        {
            //пример вызова http://localhost:5063/commits?username=igor1251&reponame=Web
            var commits = await _operator.GetCommits(username, reponame);
            return Ok(string.Join(";\n", commits));
        }
    }
}