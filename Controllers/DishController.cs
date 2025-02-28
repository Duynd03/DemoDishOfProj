using DemoDishOfProj.Data;
using DemoDishOfProj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace DemoDishOfProj.Controllers
{
    public class DishController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DishController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách món ăn
        public IActionResult Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = page ?? 1;

            // Truyền danh sách danh mục vào ViewBag
            ViewBag.Categories = _context.Categories.ToList();

            var dishes = _context.Dishes.OrderBy(d => d.Id).ToPagedList(pageNumber, pageSize);
            return View(dishes);
        }

        // Tìm kiếm món ăn
        public IActionResult Search(string search, int? page)
        {
            int pageSize = 3;
            int pageNumber = page ?? 1;

            var dishes = _context.Dishes
                .Where(d => string.IsNullOrEmpty(search) || d.Name.Contains(search))
                .OrderBy(d => d.Id)
                .ToPagedList(pageNumber, pageSize);

            return PartialView("_DishList", dishes);
        }

        // Thêm món ăn bằng AJAX
        [HttpPost]
        public async Task<IActionResult> Create(Dish dish, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName); // Chỉ lấy tên file
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    dish.Image = fileName; // Chỉ lưu tên file
                }

                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        public async Task<IActionResult> GetDish(int id)
        {
            if (id <= 0) return BadRequest("ID không hợp lệ!");

            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            return Json(dish);
        }

        // Chỉnh sửa món ăn bằng AJAX
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Dish dish, IFormFile ImageFile)
        {
            if (id != dish.Id) return NotFound();

            var existingDish = await _context.Dishes.FindAsync(id);
            if (existingDish == null) return NotFound();

            if (ModelState.IsValid)
            {
                existingDish.Name = dish.Name;
                existingDish.Description = dish.Description;
                existingDish.Price = dish.Price;
                existingDish.CategoryId = dish.CategoryId;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingDish.Image))
                    {
                        var oldFilePath = Path.Combine(uploadFolder, existingDish.Image);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Lưu ảnh mới
                    var fileName = Path.GetFileName(ImageFile.FileName); // Chỉ lấy tên file
                    var filePath = Path.Combine(uploadFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    existingDish.Image = fileName; // Chỉ lưu tên file
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        // Xóa món ăn bằng AJAX
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy món ăn!" });
        }
    }
}