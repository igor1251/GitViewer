using System.Diagnostics;

using GitViewer.GitHubClient.Configs;
using GitViewer.GitHubClient.Operators;
using GitViewer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly GitHubOperator _operator = new();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Redirect(_operator.GetLoginUrl());
        }

        [Route("authenticate")]
        public IActionResult Authenticate(string code)
        {
            _operator.SetOauthToken(code);
            return Ok($"Recieved code {code}");
        }

        [Route("repos")]
        public async Task<IActionResult> GetUserRepos()
        {
            var repoList = await _operator.GetRepos(ClientConfig.UserName);
            return Ok(string.Join(";\n", repoList));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}