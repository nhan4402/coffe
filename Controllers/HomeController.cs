using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Models;
using Shopping_Coffee.Repository;

namespace Shopping_Coffee.Controllers
{
    public class HomeController : Controller
    {   private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _dataContext = context;
            _logger = logger;
            _dataContext = context;
        }

		public async Task<IActionResult> Index(int page = 1, int limit = 6)
		{
			List<ProductModel> productslist = await _dataContext.Products
				.Include(p => p.Category)
				.Include(p => p.Brand)
				.ToListAsync();

			int totalItems = productslist.Count;
			int totalPages = (int)Math.Ceiling(totalItems / (double)limit);
			var pageItems = productslist

				.Skip((page - 1) * limit)
				.Take(limit)
				.ToList();

			ViewData["CURENT_PAGE"] = page;
			ViewData["TOLTAL_PAGE"] = totalPages;
			ViewData["LIMIT"] = limit;
			ViewData["total"] = totalItems;

			return View(pageItems);
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
