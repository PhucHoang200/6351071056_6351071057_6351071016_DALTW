using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Data;
using ShoeWeb.Models;
using ShoeWeb.Areas.Customer.CustomertVM;


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

            List<List<Product>> products = new List<List<Product>>();
            var categories = _db.categories.ToList();
            foreach (var item in categories)
            {
                var productByCate = _db.products
                    .Where(p => p.cateId == item.cateId)
                    .Take(4) // Lấy tối đa 4 sản phẩm
                    .ToList();
                products.Add(productByCate);
            }

            var brands = _db.brands.ToList();

            HomeVM homeVM = new HomeVM()
            {
                Products = products,
                Categories = categories,
                Brands = brands
            };

            // Truyền dữ liệu vào View thông qua ViewBag
            //ViewBag.Categories = categories;
            return View(homeVM); // Truyền danh sách sản phẩm vào View
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
            var products = _db.products.ToList();
            var categories = _db.categories.ToList();

            ProductVM productVM = new ProductVM()
            {
                Products = products,
                Categories = categories
            };
            ViewBag.Categories = categories;

            return View(productVM);
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