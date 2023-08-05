using System.Diagnostics;

using GitViewer.GitStorage;
using GitViewer.GitStorage.Local;
using GitViewer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class AuthController : Controller
    {
        readonly GitStorageFasade _gitStorage;
        readonly ILogger<AuthController> _logger;

        public AuthController(GitStorageFasade gitStorage, ILogger<AuthController> logger)
        {
            _gitStorage = gitStorage;
            _logger = logger;
        }

        public IActionResult Index()
        {
            //var reloginRequired = await _operator.ReloginRequired();
            //if (reloginRequired) return Redirect(_operator.GetLoginUrl());
            //return Redirect(_gitStorage.GetLoginUrl());
            return RedirectToAction("Index", "Commits");
        }

        [Route("authenticate")]
        public IActionResult Authenticate(string code)
        {
            _gitStorage.SetOauthToken(code);
            _logger.LogInformation($"Recieved API-key {code}");
            return RedirectToAction("Index", "Commits");
        }

        //[Route("repos")]
        //public async Task<IActionResult> GetUserRepos()
        //{
        //    var repoList = await _gitLocalStorage.GetRepositoriesAsync();
        //    return Ok(string.Join(";\n", repoList.Select(item => item.Name)));
        //}

        //[Route("commits")]
        //public async Task<OkObjectResult> GetUserCommits(string username, string reponame)
        //{
        //    //пример вызова http://localhost:5063/commits?username=igor1251&reponame=Web
        //    var commits = await _operator.GetCommits(username, reponame);
        //    return Ok(string.Join(";\n", commits));
        //}

        //[Route("search")]
        //public async Task<IActionResult> SearchRepo(string owner, string repo)
        //{
        //    var repos = await _gitOperator.SearchOwnerRepo(owner, repo);
        //    return Ok(string.Join(";\n", repos));
        //}
    }
}