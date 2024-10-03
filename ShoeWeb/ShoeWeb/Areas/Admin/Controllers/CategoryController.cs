using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Data;
using ShoeWeb.Areas.Admin.Admin_ViewModel;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ShoeWeb.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public CategoryController   () : this(new ApplicationDbContext())
        {

        }
        public async Task<CategoryVM> GetCategoriese()
        {
            return new CategoryVM()
            {
                categories = await _db.categories.ToListAsync()

            };

        }
        public async Task<ActionResult> Index()
        {

            var cate = await GetCategoriese();
            return View(cate);
        }
    }
}