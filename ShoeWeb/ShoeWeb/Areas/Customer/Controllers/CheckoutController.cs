using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using ShoeWeb.App_Start;
using ShoeWeb.Data;
using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ShoeWeb.Areas.Customer.CustomerVM;
using ShoeWeb.Utility;
using System.Diagnostics;
using ShoeWeb.Helper.VnPay;
using ShoeWeb.Service.VnPay;
using System.Configuration;
using WebGrease;
using Serilog.Core;
using Org.BouncyCastle.Asn1.X9;
using Serilog;
using log4net;


namespace ShoeWeb.Areas.Customer.Controllers
{
    [Authorize(Roles = SD.CustomerRole)]

    public class CheckoutController : Controller
    {

        private readonly ApplicationDbContext _db;
        private ApplicationUserManager _userManager;
        //private readonly IVnPayService _vnPayService;
        private OrderViewModel orderViewModel;

        public CheckoutController(ApplicationDbContext db)
        {
            _db = db;
            //_vnPayService = new VnPayService();
        }
        public CheckoutController() : this(new ApplicationDbContext())
        {
            //_vnPayService = new VnPayService();

        }
        public CheckoutController(ApplicationUserManager userManager)
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
        //[HttpGet]
        //public ActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        //{ 
        //    if(model == null) model = new PaymentInformationModel();
        //    model.OrderType = "Thực Phẩm - Tiêu Dùng";
        //    model.OrderDescription = "Thanh toán VNPay thành công";
        //    model.Amount = 100000.00M;
        //    model.Name = "hieuchance2018";



        //    var url = _vnPayService.CreatePaymentUrl(model, this.HttpContext.ApplicationInstance.Context);

        //    Debug.WriteLine(model.OrderType);
        //    Debug.WriteLine(model.OrderDescription);
        //    Debug.WriteLine(model.Amount);
        //    Debug.WriteLine(model.Name);
        //    return Redirect(url);
        //}

