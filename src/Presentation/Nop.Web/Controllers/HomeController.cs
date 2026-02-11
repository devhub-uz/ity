using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

public partial class HomeController : BasePublicController
{
    protected readonly IAclService _aclService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingService _storeMappingService;

    public HomeController(
        IAclService aclService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStoreMappingService storeMappingService)
    {
        _aclService = aclService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _storeMappingService = storeMappingService;
    }

    [SaveLastContinueShoppingPage]
    public virtual IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public virtual async Task<IActionResult> GetMoreHomepageProducts(int pageNumber = 2, int pageSize = 12)
    {
        var allProducts = await (await _productService.GetAllProductsDisplayedOnHomepageAsync())
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            .Where(p => _productService.ProductIsAvailable(p))
            .Where(p => p.VisibleIndividually).ToListAsync();

        var totalCount = allProducts.Count;
        
        if (totalCount == 0)
            return Content("");

        var products = allProducts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, null)).ToList();
        
        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.HasMorePages = (pageNumber * pageSize) < totalCount;

        return View("Components/HomepageProducts/Default", model);
    }
}