using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Coffee.Models;
using Shopping_Coffee.Repository;
using System.Collections.Generic;

namespace Shopping_Coffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnviroment)
        {
            _dataContext = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public async Task<IActionResult> Index(int page = 1, int limit = 5)
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

            return View(pageItems);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
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
            return View(product);
        }

        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            var existed_product = await _dataContext.Products.FindAsync(product.Id); // ✅ Sửa lại với await
            if (existed_product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                existed_product.Slug = product.Name.Replace(" ", "-");

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }
                    existed_product.Image = imageName;
                }

                // Cập nhật các trường khác
                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CategoryId = product.CategoryId;
                existed_product.BrandId = product.BrandId;

                _dataContext.Update(existed_product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có lỗi";
                return View(product);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            if (!string.Equals(product.Image, "noname.jpg"))
            {
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                string oldfilePath = Path.Combine(uploadsDir, product.Image);
                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "sản phẩm đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