        //[HttpGet]
        //public ActionResult PaymentCallbackVnpay()
        //{
        //    var response = _vnPayService.PaymentExecute(Request.QueryString);

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        public async Task<List<ShoppingCartItem>> GetCartItems()
        {
            if (User.Identity.IsAuthenticated)
            {

                var userId = User.Identity.GetUserId();
                var cart = await _db.shoppingCarts
                .Where(c => c.UserId == userId)
                .Include("ShoppingCartItems.Product")
                .FirstOrDefaultAsync();

                return cart?.ShoppingCartItems.ToList() ?? new List<ShoppingCartItem>();



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

        // GET: Customer/Checkout
        [HttpGet]
        public  ActionResult Checkout()
        {
            orderViewModel = new OrderViewModel();
            return View(orderViewModel);
        }
        public string GenerateOrderCode()
        {
            var lastOrder = _db.Orders.OrderByDescending(o => o.Id).FirstOrDefault();
            int newOrderId = (lastOrder == null) ? 1 : lastOrder.Id + 1;
            return "ORD" + newOrderId.ToString("D6"); // Sinh mã đơn hàng dạng ORD000001, ORD000002, ...
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Checkout(OrderViewModel model)
        {
            Debug.WriteLine(model.PaymentMethod);
            if (model == null) return View(model);
            if (ModelState.IsValid)
            {

                try
                {
                    var userId = User.Identity.GetUserId();
                    var cart = await _db.shoppingCarts.Where(c => c.UserId == userId).FirstOrDefaultAsync();
                    var cartItems = await _db.shoppingCartItems.Where(c => c.ShoppingCartId == cart.Id && c.status == false).ToListAsync();
                    if (cart == null || cartItems == null || !cartItems.Any())
                    {
                        ModelState.AddModelError("", "Giỏ hàng rỗng.");
                        return View(model);
                    }
                    if (model.PaymentMethod == "VNPay")
                    {
                        CreatePayment("abc",10000);
                        return View();
                    }
                    else
                    {
                        Order order = new Order()
                        {
                            Code = GenerateOrderCode(), // Tạo mã đơn hàng
                            CustomerName = model.FirstName + " " + model.LastName,
                            Phone = model.Phone,
                            Address = model.Address,
                            TinhThanh = model.ProvinceText,
                            QuanHuyen = model.DistrictText,
                            PhuongXa = model.WardText,
                            TotalAmount = cart.TotalPrice, // Tính tổng tiền đơn hàng
                            Quantity = cartItems.Count, // Tổng số lượng sản phẩm trong giỏ
                            TypePayment = model.PaymentMethod == "VNPay" ? 1 : 0, // 1: VNPay, 0: COD
                            Status = 0, // Trạng thái đơn hàng (0: chưa xử lý)
                            CreatedDate = DateTime.Now,
                            isPayment = model.PaymentMethod == "VNPay" ? true : false, // Nếu là VNPay thì thanh toán, còn lại là COD -> chưa thanh toán
                            isAccept = false, // Chưa được xác nhận
                            userId = userId
                        };

                        _db.Orders.Add(order);
                        await _db.SaveChangesAsync();

                        if (await SaveOrderDetails(cart.Id, order.Id))
                        {
                            return View("Success");
                        }
                        else
                        {
                            Debug.WriteLine("loi");

                            return View("Error");
                        }
                    }



                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ngoai le" + ex.Message);
                    return View("Error");
                }

            }

            else
            {
                return View(model);

            }
        }
        [HttpPost]
        public ActionResult CreatePayment(string orderDescription, decimal amount)
        {
            // Lấy thông tin cấu hình
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            VnPayLibrary vnpay = new VnPayLibrary();

            // Chuyển IP từ IPv6 sang IPv4 nếu cần
            string ipAddress = Request.UserHostAddress;
            if (ipAddress == "::1") ipAddress = "127.0.0.1";

            // Thêm thông tin giao dịch
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng: {orderDescription}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", Guid.NewGuid().ToString()); // Unique TxnRef

            // Tạo URL thanh toán
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            Debug.WriteLine(paymentUrl);
            return Redirect(paymentUrl);
        }   


        public ActionResult PaymentResult()
        {
            string vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"];
            if (vnp_ResponseCode == "00")
            {
                ViewBag.Message = "Thanh toán thành công!";
            }
            else
            {
                ViewBag.Message = "Giao dịch không thành công.";
            }
            return View();
        }
        public async Task<bool> SaveOrderDetails(int cartId, int orderId)
        {
            var orderDetailItems = new List<OrderDetail>();
            var cartItems = await _db.shoppingCartItems
                .Where(c => c.ShoppingCartId == cartId && c.status == false)
                .ToListAsync();

            // Duyệt qua từng item trong giỏ hàng để tạo OrderDetail và cập nhật trạng thái
            foreach (var item in cartItems)
            {
                var newOrderItem = new OrderDetail
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Price = item.UnitPrice,
                    Quantity = item.Quantity,
                    numberSize = item.numberSize // Sử dụng numberSize từ giỏ hàng
                };

                // Cập nhật trạng thái của item trong giỏ hàng
                item.status = true;
                _db.Entry(item).State = EntityState.Modified;

                // Truy vấn bảng trung gian sizeOfProduct và liên kết với bảng size
                var sizeOfProduct = await _db.sizeOfProducts
                    .Include(sp => sp.size) // Bao gồm thông tin từ bảng size
                    .Include(sp => sp.product)
                    .FirstOrDefaultAsync(sp => sp.productId == item.ProductId && sp.size.numberSize == item.numberSize);

                if (sizeOfProduct != null)
                {
                    if (sizeOfProduct.product.quantity >= item.Quantity)
                    {
                        sizeOfProduct.product.quantity -= item.Quantity; // Trừ số lượng
                        _db.Entry(sizeOfProduct).State = EntityState.Modified; // Đánh dấu thay đổi
                    }
                    else
                    {
                        // Nếu không đủ số lượng, thông báo lỗi
                        Console.WriteLine($"Không đủ số lượng sản phẩm: {item.ProductId} (Size: {item.numberSize})");
                        return false;
                    }
                }
                else
                {
                    // Nếu không tìm thấy thông tin sizeOfProduct, thông báo lỗi
                    Console.WriteLine($"Không tìm thấy thông tin sản phẩm: {item.ProductId} (Size: {item.numberSize})");
                    return false;
                }

                // Thêm OrderDetail vào danh sách
                orderDetailItems.Add(newOrderItem);
            }

            try
            {
                // Thêm tất cả OrderDetail vào cơ sở dữ liệu
                _db.OrderDetails.AddRange(orderDetailItems);

                // Lưu tất cả các thay đổi vào cơ sở dữ liệu
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi để biết nguyên nhân nếu có
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        //public async Task<bool> SaveOrderDetails(int cartId, int orderId)
        //{
        //    var orderDetailItems = new List<OrderDetail>();
        //    var cartItems = await _db.shoppingCartItems.Where(c => c.ShoppingCartId == cartId && c.status == false).ToListAsync();


        //    // Duyệt qua từng item trong giỏ hàng để tạo OrderDetail và cập nhật trạng thái
        //    foreach (var item in cartItems)
        //    {
        //        var newOrderItem = new OrderDetail
        //        {
        //            OrderId = orderId,
        //            ProductId = item.ProductId,
        //            Price = item.UnitPrice,
        //            Quantity = item.Quantity,
        //            numberSize = item.numberSize
        //        };

        //        // Cập nhật trạng thái của item thành true
        //        item.status = true;

        //        // Thêm OrderDetail vào danh sách
        //        orderDetailItems.Add(newOrderItem);

        //        // Cập nhật trạng thái của ShoppingCartItem trong cơ sở dữ liệu
        //        _db.Entry(item).State = EntityState.Modified; // Cập nhật trạng thái của từng item
        //    }

        //    try
        //    {
        //        // Thêm tất cả OrderDetail vào cơ sở dữ liệu
        //        _db.OrderDetails.AddRange(orderDetailItems);

        //        // Lưu tất cả các thay đổi vào cơ sở dữ liệu
        //        await _db.SaveChangesAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log lỗi để biết nguyên nhân nếu có
        //        Console.WriteLine(ex.Message);
        //        return false;
        //    }
        //}



    }
}