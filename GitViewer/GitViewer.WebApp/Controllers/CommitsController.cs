using System.Drawing.Printing;

using GitViewer.GitStorage;
using GitViewer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

using NuGet.Protocol.Plugins;

namespace GitViewer.WebApp.Controllers
{
    public class CommitsController : Controller
    {
        readonly GitStorageFasade _gitStorage;
        readonly ILogger<CommitsController> _logger;
        readonly CommitsViewModel _viewModel;

        public CommitsController(GitStorageFasade gitStorage, ILogger<CommitsController> logger, CommitsViewModel viewModel)
        {
            _gitStorage = gitStorage;
            _logger = logger;
            _viewModel = viewModel;
        }

        async Task LoadCommitsAsync(string owner, string repo, string login, int page = 1)
        {
            var response = await _gitStorage.SearchCommitsAsync(page);
            var (currentPage, pageCount, pageSize) = _gitStorage.GetPaginationInfo();

            _viewModel.Login = login;
            _viewModel.Owner = owner;
            _viewModel.Repo = repo;

            _viewModel.CurrentPage = currentPage;
            _viewModel.PageSize = pageSize;
            _viewModel.PageCount = pageCount;

            if (_viewModel.Results.Any()) _viewModel.Results.Clear();

            _viewModel.Results.AddRange(response.Select(item => new SelectableCommitEditorViewModel()
            {
                Id = item.Id,
                Name = item.Name,
                Author = item.Author?.Name,
                Date = item.Date,
                Selected = false,
            }));
        }

        [HttpGet]
        public async Task<IActionResult> GoToPage(int page = 1)
        {

            var (owner, repo, login) = _gitStorage.GetSearchParameters();
            await LoadCommitsAsync(owner, repo, login, page);

            return View("Search", _viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!await _gitStorage.CheckRemoteStorageConfig())
                return RedirectToAction("Index", "Auth");
            
            return View("Search", _viewModel);
        }

        [HttpGet]
        public IActionResult Search(string owner, string repo, string login)
        {
            if (!ModelState.IsValid) 
                return View("Search", _viewModel);
            
            _gitStorage.SetSearchParameters(owner, repo, login);
            
            return RedirectToAction(nameof(GoToPage));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(List<long> ids)
        {
            if (ModelState.IsValid)
            {
                if (ids.Any())
                    await _gitStorage.DeleteCommitsAsync(ids);

                var (owner, repo, login) = _gitStorage.GetSearchParameters();
                await LoadCommitsAsync(owner, repo, login);
            }
            
            return PartialView("_CommitsPartialView", _viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Fetch(string owner, string repo, string login)
        {
            if (ModelState.IsValid)
            {
                await _gitStorage.FetchCommitsAsync(owner, repo, login);
                await LoadCommitsAsync(owner, repo, login);
            }

            return RedirectToAction(nameof(GoToPage), _viewModel);
        }
    }
}
