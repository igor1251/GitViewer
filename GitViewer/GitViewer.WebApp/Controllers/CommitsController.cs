using GitViewer.GitStorage;
using GitViewer.WebApp.Models;

using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class CommitsController : Controller
    {
        readonly GitStorageFasade _gitStorage;
        readonly ILogger<CommitsController> _logger;

        public CommitsController(GitStorageFasade gitStorage, ILogger<CommitsController> logger)
        {
            _gitStorage = gitStorage;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new CommitsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(CommitsViewModel model, string action)
        {
            if (ModelState.IsValid)
            {
                switch (action)
                {
                    case "search":
                        var owner = model.Owner;
                        var repo = model.Repo;
                        var login = model.Login;
                        var searchResult = await _gitStorage.GetCommitsAsync(owner, repo, login);
                        foreach (var item in searchResult)
                            model.Commits.Add(item);
                        break;
                }
            }
            return View("Index", model);
        }
    }
}
