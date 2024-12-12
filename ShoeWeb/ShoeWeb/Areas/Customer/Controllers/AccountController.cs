using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ShoeWeb.Data;
using ShoeWeb.Areas.Customer.CustomertVM;
using ShoeWeb.Models;
using ShoeWeb.Areas.Customer.CustomerVM;
using ShoeWeb.Models.Identity;
using ShoeWeb.Helper;
using System.Threading.Tasks;
using ShoeWeb.App_Start;
using Microsoft.AspNet.Identity.EntityFramework;
using ShoeWeb.Identity;
using Microsoft.AspNet.Identity.Owin;
using ShoeWeb.Utility;
using System.Web.Helpers;
using System.Diagnostics;
using System.Data.Entity;

namespace ShoeWeb.Areas.Customer.Controllers
{
    [Authorize(Roles = SD.CustomerRole)]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private ApplicationSignInManager _signInManager;


        // Constructor với UserManager
        public AccountController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));  // Đảm bảo _userManager không phải null\
            
        }

        // Constructor mặc định (có thể không cần nữa nếu bạn tiêm dependency qua constructor)
        public AccountController() : this(new ApplicationDbContext(), new UserManager<AppUser>(new UserStore<AppUser>(new ApplicationDbContext())))
        { }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ActionResult ProfileInformation()
        {

            return View();
        }
        public async Task<List<OrderDetail>> GetOrderWaitingAccept(string userId)
        {
            try
            {
                var ordersWaiting = await _db.Orders
                     .Where(o => o.userId == userId && o.isAccept == false)
                     .ToListAsync();
                // Lấy danh sách orderId từ các đơn hàng
                var orderIds = ordersWaiting.Select(o => o.Id).ToList();

                var orderItemsWaiting = await _db.OrderDetails
                                    .Where(oi => orderIds.Contains(oi.OrderId))
                                    .Include(oi => oi.Product)
                                    .ToListAsync();
                return orderItemsWaiting;
            }
            catch (Exception ex)
            {
                return new List<OrderDetail>();
            }
        }
          public async Task<List<OrderDetail>> GetOrderShipping(string userId)
        {
            try
            {
                var ordersWaiting = await _db.Orders
                     .Where(o => o.userId == userId && o.StatusShipping == 2)
                     .ToListAsync();
                // Lấy danh sách orderId từ các đơn hàng
                var orderIds = ordersWaiting.Select(o => o.Id).ToList();

                var orderItemsWaiting = await _db.OrderDetails
                                    .Where(oi => orderIds.Contains(oi.OrderId))
                                    .Include(oi => oi.Product)
                                    .ToListAsync();
                return orderItemsWaiting;
            }
            catch (Exception ex)
            {
                return new List<OrderDetail>();
            }
        }

        // GET: Customer/Account
        [HttpGet]
        public async Task<ActionResult> UserInformation()
        {
            var userId = User.Identity.GetUserId();
            Debug.WriteLine(userId);

            var user = await _userManager.FindByIdAsync(userId);
            var ordersSuccess = await _db.Orders
                       .Where(o => o.userId == userId && o.StatusShipping == 3)
                       .ToListAsync();
            // Lấy danh sách orderId từ các đơn hàng
            var orderIds = ordersSuccess.Select(o => o.Id).ToList();

            // Lấy các orderItems có orderId trùng với các orderId vừa lấy
            var orderItemsSuccess = await _db.OrderDetails
                                      .Where(oi => orderIds.Contains(oi.OrderId))
                                      .Include(oi => oi.Product)
                                      .ToListAsync();





            var model = new UserProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                orderItem_success = orderItemsSuccess ?? new List<OrderDetail>(),
                orderItem_waitingAccept = await GetOrderWaitingAccept(userId),
                orderItem_shipping = await GetOrderShipping(userId)

            };

            return View(model);
        }

        // Cập nhật thông tin người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(string UserName, string Email, string PhoneNumber)
        {
            if (UserName == "" || Email == "" || PhoneNumber == "")
            {
                TempData["Error"] = "Cập nhật thông tin thất bại. Vui lòng điền đầy đủ thông tin.";
                return RedirectToAction("UserInformation");

            }
            if (ModelState.IsValid)
            {
                var user = _db.Users.Find(User.Identity.GetUserId());
                if (user != null)
                {
                    user.PhoneNumber = PhoneNumber;
                    user.UserName = UserName;

                    var isUserNameTaken = _db.Users.Any(u => u.UserName == UserName && u.Id != user.Id);
                    if (isUserNameTaken)
                    {
                        ModelState.AddModelError("UserName", "Tên người dùng đã được sử dụng.");
                        ViewBag.ActiveTab = "account-general";
                        return View("UserInformation", new UserProfileViewModel
                        {
                            UserName = UserName,
                            Email = Email,
                            PhoneNumber = PhoneNumber
                        });
                    }

                    _db.SaveChanges();
                    Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    // Đăng nhập lại với thông tin mới
                    var signInManager = new SignInManager<AppUser, string>(_userManager, Request.GetOwinContext().Authentication);
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    TempData["Success"] = "Thông tin của bạn đã được cập nhật.";
                    return RedirectToAction("UserInformation");
                }
            }
            ViewBag.ActiveTab = "account-general";
            TempData["Error"] = "Cập nhật thông tin thất bại. ";
            return View("UserInformation");
        }



        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Customer/Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string CurrentPassword, string NewPassword, string RepeatPassword)
        {
            

            if (!ModelState.IsValid)
            {
                
                ViewBag.ActiveTab = "account-change-password";
                return View("UserInformation");
            }

            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            var model = new UserProfileViewModel()
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                ViewBag.ActiveTab = "account-change-password";
                return RedirectToAction("UserInformation", model);
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, CurrentPassword);

            if (!passwordValid)
            {
                ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                TempData["Error"] = "Mật khẩu hiện tại không đúng.";

                ViewBag.ActiveTab = "account-change-password";
                return View("UserInformation", model);
            }

            if (NewPassword != RepeatPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu mới và xác nhận mật khẩu không trùng khớp.");
                TempData["Error"] = "Mật khẩu mới và xác nhận mật khẩu không trùng khớp.";

                ViewBag.ActiveTab = "account-change-password";
                return View("UserInformation", model);
            }

            var result = await _userManager.ChangePasswordAsync(userId, CurrentPassword, NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Mật khẩu đã được thay đổi.";
                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Login", "User", new { area = "Customer" });
            }

            TempData["Error"] = "Đổi mật khẩu thất bại.";
            ViewBag.ActiveTab = "account-change-password";
            return View("UserInformation", model);
        }


        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ConfirmEmail(UserProfileViewModel model)
        {
            if (model == null)
            {
                return View(new UserProfileViewModel());
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ConfirmEmail(string email)
        {
            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            // Kiểm tra email hợp lệ
            if (user == null || user.Email != email)
            {
                TempData["Error"] = "Email sai";
                return View();
            }

            // Chuyển hướng đến ForgotPassword
            return RedirectToAction("ForgotPassword", new { email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Vui lòng nhập email.";
                return RedirectToAction("ForgotPassword");
            }

            var user = await _userManager.FindByEmailAsync(email); // Đổi tên biến từ User -> user

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user.Id))
            {
                TempData["Error"] = "Không tìm thấy người dùng hoặc email chưa được xác nhận.";
                return RedirectToAction("ForgotPassword");
            }

            // Sinh mã OTP
            string otpCode = GenerateOtp();

            // Lưu OTP vào cơ sở dữ liệu
            var otp = new OTP
            {
                UserId = user.Id,
                OTPCode = otpCode,
                ExpiryDate = DateTime.Now.AddMinutes(5),
                IsUsed = false
            };

            _db.oTPs.Add(otp);
            await _db.SaveChangesAsync();

            // Gửi email chứa OTP
            string subject = "Mã OTP của bạn";
            string body = $"Mã OTP của bạn là: <b>{otpCode}</b>. Vui lòng nhập mã này để tiếp tục.";
            SendMail.SendEmail(email, subject, body, "");

            TempData["Success"] = "Mã OTP đã được gửi đến email của bạn.";
            return RedirectToAction("ConfirmOtp", "Account", new { area = "Customer" });
        }


        [HttpGet]
        public async Task<ActionResult> ConfirmOtp(ShoeWeb.Areas.Customer.CustomerVM.UserProfileViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại.";
                return RedirectToAction("Login");
            }

            // Kiểm tra OTP chưa sử dụng
            var existingOtp = _db.oTPs.FirstOrDefault(o => o.UserId == user.Id && !o.IsUsed && o.ExpiryDate > DateTime.Now);
            if (existingOtp != null)
            {
                TempData["Error"] = "Bạn đã yêu cầu OTP trước đó. Vui lòng kiểm tra email của bạn.";
                return View(model);
            }

            // Sinh mã OTP mới
            string otpCode = GenerateOtp();

            // Lưu OTP mới vào cơ sở dữ liệu
            var otp = new OTP
            {
                UserId = user.Id,
                OTPCode = otpCode,
                ExpiryDate = DateTime.Now.AddMinutes(5), // 5 phút hiệu lực
                IsUsed = false
            };
            _db.oTPs.Add(otp);
            await _db.SaveChangesAsync();

            // Gửi OTP qua email
            string subject = "Mã OTP của bạn";
            string body = $"Mã OTP của bạn là: <b>{otpCode}</b>. Vui lòng nhập mã này để tiếp tục.";
            SendMail.SendEmail(user.Email, subject, body, "");

            TempData["Success"] = "Mã OTP đã được gửi đến email của bạn.";
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmOtp(string otpCode)
        {
            var userId = User.Identity.GetUserId();

            // Tìm OTP trong cơ sở dữ liệu
            var otp = _db.oTPs.FirstOrDefault(o => o.UserId == userId &&
                                                    o.OTPCode == otpCode &&
                                                    o.ExpiryDate > DateTime.Now &&
                                                    !o.IsUsed);

            if (otp == null)
            {
                TempData["Error"] = "Mã OTP không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("ConfirmOtp");
            }

            otp.IsUsed = true;
            _db.SaveChanges();


            TempData["OtpCode"] = otpCode;
            return RedirectToAction("InputPassword");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string NewPassword, string ConfirmPassword)
        {

            if (NewPassword == "" || ConfirmPassword == "")
            {
                TempData["Error"] = "Không được để trống";
                return RedirectToAction("InputPassword");
            }

            // Kiểm tra mật khẩu mới và mật khẩu xác nhận có trùng khớp không
            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu không trùng khớp.");
                return RedirectToAction("InputPassword"); // Trả lại view và hiển thị lỗi
            }

            // Lấy userId từ TempData
            var userId = User.Identity.GetUserId();
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null) return RedirectToAction("InputPassword");

            // Mã hóa mật khẩu mới và lưu vào database
            user.PasswordHash = new Microsoft.AspNet.Identity.PasswordHasher().HashPassword(NewPassword);
            _db.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập.";
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public async Task<ActionResult> InputPassword(CustomerVM.ChangePasswordViewModel model)
        {
            // Kiểm tra nếu model null và khởi tạo mới
            if (model == null)
            {
                model = new CustomerVM.ChangePasswordViewModel();
            }

            // Trả về view với model
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendOtp()
        {
            // Lấy userId từ session hoặc claim
            var userId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Bạn phải đăng nhập để yêu cầu OTP.";
                return RedirectToAction("Login", "User", new { area = "Customer" });
            }

            // Tạo mã OTP mới
            var otpCode = GenerateOtp();

            // Đặt thời gian hết hạn cho OTP (ví dụ: 5 phút)
            var expiryDate = DateTime.Now.AddMinutes(5);

            // Lưu OTP vào cơ sở dữ liệu
            var otp = new OTP
            {
                UserId = userId,
                OTPCode = otpCode,
                ExpiryDate = expiryDate,
                IsUsed = false
            };

            _db.oTPs.Add(otp);
            await _db.SaveChangesAsync();

            // Gửi OTP qua email (hoặc SMS nếu cần)
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Login", "User", new { area = "Customer" });
            }

            // Gửi OTP qua email
            var emailSubject = "Mã OTP để thay đổi mật khẩu";
            var emailBody = $"Mã OTP của bạn là: {otpCode}. Mã này sẽ hết hạn sau 5 phút.";

            SendMail.SendEmail(user.Email, emailSubject, emailBody, "");

            TempData["Success"] = "Mã OTP đã được gửi lại vào email của bạn.";
            return RedirectToAction("ConfirmOtp", "Account");
        }
    }
}