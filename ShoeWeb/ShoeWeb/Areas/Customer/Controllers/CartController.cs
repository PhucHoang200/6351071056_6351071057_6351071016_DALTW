using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ShoeWeb.App_Start;
using ShoeWeb.Areas.Customer.CustomerVM;
using ShoeWeb.Data;
using ShoeWeb.Helper;
using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
                .Include("ShoppingCartItems.Product")
                .FirstOrDefaultAsync();

                return cart?.ShoppingCartItems.Where(i => i.status == false).ToList() ?? new List<ShoppingCartItem>();



            }
            else
            {
                var cart = Session["Cart"] as List<ShoppingCartItem>;
                return cart?.Select(ci => new ShoppingCartItem
                {
                    ProductId = ci.ProductId,
                    Product = ci.Product,
                    Price = ci.Price,
                    Quantity = ci.Quantity
                }).ToList() ?? new List<ShoppingCartItem>();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.Identity.GetUserId();
                bool isConverted = await ConvertSessionToDb(userId);
            }

            var cartItems = await GetCartItems();
            decimal total = cartItems.Sum(item => item.Price * item.Quantity);

            var cartViewModel = new CartVM
            {
                Products = cartItems,
                TotalPrice = total
            };

            return View(cartViewModel);
        }


        public async Task<bool> ConvertSessionToDb(string idUser)
        {
            try
            {
                var cartItems = Session["Cart"] as List<ShoppingCartItem>;

                if (cartItems == null || !cartItems.Any())
                {
                    return false; 
                }

                var cart = await _db.shoppingCarts.FirstOrDefaultAsync(c => c.UserId == idUser);

                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = idUser,
                        TotalPrice = cartItems.Sum(item => item.UnitPrice * item.Quantity),
                    };
                    _db.shoppingCarts.Add(cart);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    cart.TotalPrice += cartItems.Sum(item => item.UnitPrice * item.Quantity);
                    await _db.SaveChangesAsync();
                }

                foreach (var item in cartItems)
                {
                    item.ShoppingCartId = cart.Id;
                }

                _db.shoppingCartItems.AddRange(cartItems);
                await _db.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");

                return true; 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi chuyển Session sang DB: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> AddItemToDb(int id, int quantity, float numberSize)
        {
            try
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var product = await _db.products.FindAsync(id);

                if (product != null)
                {
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
                        await _db.SaveChangesAsync();
                    }
                    Debug.WriteLine(numberSize);

                    // Tìm sản phẩm trong giỏ hàng
                    var existingItem = await _db.shoppingCartItems
                        .FirstOrDefaultAsync(ci => ci.ShoppingCartId == cart.Id && ci.ProductId == id && ci.numberSize == numberSize);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                        _db.Entry(existingItem).State = EntityState.Modified;
                    }
                    else
                    {
                        Debug.WriteLine("not found");

                        // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới sản phẩm
                        var cartItem = new ShoppingCartItem
                        {
                            ProductId = product.productId,
                            Price = product.price,
                            numberSize = numberSize,
                            UnitPriceDb = quantity * product.price,
                            Quantity = quantity,
                            ShoppingCartId = cart.Id
                        };
                        _db.shoppingCartItems.Add(cartItem);
                    }
                    _db.Entry(cart).State = EntityState.Modified;

                    await _db.SaveChangesAsync();
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;

            }
        }
        public bool AddItemToSession(int id, int quantity, float numberSize)
        {
            try
            {
                var cart = Session["Cart"] as List<ShoppingCartItem> ?? new List<ShoppingCartItem>();

                var product = _db.products.Find(id);
                if (product == null)
                {
                    return false;
                }
                Debug.WriteLine(numberSize);

                var existingItem = cart.FirstOrDefault(i => i.ProductId == id && i.numberSize == numberSize);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(new ShoppingCartItem
                    {
                        ProductId = product.productId,
                        Product = product,
                        Price = product.price,
                        Quantity = quantity,
                        UnitPriceDb = quantity * product.price,
                        numberSize = numberSize,
                        ShoppingCartId = 0  //  chưa đăng nhập, ShoppingCartId = 0
                    });
                }

                Session["Cart"] = cart;
                return true;
            }
            catch
            {
                return false;
            }
        }




        [HttpPost]
        public async Task<ActionResult> AddToCart(int id, int quantity, int sizeId)
        {

            try
            {
                var size = await _db.sizes.FindAsync(sizeId);

                var sizeOfProduct = await _db.sizeOfProducts.Where(s => s.sizeId == sizeId).FirstOrDefaultAsync();

                if (sizeOfProduct == null)
                {
                    return Json(new { success = false, Message = "Size not found." });
                }
                Debug.WriteLine("size ok");
                if (User.Identity.IsAuthenticated)
                {
                    var result = await AddItemToDb(id, quantity, size.numberSize);
                    if (result)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, Message = "Failed to add item to database." });
                    }
                }
                else
                {

                    bool result = AddItemToSession(id, quantity, size.numberSize);
                    if (result)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, Message = "Failed to add item to session." });
                    }
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, trả về thông báo lỗi
                return Json(new { success = false, Message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult> UpdateCartItem(int quantity, int productId, float numberSize)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var cartItem = await _db.shoppingCartItems
                                              .Where(c => c.ProductId == productId && c.numberSize == numberSize)
                                              .FirstOrDefaultAsync();
                    if (cartItem == null)
                    {
                        return Json(new { success = false, Message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
                    }

                    cartItem.Quantity = quantity;
                    cartItem.UnitPriceDb = cartItem.Price * quantity;
                    await _db.SaveChangesAsync();

                    return Json(new { success = true, data = cartItem });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, Message = "Lỗi: " + ex.Message });
                }
            }
            else
            {
                var cart = Session["Cart"] as List<ShoppingCartItem>;
                var product = _db.products.Find(productId);

                if (product == null)
                {
                    return Json(new { success = false, Message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
                }

                var existingItem = cart.FirstOrDefault(i => i.ProductId == productId && i.numberSize == numberSize);
                if (existingItem != null)
                {
                    existingItem.Quantity = quantity; 
                }
                else
                {
                    cart.Add(new ShoppingCartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        numberSize = numberSize,
                    });
                }

                return Json(new { success = true, data = cart });
            }
        }
        [HttpPost]
        public async Task<ActionResult> DeleteCartItem(int productId, int shoppingCartId, float numberSize)
        {
            try
            {
                var cartItem = await _db.shoppingCartItems
                    .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.ShoppingCartId == shoppingCartId && ci.numberSize == numberSize);

                if (cartItem != null)
                {
                    _db.shoppingCartItems.Remove(cartItem);
                    await _db.SaveChangesAsync();

                    var cart = await _db.shoppingCarts.FindAsync(shoppingCartId);
                    var totalPrice = cart.ShoppingCartItems.Sum(ci => ci.Quantity * ci.Price); 

                    return Json(new { success = true, data = new { TotalPrice = totalPrice } });
                }

                return Json(new { success = false, Message = "Sản phẩm không tìm thấy trong giỏ hàng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = "Đã có lỗi xảy ra: " + ex.Message });
            }
        }


        [HttpGet]
        public async Task<ActionResult> Checkout()
        {
            return View();
        }
    }


}



