using ShoeWeb.Data;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using ShoeWeb.Areas.Admin.Admin_ViewModel;
using System.Web.Services.Description;
using System.Net.Http;
using ShoeWeb.Models;
using ShoeWeb.Utility;
using ShoeWeb.Helper;



namespace ShoeWeb.Areas.Admin.Controllers
{
    [AdminAutho]


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
                categories = await _db.categories.ToListAsync(),
                brands = await _db.brands.ToListAsync(),
                sizes = await _db.sizes.ToListAsync(),
                origins = await _db.origin.ToListAsync()
            };
        }
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var productVM = await GetProductVM();
            return View(productVM);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var product = await _db.products.Where(c => c.productId == id).FirstOrDefaultAsync();
                if (product != null)
                {
                    _db.products.Remove(product);
                    await _db.SaveChangesAsync();

                    var updatedProduct = await GetProductVM();
                    return Json(new { success = true, products = updatedProduct.Products });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public async Task<ActionResult> Update(int id, string name, string description, HttpPostedFileBase image)
        {
            var product = _db.products.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            product.productName = name;
            product.productDescription = description;

            if (image != null && image.ContentLength > 0)
            {
                await UploadImageAsync(product, image);
            }

            await _db.SaveChangesAsync();

            var productVM = await GetProductVM();

            return Json(new { success = true, message = "Cập nhật thành công!", products = productVM.Products });
        }

        public async Task UploadImageAsync(Product product, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(Server.MapPath("~/Content/images/"), uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.InputStream.CopyToAsync(fileStream);
                }

                product.image = "/Content/images/" + uniqueFileName;
            }
        }


        [HttpPost]
        public async Task<ActionResult> Add(string name, decimal price, int quantity, string description,
                                             int categoryId, int brandId, int sizeId, int originId,
                                             HttpPostedFileBase image)
        {
            Product newProduct = new Product()
            {
                productName = name,
                productDescription = description,
                price = price,
                quantity = quantity,
                createdDate = DateTime.Now,
                updatedDate = DateTime.Now,
                cateId = categoryId,
                brandId = brandId,
                idOrigin = originId
            };

            if (image != null && image.ContentLength > 0)
            {
                await UploadImageAsync(newProduct, image);
            }

            try
            {
                _db.products.Add(newProduct);
                await _db.SaveChangesAsync();

                var sizeOfProduct = new SizeOfProduct()
                {
                    sizeId = sizeId,
                    productId = newProduct.productId
                };
                _db.sizeOfProducts.Add(sizeOfProduct);
                await _db.SaveChangesAsync();

                var productVM = await GetProductVM();

                return Json(new { success = true, message = "Thêm sản phẩm thành công!", products = productVM.Products });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




    }
}