using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Repository;

namespace Shopping_Coffee.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();

            ViewBag.Keyword = searchTerm;

            return View(products);
        }
        public IActionResult Details(int? Id)
        {
           if (Id == null) return RedirectToAction("Index");

           var productsById = _dataContext.Products.FirstOrDefault(p => p.Id == Id);

          return View(productsById);
        }

       


    }
}
