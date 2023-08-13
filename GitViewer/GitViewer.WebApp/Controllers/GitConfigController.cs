using GitViewer.GitStorage;
using GitViewer.GitStorage.Models;
using GitViewer.WebApp.Models;

using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class GitConfigController : Controller
    {
        readonly GitStorageFasade _gitStorage;
        readonly ILogger<GitConfigController> _logger;
        readonly GitConfigViewModel _viewModel;

        public GitConfigController(GitStorageFasade gitStorage, ILogger<GitConfigController> logger, GitConfigViewModel viewModel)
        {
            _gitStorage = gitStorage;
            _logger = logger;
            _viewModel = viewModel;
        }

        public async Task<IActionResult> Index()
        {
            var currentConfig = await _gitStorage.GetRemoteStorageConfigAsync();
            if (currentConfig != null)
            {
                _viewModel.Login = currentConfig.Login;
                _viewModel.Password = currentConfig.Password;
                _viewModel.ClientSecret = currentConfig.ClientSecret;
                _viewModel.ClientId = currentConfig.ClientId;
            }
            return View(_viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(string login, string password, string clientId, string clientSecret)
        {
            if (ModelState.IsValid)
            {
                await _gitStorage.UpdateRemoteStorageConfig(new()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Login = login,
                    Password = password,
                });
                
                return RedirectToAction("Index", "Auth");
            }
            
            return View("Index", _viewModel);
        }
    }
}
