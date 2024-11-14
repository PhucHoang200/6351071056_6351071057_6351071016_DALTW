using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ShoeWeb.App_Start;
using ShoeWeb.Areas.Customer.CustomerVM;
using ShoeWeb.Data;
using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShoeWeb.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private ApplicationUserManager _userManager;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        public CartController() : this(new ApplicationDbContext())
        {
        }
        public CartController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public async Task<List<ShoppingCartItem>> GetCartItems()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var cart = await _db.shoppingCarts
                    .Where(c => c.UserId == userId)
                    .Include(c => c.ShoppingCartItems)
                    .FirstOrDefaultAsync();

                return cart?.ShoppingCartItems.ToList() ?? new List<ShoppingCartItem>();
            }
            else
            {
                var cart = Session["Cart"] as List<ShoppingCartItem>;
                return cart?.Select(ci => new ShoppingCartItem
                {
                    ProductId = ci.ProductId,
                    Price = ci.Price,
                    Quantity = ci.Quantity
                }).ToList() ?? new List<ShoppingCartItem>();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {

            var cartItems = await GetCartItems();

            decimal total = cartItems.Sum(item => item.UnitPrice);

            var cartVM = new CartVM()
            {
                Products = cartItems,
                TotalPrice = total
            };
            return View(cartVM);
        }



        [HttpPost]
        public async Task<ActionResult> AddToCart(int id, int quantity = 1)
        {
            try
            {
                // Kiểm tra nếu người dùng đã đăng nhập
                if (User.Identity.IsAuthenticated)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    var product = await _db.products.FindAsync(id);

                    // Kiểm tra sản phẩm có tồn tại không
                    if (product != null)
                    {
                        // Tìm giỏ hàng của người dùng
                        var cart = await _db.shoppingCarts.FirstOrDefaultAsync(c => c.UserId == user.Id);

                        if (cart == null)
                        {
                            // Nếu giỏ hàng chưa tồn tại, tạo mới giỏ hàng
                            cart = new ShoppingCart
                            {
                                UserId = user.Id,
                                TotalPrice = 0
                            };
                            _db.shoppingCarts.Add(cart);
                            await _db.SaveChangesAsync();  // Lưu giỏ hàng mới
                        }

                        // Tìm sản phẩm trong giỏ hàng
                        var existingItem = await _db.shoppingCartItems
                            .FirstOrDefaultAsync(ci => ci.ShoppingCartId == cart.Id && ci.ProductId == id);

                        if (existingItem != null)
                        {
                            existingItem.Quantity += quantity;
                            _db.Entry(existingItem).State = EntityState.Modified;
                        }
                        else
                        {
                            // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới sản phẩm
                            var cartItem = new ShoppingCartItem
                            {
                                ProductId = product.productId,
                                Price = product.price,
                                Quantity = quantity,
                                ShoppingCartId = cart.Id
                            };
                            _db.shoppingCartItems.Add(cartItem);
                        }

                        // Cập nhật lại tổng giá trị của giỏ hàng
                        cart.TotalPrice = cart.ShoppingCartItems?.Sum(ci => ci.Quantity * ci.Price) ?? 0;
                        _db.Entry(cart).State = EntityState.Modified;

                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        // Nếu không tìm thấy sản phẩm trong DB, trả về thông báo lỗi
                        return Json(new { success = false, Message = "Product not found." });
                    }
                }
                else
                {
                    // Nếu người dùng chưa đăng nhập, lưu giỏ hàng vào session
                    var cart = Session["Cart"] as List<ShoppingCartItem> ?? new List<ShoppingCartItem>();

                    var product = await _db.products.FindAsync(id);

                    // Kiểm tra sản phẩm có tồn tại không
                    if (product != null)
                    {
                        // Kiểm tra nếu sản phẩm đã có trong giỏ hàng
                        var existingItem = cart.FirstOrDefault(i => i.ProductId == id);
                        if (existingItem != null)
                        {
                            // Cập nhật số lượng sản phẩm
                            existingItem.Quantity += quantity;
                        }
                        else
                        {
                            // Thêm sản phẩm mới vào giỏ hàng
                            cart.Add(new ShoppingCartItem
                            {
                                ProductId = product.productId,
                                Price = product.price,
                                Quantity = quantity
                            });
                        }

                        // Lưu giỏ hàng vào session
                        Session["Cart"] = cart;
                    }
                    else
                    {
                        // Nếu không tìm thấy sản phẩm trong DB, trả về thông báo lỗi
                        return Json(new { success = false, Message = "Product not found." });
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, trả về thông báo lỗi
                return Json(new { success = false, Message = ex.Message });
            }
        }


    }
}