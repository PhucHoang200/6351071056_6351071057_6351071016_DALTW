using ShoeWeb.Data;
using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Areas.Customer.CustomertVM;
using System.Net;
using ShoeWeb.Areas.Customer.CustomerVM;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Data.Entity;

namespace ShoeWeb.Areas.Customer.Controllers
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

        public ActionResult Search(string searchTerm)
        {
            // Tìm kiếm sản phẩm dựa trên từ khóa
            var products = _db.products
                .Where(p => p.productName.ToLower().Contains(searchTerm.ToLower()) ||
                            p.productDescription.ToLower().Contains(searchTerm.ToLower()))
                .ToList();

            // Lưu trữ kết quả tìm kiếm trong TempData để truyền sang View Product
            TempData["SearchResults"] = products;
            TempData["SearchTerm"] = searchTerm;

            // Chuyển hướng đến Action Product để hiển thị kết quả
            return RedirectToAction("Product");
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



        private List<Product> Laysanpham(ApplicationDbContext context)
        {
            return context.products.ToList(); // Lấy tất cả sản phẩm
        }

        private List<Category> Laydanhmuc(ApplicationDbContext context)
        {
            return context.categories.ToList(); // Lấy tất cả danh mục
        }

        private List<Brand> Laythuonghieu(ApplicationDbContext context)
        {
            return context.brands.ToList();
        }

        private List<Size> Laykichthuoc(ApplicationDbContext context)
        {
            return context.sizes.ToList();
        }

        private List<Product> Laysanpham(ApplicationDbContext context, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return context.products.ToList(); // Lấy tất cả sản phẩm
            }

            return context.products
                .Where(p => p.productName.Contains(searchTerm) || p.productDescription.Contains(searchTerm))
                .ToList(); // Lấy sản phẩm theo từ khóa tìm kiếm
        }

        public ActionResult Product(string searchTerm)
        {
            // Lưu từ khóa tìm kiếm vào TempData
            TempData["SearchTerm"] = searchTerm;

            // Lấy danh sách sản phẩm từ TempData nếu có, nếu không lấy tất cả sản phẩm
            var products = TempData["SearchResults"] as List<Product> ?? Laysanpham(_db, searchTerm);

            using (var applicationDbContext = new ApplicationDbContext())
            {


                var sanphammoi = Laysanpham(applicationDbContext);
                var danhmucList = Laydanhmuc(applicationDbContext);
                var thuonghieuList = Laythuonghieu(applicationDbContext);
                var kichthuocList = Laykichthuoc(applicationDbContext);

                var productVm = new ProductVM
                {
                    Products = products,
                    Categories = danhmucList,
                    Brands = thuonghieuList,
                    Sizes = kichthuocList
                };

                return View(productVm);
            }

        }

        public ActionResult FilterByCategory(int id)
        {
            using (var applicationDbContext = new ApplicationDbContext())
            {
                var giayTheoDanhMuc = applicationDbContext.products.Where(s => s.cateId == id).ToList();
                var productVm = new ProductVM
                {
                    Products = giayTheoDanhMuc,
                    Categories = Laydanhmuc(applicationDbContext),
                    Brands = Laythuonghieu(applicationDbContext),
                    Sizes = Laykichthuoc(applicationDbContext)
                };
                return View("Index", productVm);
            }

        }

        public ActionResult FilterByBrand(int id)
        {
            using (var applicationDbContext = new ApplicationDbContext())
            {
                var giayTheoThuongHieu = applicationDbContext.products.Where(s => s.brandId == id).ToList();
                var productVm = new ProductVM
                {
                    Products = giayTheoThuongHieu,
                    Categories = Laydanhmuc(applicationDbContext),
                    Brands = Laythuonghieu(applicationDbContext),
                    Sizes = Laykichthuoc(applicationDbContext)
                };
                return View("Index", productVm);
            }
        }

        public ActionResult FilterSize(int id)
        {
            using (var applicationDbContext = new ApplicationDbContext())
            {
                // Lấy các sản phẩm dựa trên kích cỡ từ bảng SizeOfProduct
                var giayTheoKichCo = applicationDbContext.sizeOfProducts
                    .Where(sop => sop.sizeId == id) // Lọc theo sizeId
                    .Select(sop => sop.product) // Lấy sản phẩm từ SizeOfProduct
                    .ToList();

                var productVm = new ProductVM
                {
                    Products = giayTheoKichCo,
                    Categories = Laydanhmuc(applicationDbContext),
                    Brands = Laythuonghieu(applicationDbContext),
                    Sizes = Laykichthuoc(applicationDbContext)
                };
                return View("Index", productVm);
            }
        }


        public ActionResult FilterByGender(bool gender)
        {
            using (var applicationDbContext = new ApplicationDbContext())
            {
                var giayTheoGioiTinh = applicationDbContext.sizeOfProducts
                    .Where(sop => sop.size.gender == gender) // Lọc theo giới tính
                    .Select(sop => sop.product) // Lấy danh sách sản phẩm từ SizeOfProduct
                    .ToList();

                var productVm = new ProductVM
                {
                    Products = giayTheoGioiTinh,
                    Categories = Laydanhmuc(applicationDbContext),
                    Brands = Laythuonghieu(applicationDbContext),
                    Sizes = Laykichthuoc(applicationDbContext)
                };
                return View("Index", productVm);
            }
        }


        [HttpGet]
        public async Task<ActionResult> ProductDetails(int? productId)
        {
            var detailProductVm = new DetailProductVM();

            var product = _db.products.FirstOrDefault(p => p.productId == productId);

            if (product == null)
            {
                return View(detailProductVm);
            }

            var sizeIds = _db.sizeOfProducts
                      .Where(s => s.productId == productId)
                      .Select(s => s.sizeId)
                      .ToList();

            if (sizeIds.Any())
            {
                var sizes = await _db.sizes
                                     .Where(size => sizeIds.Contains(size.sizeId))
                                     .ToListAsync();

                detailProductVm.sizes = sizes;
            }

            else
            {
                detailProductVm.sizes = new List<Size>(); 
            }

            var relatedProducts = _db.products
                                       .Where(p => p.cateId == product.cateId && p.productId != productId)
                                       .Take(4)
                                       .ToList();

            detailProductVm.products = relatedProducts;
            detailProductVm.Product = product;

            return View(detailProductVm);
        }




    }
}