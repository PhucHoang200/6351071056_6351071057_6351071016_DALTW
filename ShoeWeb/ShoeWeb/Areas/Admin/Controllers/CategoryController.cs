using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Data;
using ShoeWeb.Areas.Admin.Admin_ViewModel;
using System.Threading.Tasks;
using System.Data.Entity;
using ShoeWeb.Models;
using System.Runtime.InteropServices;
using ShoeWeb.Utility;
using ShoeWeb.Helper;

namespace ShoeWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public CategoryController() : this(new ApplicationDbContext())
        {

        }
        public async Task<CategoryVM> GetCategoriese()
        {
            return new CategoryVM()
            {
                categories = await _db.categories.ToListAsync()

            };

        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {

            var cate = await GetCategoriese();
            return View(cate);
        }

        [HttpPost]
        public async Task<ActionResult> AddCategory(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            {
                return Json(new { success = false, message = "Thông tin đầu vào không hợp lệ." });
            }

            try
            {
                // Kiểm tra xem danh mục đã tồn tại chưa
                var existingCategory = await _db.categories.FirstOrDefaultAsync(c => c.cateName == name);
                if (existingCategory != null)
                {
                    return Json(new { success = false, message = "Danh mục đã tồn tại!" });
                }

                // Nếu không tồn tại, thêm mới
                Category cateNew = new Category()
                {
                    cateDescription = description,
                    cateName = name
                };

                _db.categories.Add(cateNew);
                await _db.SaveChangesAsync();

                // Lấy danh sách danh mục mới
                var updatedCategories = await GetCategoriese();
                return Json(new { success = true, categories = updatedCategories.categories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi thêm danh mục.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _db.categories.Where(c => c.cateId == id).FirstOrDefaultAsync();
                if (category != null)
                {
                    var isProduct = await _db.products.AnyAsync(p => p.cateId == category.cateId);

                    if (isProduct)
                    {
                        return Json(new { success = false, message = "Cannot delete category because there are products associated with it." });

                    }
                    _db.categories.Remove(category);
                    await _db.SaveChangesAsync();


                    var updatedCategories = await GetCategoriese();
                    return Json(new { success = true, categories = updatedCategories.categories });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Json(new { success = false });

        }


        [HttpPost]
        public async Task<ActionResult> Update(int id, string descriptionCate, string nameCate)
        {
            if (string.IsNullOrEmpty(descriptionCate) || string.IsNullOrEmpty(nameCate))
            {
                return Json(new { success = false, message = "Thông tin đầu vào không hợp lệ." });
            }

            var category = await _db.categories.FirstOrDefaultAsync(c => c.cateId == id);
            if (category == null)
            {
                return Json(new { success = false, message = "Danh mục không tồn tại." });
            }

            category.cateName = nameCate;
            category.cateDescription = descriptionCate;

            try
            {
                await _db.SaveChangesAsync();

                var updatedCategories = await GetCategoriese();
                return Json(new { success = true, categories = updatedCategories.categories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật danh mục.", error = ex.Message });
            }
        }

    }
}
