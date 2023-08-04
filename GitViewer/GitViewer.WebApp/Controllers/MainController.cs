﻿using System.Diagnostics;

using GitViewer.Client;
using GitViewer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class MainController : Controller
    {
        readonly GitHubOperator _gitOperator;
        readonly ILogger<MainController> _logger;

        public MainController(GitHubOperator gitOperator, ILogger<MainController> logger)
        {
            _gitOperator = gitOperator;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //var reloginRequired = await _operator.ReloginRequired();
            //if (reloginRequired) return Redirect(_operator.GetLoginUrl());
            //else return RedirectToAction("GetUserRepos");
            return RedirectToAction("Index", "Commits");
        }

        [Route("authenticate")]
        public IActionResult Authenticate(string code)
        {
            _gitOperator.SetOauthToken(code);
            //return Ok($"Recieved code {code}");
            return RedirectToAction("Index", "Commits");
        }

        [Route("repos")]
        public async Task<IActionResult> GetUserRepos()
        {
            var repoList = await _gitOperator.GetRepos();
            return Ok(string.Join(";\n", repoList));
        }

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