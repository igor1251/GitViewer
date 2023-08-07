using GitViewer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GitViewer.WebApp.Components
{
    public class PagerViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(PagedResultBase result) =>
            Task.FromResult((IViewComponentResult)View("Default", result));
    }
}
