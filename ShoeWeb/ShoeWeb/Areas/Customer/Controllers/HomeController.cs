using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Data;
using ShoeWeb.Models;

namespace ShoeWeb.Controllers
{
    public class HomeController : Controller
    {
        public readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public HomeController() : this(new ApplicationDbContext())
        {
            
        }
        public ActionResult Index()
        {

            // Lấy danh sách các sản phẩm từ cơ sở dữ liệu
            var products = _db.products.ToList();
            var categories = _db.categories.ToList();

            // Truyền dữ liệu vào View thông qua ViewBag
            ViewBag.Categories = categories;
            return View(products); // Truyền danh sách sản phẩm vào View
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    
        public ActionResult Product()
        {
            ViewBag.Message = "Your product page.";

            return View();
        }

        public ActionResult Cart()
        {
            ViewBag.Message = "You cart page.";

            return View();
        }

        public ActionResult ProductDetails()
        {
            ViewBag.Message = "Your productdetial page.";

            return View();
        }
     }
}