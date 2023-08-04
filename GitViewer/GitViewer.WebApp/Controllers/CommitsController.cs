using GitViewer.Client;
using GitViewer.WebApp.Models;

using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class CommitsController : Controller
    {
        readonly GitHubOperator _gitOperator;
        readonly ILogger<CommitsController> _logger;

        public CommitsController(GitHubOperator gitOperator, ILogger<CommitsController> logger)
        {
            _gitOperator = gitOperator;
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
                    case "SearchRepo":
                        var owner = model.Owner;
                        var repo = model.Repo;
                        var login = model.Login;
                        var searchResult = await _gitOperator.SearchOwnerRepo(owner, repo, login);
                        foreach (var item in searchResult)
                            model.Commits.Add(item);
                        break;
                    case "FilterByUser":

                        break;
                }
            }
            return View("Index", model);
        }
    }
}
