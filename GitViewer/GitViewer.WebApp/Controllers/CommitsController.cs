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

            var searchResult = await _gitStorage.GetCommitsAsync(owner, repo, login);
            foreach (var item in searchResult)
                model.Results.Add(new()
                {
                    Selected = false,
                    Id = item.Id,
                    Name = item.Name,
                    Author = item.Author?.Name,
                    Date = item.Date
                });
        }

        [HttpPost]
        public IActionResult TestAJAX(string text)
        {
            return PartialView("_TestPartialView", text);
        }

        public async Task<IActionResult> GoToPage(int page = 1)
        {
            var response = await _gitStorage.SearchCommitsAsync(page);
            var (owner, repo, login) = _gitStorage.GetSearchParameters();
            var (currentPage, pageCount, pageSize) = _gitStorage.GetPaginationInfo();
            var model = new CommitsViewModel()
            {
                Login = login,
                Owner = owner,
                Repo = repo,
                
                CurrentPage = currentPage,
                PageSize = pageSize,
                PageCount = pageCount,
                
                Results = response.Select(item => new SelectableCommitEditorViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Author = item.Author?.Name,
                    Date = item.Date,
                    Selected = false,
                }).ToList(),
            };
            return View("Search", model);
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
                        _gitStorage.SetSearchParameters(model.Owner, model.Repo, model.Login);
                        break;
                    case "fetch":
                        await FillCommitsAsync(model, true);
                        break;
                    case "delete":
                        var selectedIds = model.GetSelectedIds();
                        if (selectedIds.Any())
                        {
                            await _gitStorage.DeleteCommitsAsync(selectedIds);
                        }
                        break;
                }
            }

            return RedirectToAction("GoToPage");
        }
    }
}
