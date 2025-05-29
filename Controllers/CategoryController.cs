using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Models;
using Shopping_Coffee.Repository;

namespace Shopping_Coffee.Controllers
{

    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(string Slug = "")
        {
            var category = _dataContext.Categories.FirstOrDefault(c => c.Slug == Slug);
            if (category == null) return RedirectToAction("Index");

            var productsByCategory = _dataContext.Products
                .Where(p => p.CategoryId == category.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(await productsByCategory);
        }

    }
}
