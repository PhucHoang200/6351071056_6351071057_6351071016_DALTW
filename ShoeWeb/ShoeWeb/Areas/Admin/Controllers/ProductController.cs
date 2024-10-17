using ShoeWeb.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using ShoeWeb.Areas.Admin.Admin_ViewModel;
namespace ShoeWeb.Areas.Admin.Controllers
{
   
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        public ProductController() : this(new ApplicationDbContext())
        {

        }

        public async Task<ProductVM> GetProductVM()
        {
            return new ProductVM()
            {
                Products = await _db.products.ToListAsync(),
            };
        }
        public async Task<ActionResult> Index()
        {
            var productVM = await GetProductVM();
            return View(productVM);
        }
    }
}