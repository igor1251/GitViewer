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

        #region old

        //async Task FillCommitsAsync(CommitsViewModel model, bool needSyncWithRemote = false)
        //{
        //    if (model.Results.Any()) model.Results.Clear();

        //    var owner = model.Owner;
        //    var repo = model.Repo;
        //    var login = model.Login;

        //    if (needSyncWithRemote) await _gitStorage.FetchCommitsAsync(owner, repo, login);

        //    var searchResult = await _gitStorage.GetCommitsAsync(owner, repo, login);
        //    foreach (var item in searchResult)
        //        model.Results.Add(new()
        //        {
        //            Selected = false,
        //            Id = item.Id,
        //            Name = item.Name,
        //            Author = item.Author?.Name,
        //            Date = item.Date
        //        });
        //}

        [HttpPost]
        public IActionResult TestAJAX(string text)
        {
            return PartialView("_TestPartialView", text);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Search(CommitsViewModel model, string action)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        switch (action)
        //        {
        //            case "search":
        //                _gitStorage.SetSearchParameters(model.Owner, model.Repo, model.Login);
        //                break;
        //            case "fetch":
        //                await FillCommitsAsync(model, true);
        //                break;
        //            case "delete":
        //                var selectedIds = model.GetSelectedIds();
        //                if (selectedIds.Any())
        //                {
        //                    await _gitStorage.DeleteCommitsAsync(selectedIds);
        //                }
        //                break;
        //        }
        //    }

        //    return RedirectToAction("GoToPage");
        //}

        #endregion

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!await _gitStorage.CheckRemoteStorageConfig())
                return RedirectToAction("Index", "Auth");
            return View("Search", new CommitsViewModel());
        }

        [HttpPost]
        public IActionResult Search(CommitsViewModel model)
        {
            if (!ModelState.IsValid) return View("Search", model);

            _gitStorage.SetSearchParameters(model.Owner, model.Repo, model.Login);
            return RedirectToAction(nameof(GoToPage));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(List<long> ids)
        {
            var model = new CommitsViewModel();
            if (ModelState.IsValid)
            {
                if (ids.Any())
                    await _gitStorage.DeleteCommitsAsync(ids);

                var response = await _gitStorage.SearchCommitsAsync();
                var (owner, repo, login) = _gitStorage.GetSearchParameters();
                var (currentPage, pageCount, pageSize) = _gitStorage.GetPaginationInfo();

                model.Login = login;
                model.Owner = owner;
                model.Repo = repo;

                model.CurrentPage = currentPage;
                model.PageSize = pageSize;
                model.PageCount = pageCount;

                model.Results = response.Select(item => new SelectableCommitEditorViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Author = item.Author?.Name,
                    Date = item.Date,
                    Selected = false,
                }).ToList();
                
            }
            
            return PartialView("_CommitsPartialView", model);
        }

        [HttpPost]
        public async Task<IActionResult> Fetch(CommitsViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _gitStorage.FetchCommitsAsync(model.Owner, model.Repo, model.Login);

                var response = await _gitStorage.SearchCommitsAsync();
                var (currentPage, pageCount, pageSize) = _gitStorage.GetPaginationInfo();
                model = new CommitsViewModel()
                {
                    Login = model.Owner,
                    Owner = model.Login,
                    Repo = model.Repo,

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
            }
            return RedirectToAction(nameof(GoToPage), model);
        }
    }
}
