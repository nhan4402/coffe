using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Models;
using Shopping_Coffee.Repository;

namespace Shopping_Coffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private int LIMIT = 5;
        private static int CURENT_PAGE = 1;
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int page = 1, int limit = 5)
        {
            List<CategoryModel> tabsList = await _dataContext.Categories.ToListAsync();
           int totalItems = tabsList.Count;// 10
            int totalPages = (int)Math.Ceiling(totalItems / (double)limit); // 10/5=2

            // Lấy danh sách theo phân trang
            var pagedItems = tabsList
                // 0 
                .Skip((page - 1) * limit)
                // 5
                .Take(limit)
                .ToList();
            // Gửi sang View qua ViewData (hoặc ViewModel nếu bạn muốn)
            ViewData["CURENT_PAGE"] = page;
            ViewData["TOLTAL_PAGE"] = totalPages;
            ViewData["LIMIT"] = limit;

            return View(pagedItems);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong database");
                    return View(category);
                }

                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(category);
        }

        
        public async Task<IActionResult> Edit(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");

                _dataContext.Update(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Danh mục đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
