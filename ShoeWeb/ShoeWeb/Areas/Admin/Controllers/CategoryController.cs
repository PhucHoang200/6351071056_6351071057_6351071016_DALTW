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

namespace ShoeWeb.Areas.Admin.Controllers
{
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
            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(name))
            {
                return Json(new { success = false, message = "Thông tin đầu vào không hợp lệ." });
            }
            Category cateNew = new Category()
            {
                cateDescription = description,
                cateName = name
            };
            try
            {
                 _db.categories.Add(cateNew);
                await _db.SaveChangesAsync();

                var updatedCategories = await GetCategoriese();
                return Json(new { success = true, categories = updatedCategories.categories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi thêm danh mục.", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var category = await _db.categories.Where(c => c.cateId == id).FirstOrDefaultAsync();
                if (category != null)
                {
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