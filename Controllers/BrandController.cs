using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Repository;

namespace Shopping_Coffee.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            var brand = await _dataContext.Brands.FirstOrDefaultAsync(b => b.Slug == Slug);
            if (brand == null) return RedirectToAction("Index");

            var productsByBrand = await _dataContext.Products
                .Where(p => p.BrandId == brand.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(productsByBrand);
        }
    }
}
