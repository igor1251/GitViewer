using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Controllers
{
    public class CommitsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
