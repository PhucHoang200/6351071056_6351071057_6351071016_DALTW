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
using PagedList;
using PagedList.Mvc;
using System.Web.UI;

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

        public ActionResult Product(string searchTerm, int? page, bool? FilterByGender, int? FilterSize, int? FilterByBrand, int? FilterByCategory)
        {
            // Lưu từ khóa tìm kiếm vào TempData
            TempData["SearchTerm"] = searchTerm;

            // Lấy danh sách sản phẩm từ TempData nếu có, nếu không lấy tất cả sản phẩm
            var products = TempData["SearchResults"] as List<Product> ?? Laysanpham(_db, searchTerm);

            // Lọc theo giới tính nếu có
            if (FilterByGender.HasValue)
            {
                products = _db.sizeOfProducts
                    .Where(sop => sop.size.gender == FilterByGender.Value) // Lọc theo giới tính từ bảng Size
                    .Select(sop => sop.product) // Lấy sản phẩm tương ứng
                    .ToList();
            }

            // Lọc theo kích thước nếu có
            if (FilterSize.HasValue)
            {
                products = _db.sizeOfProducts
                    .Where(sop => sop.sizeId == FilterSize.Value)
                    .Select(sop => sop.product)
                    .ToList();
            }

            // Lọc theo thương hiệu nếu có
            if (FilterByBrand.HasValue)
            {
                products = products.Where(p => p.brandId == FilterByBrand.Value).ToList();
            }

            // Lọc theo danh mục nếu có
            if (FilterByCategory.HasValue)
            {
                products = products.Where(p => p.cateId == FilterByCategory.Value).ToList();
            }

            int pageSize = 28; // Số sản phẩm trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            // Truyền thông tin vào view model để hiển thị trong view
            var productVm = new ProductVM
            {
                Products = pagedProducts,
                Categories = Laydanhmuc(_db),
                Brands = Laythuonghieu(_db),
                Sizes = Laykichthuoc(_db)
            };

            return View(productVm);
        }


        public ActionResult FilterByCategory(int id, int? page, bool? FilterByGender, int? FilterSize, int? FilterByBrand)
        {
            using (var applicationDbContext = new ApplicationDbContext())
            {
                var giayTheoDanhMuc = applicationDbContext.products.Where(s => s.cateId == id).ToList();
                // Số sản phẩm trên mỗi trang
                int pageSize = 4;
                // Trang hiện tại
                int pageNumber = page ?? 1;

                var pagedProducts = giayTheoDanhMuc.ToPagedList(pageNumber, pageSize);
                var productVm = new ProductVM
                {
                    Products = pagedProducts,
                    Categories = Laydanhmuc(applicationDbContext),
                    Brands = Laythuonghieu(applicationDbContext),
                    Sizes = Laykichthuoc(applicationDbContext)
                };
                return View("Index", productVm);
            }

        }

        public ActionResult FilterByBrand(int id, int? page, bool? FilterByGender, int? FilterSize, int? FilterByBrand)
        {
            using (var applicationDbContext = new ApplicationDbContext())
            {
                var giayTheoThuongHieu = applicationDbContext.products.Where(s => s.brandId == id).ToList();
                int pageSize = 4;
                int pageNumber = page ?? 1;

                var pagedProducts = giayTheoThuongHieu.ToPagedList(pageNumber, pageSize);
                var productVm = new ProductVM
                {
                    Products = pagedProducts,
                    Categories = Laydanhmuc(applicationDbContext),
                    Brands = Laythuonghieu(applicationDbContext),
                    Sizes = Laykichthuoc(applicationDbContext)
                };
                return View("Index", productVm);
            }
        }

        public ActionResult FilterSize(int id, int? page, bool? FilterByGender, int? FilterSize, int? FilterByBrand)
        {
            using (var applicationDbContext = new ApplicationDbContext())
            {
                // Lấy các sản phẩm dựa trên kích cỡ từ bảng SizeOfProduct
                var giayTheoKichCo = applicationDbContext.sizeOfProducts
                    .Where(sop => sop.sizeId == id) // Lọc theo sizeId
                    .Select(sop => sop.product) // Lấy sản phẩm từ SizeOfProduct
                    .ToList();
                int pageSize = 4;
                int pageNumber = page ?? 1;

                var pagedProducts = giayTheoKichCo.ToPagedList(pageNumber, pageSize);

                var productVm = new ProductVM
                {
                    Products = pagedProducts,
                    Categories = Laydanhmuc(applicationDbContext),
                    Brands = Laythuonghieu(applicationDbContext),
                    Sizes = Laykichthuoc(applicationDbContext)
                };
                return View("Index", productVm);
            }
        }


        public ActionResult FilterByGender(bool gender, int? page, bool? FilterByGender, int? FilterSize, int? FilterByBrand)
        {
            using (var applicationDbContext = new ApplicationDbContext())
            {
                var giayTheoGioiTinh = applicationDbContext.sizeOfProducts
                    .Where(sop => sop.size.gender == gender) // Lọc theo giới tính
                    .Select(sop => sop.product) // Lấy danh sách sản phẩm từ SizeOfProduct
                    .ToList();
                int pageSize = 4;
                int pageNumber = page ?? 1;

                var pagedProducts = giayTheoGioiTinh.ToPagedList(pageNumber, pageSize);
                var productVm = new ProductVM
                {
                    Products = pagedProducts,
                    Categories = Laydanhmuc(applicationDbContext),
                    Brands = Laythuonghieu(applicationDbContext),
                    Sizes = Laykichthuoc(applicationDbContext)
                };
                return View("Index", productVm);
            }
        }


        [HttpGet]
        public ActionResult ProductDetails(int? productId)
        {
            var detailProductVm = new DetailProductVM();

            var product = _db.products.FirstOrDefault(p => p.productId == productId);

            if (product == null)
            {
                return HttpNotFound();
            }

            var SizeOfProduct = _db.sizeOfProducts.Where(s => s.productId == productId).ToList();

            if (SizeOfProduct != null && SizeOfProduct.Any())
            {
                var sizeIds = SizeOfProduct.Select(s => s.sizeId).ToList();
                detailProductVm.sizes = _db.sizes.Where(s => sizeIds.Contains(s.sizeId)).ToList();
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
            detailProductVm.sizeOfProduct = SizeOfProduct;

            return View(detailProductVm);
        }




    }
}