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

        public async Task<IActionResult> Index()
        {
            if (!await _gitStorage.CheckRemoteStorageConfig())
                return RedirectToAction("Index", "Auth");
            return View(new CommitsViewModel());
        }

        async Task FillCommitsAsync(CommitsViewModel model, bool needSyncWithRemote = false)
        {
            if (model.Commits.Any()) model.Commits.Clear();

            var owner = model.Owner;
            var repo = model.Repo;
            var login = model.Login;

            if (needSyncWithRemote) await _gitStorage.FetchCommitsAsync(owner, repo, login);

            var searchResult = await _gitStorage.GetCommitsAsync(owner, repo, login);
            foreach (var item in searchResult)
                model.Commits.Add(new()
                {
                    Selected = false,
                    Id = item.Id,
                    Name = item.Name,
                    Author = item.Author?.Name,
                    Date = item.Date
                });
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
                        await FillCommitsAsync(model);
                        break;
                    case "fetch":
                        await FillCommitsAsync(model, true);
                        break;
                    case "delete":
                        var selectedIds = model.GetSelectedIds();
                        if (selectedIds.Any())
                        {
                            await _gitStorage.DeleteCommitsAsync(selectedIds);
                            await FillCommitsAsync(model);
                        }
                        break;
                }
            }
            
            return View("Index", model);
        }
    }
}
