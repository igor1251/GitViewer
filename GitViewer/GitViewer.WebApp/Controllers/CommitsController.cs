using GitViewer.GitStorage;
using GitViewer.WebApp.Models;
using GitViewer.WebApp.Extensions;
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

        public async Task<IActionResult> Search()
        {
            if (!await _gitStorage.CheckRemoteStorageConfig())
                return RedirectToAction("Index", "Auth");
            return View(new CommitsViewModel());
        }

        async Task FillCommitsAsync(CommitsViewModel model, bool needSyncWithRemote = false)
        {
            if (model.Results.Any()) model.Results.Clear();

            var owner = model.Owner;
            var repo = model.Repo;
            var login = model.Login;

            if (needSyncWithRemote) await _gitStorage.FetchCommitsAsync(owner, repo, login);

            var searchResult = (await _gitStorage.GetCommitsAsync(owner, repo, login)).AsQueryable().GetPaged(1, 20);
            foreach (var item in searchResult.Results)
                model.Results.Add(new()
                {
                    Selected = false,
                    Id = item.Id,
                    Name = item.Name,
                    Author = item.Author?.Name,
                    Date = item.Date
                });
            model.TotalPages = searchResult.TotalPages;
            model.CurrentPage = searchResult.CurrentPage;
            model.RowCount = searchResult.RowCount;
            model.PageSize = searchResult.PageSize;
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
            
            return View(model);
        }
    }
}
