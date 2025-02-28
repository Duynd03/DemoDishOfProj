using DemoDishOfProj.Data;
using DemoDishOfProj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace DemoDishOfProj.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách danh mục
        public IActionResult Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = page ?? 1;

            var categories = _context.Categories.OrderBy(c => c.Id).ToPagedList(pageNumber, pageSize);
            return View(categories);
        }

        // Tìm kiếm danh mục
        public IActionResult Search(string search, int? page)
        {
            int pageSize = 3;
            int pageNumber = page ?? 1;

            var categories = _context.Categories
                .Where(c => string.IsNullOrEmpty(search) || c.Name.Contains(search))
                .OrderBy(c => c.Id)
                .ToPagedList(pageNumber, pageSize);

            return PartialView("_CategoryList", categories);
        }

        // Thêm danh mục bằng AJAX
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        // Lấy thông tin danh mục để hiển thị trên modal chỉnh sửa
        public async Task<IActionResult> GetCategory(int id)
        {
            if (id <= 0) return BadRequest("ID không hợp lệ!");

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return Json(category);
        }

        // Chỉnh sửa danh mục bằng AJAX
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null) return NotFound();

            if (ModelState.IsValid)
            {
                existingCategory.Name = category.Name; // Chỉ cập nhật trường Name
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        // Xóa danh mục bằng AJAX
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy danh mục!" });
        }
    }
}
