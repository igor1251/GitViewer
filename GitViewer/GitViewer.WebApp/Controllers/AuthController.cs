using System.Diagnostics;

using GitViewer.GitStorage;
using GitViewer.GitStorage.Local;
using GitViewer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class AuthController : Controller
    {
        readonly GitStorageFasade _gitStorage;
        readonly ILogger<AuthController> _logger;

        public AuthController(GitStorageFasade gitStorage, ILogger<AuthController> logger)
        {
            _gitStorage = gitStorage;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var remoteStorageConfig = await _gitStorage.GetRemoteStorageConfigAsync();

            if (remoteStorageConfig != null)
            {
                if (remoteStorageConfig.TokenNotSet) return Redirect(await _gitStorage.GetLoginUrlAsync());
                else return RedirectToAction("Index", "Commits");
            }

            return RedirectToAction("Index", "GitConfig");
        }

        [Route("authenticate")]
        public async Task<IActionResult> Authenticate(string code)
        {
            var config = await _gitStorage.GetRemoteStorageConfigAsync() ?? throw new Exception("invalid gitApi configuration");
            config.Token = code;
            await _gitStorage.UpdateRemoteStorageConfig(config);
            
            _logger.LogInformation($"Recieved API-key {code}");
            
            return RedirectToAction("Index", "Commits");
        }
    }
}