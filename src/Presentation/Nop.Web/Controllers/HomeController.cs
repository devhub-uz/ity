using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core.Infrastructure;
using Nop.Web.Components;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

public partial class HomeController : BasePublicController
{
    [SaveLastContinueShoppingPage]
    public virtual IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetMoreHomepageProducts(int pageNumber = 2, int pageSize = 12)
    {
        var component = EngineContext.Current.Resolve<HomepageProductsViewComponent>();
        var result = await component.InvokeAsync(null, pageNumber, pageSize) as ViewViewComponentResult;
        
        return View(result.ViewName, result.ViewData.Model);
    }
}