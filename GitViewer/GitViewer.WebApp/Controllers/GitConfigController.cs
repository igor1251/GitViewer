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

        public GitConfigController(GitStorageFasade gitStorage, ILogger<GitConfigController> logger)
        {
            _gitStorage = gitStorage;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new GitConfigViewModel());
        }

        public async Task<IActionResult> Save(GitConfigViewModel model, string action)
        {
            if (ModelState.IsValid)
            {
                switch (action)
                {
                    case "save":
                        var config = new RemoteStorageConfig()
                        {
                            Login = model.Login,
                            Password = model.Password,
                            ClientId = model.ClientId,
                            ClientSecret = model.ClientSecret,
                        };
                        await _gitStorage.AddRemoteStorageConfig(config);
                        break;
                }
            }
            return View("Index", model);
        }
    }
}
