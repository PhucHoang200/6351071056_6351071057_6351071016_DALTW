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

namespace ShoeWeb.Areas.Customer.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        // Constructor với UserManager
        public AccountController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));  // Đảm bảo _userManager không phải null
        }

        // Constructor mặc định (có thể không cần nữa nếu bạn tiêm dependency qua constructor)
        public AccountController() : this(new ApplicationDbContext(), new UserManager<AppUser>(new UserStore<AppUser>(new ApplicationDbContext())))
        { }

        public ActionResult ProfileInformation()
        {
            var userId = User.Identity.GetUserId();
            var user = _db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,

            };

            return View(model);
        }

        // GET: Customer/Account
        [HttpGet]
        public ActionResult UserInformation()
        {
            var userId = User.Identity.GetUserId();
            var user = _db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,

            };

            return View(model);
        }

        // Cập nhật thông tin người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin người dùng hiện tại
                var user = _db.Users.Find(User.Identity.GetUserId());
                if (user != null)
                {
                    user.PhoneNumber = model.PhoneNumber;
                    user.UserName = model.UserName;

                    // Kiểm tra tính hợp lệ và duy nhất của UserName
                    var isUserNameTaken = _db.Users.Any(u => u.UserName == model.UserName && u.Id != user.Id);
                    if (isUserNameTaken)
                    {
                        ModelState.AddModelError("UserName", "Tên đăng nhập đã được sử dụng.");
                        return View(model);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _db.SaveChanges();

                    // Đăng xuất người dùng hiện tại
                    Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    // Đăng nhập lại với thông tin mới
                    var signInManager = new SignInManager<AppUser, string>(_userManager, Request.GetOwinContext().Authentication);
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    TempData["Success"] = "Thông tin của bạn đã được cập nhật và hệ thống đã đăng nhập lại.";
                    return RedirectToAction("UserInformation");
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy người dùng.";
                }
            }
            else
            {
                TempData["Error"] = "Thông tin không hợp lệ.";
            }

            return RedirectToAction("UserInformation");
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
        public async Task<ActionResult> ChangePassword(ShoeWeb.Areas.Customer.CustomerVM.ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy người dùng.";
                    return RedirectToAction("ChangePassword");
                }

                // Kiểm tra mật khẩu cũ
                var passwordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                if (!passwordValid)
                {
                    ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng.");
                    return View(model);
                }

                // Kiểm tra mật khẩu mới và xác nhận mật khẩu
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("NewPassword", "Mật khẩu mới và xác nhận mật khẩu không trùng khớp.");
                    return View(model);
                }

                // Đổi mật khẩu
                var result = await _userManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    // Đăng xuất người dùng
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    TempData["Success"] = "Mật khẩu đã được thay đổi. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "User", new { area = "Customer" });
                }

                // Nếu có lỗi khi thay đổi mật khẩu
                TempData["Error"] = "Đổi mật khẩu thất bại. Vui lòng thử lại.";
                return RedirectToAction("ChangePassword");
            }

            // Nếu ModelState không hợp lệ
            return View(model);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string NewPassword)
        {
            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || !user.EmailConfirmed)
            {
                TempData["Error"] = "Không tìm thấy người dùng hoặc email chưa được xác nhận.";
                return RedirectToAction("ForgotPassword");
            }

            // Tạo mã OTP
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
            _db.SaveChanges();

            // Lưu mật khẩu mới vào TempData
            TempData["NewPassword"] = NewPassword;

            // Gửi email
            string subject = "Mã OTP của bạn";
            string body = $"Mã OTP của bạn là: <b>{otpCode}</b>. Vui lòng nhập mã này để tiếp tục.";
            SendMail.SendEmail(user.Email, subject, body, "");

            TempData["Success"] = "Mã OTP đã được gửi đến email của bạn.";
            return RedirectToAction("ConfirmOtp", "Account", new { area = "Customer" });
        }

        [HttpGet]
        public ActionResult ConfirmOtp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmOtp(string OtpCode)
        {
            var userId = User.Identity.GetUserId();

            // Tìm OTP trong cơ sở dữ liệu
            var otp = _db.oTPs.FirstOrDefault(o => o.UserId == userId &&
                                                  o.OTPCode == OtpCode &&
                                                  o.ExpiryDate > DateTime.Now &&
                                                  !o.IsUsed);

            if (otp == null)
            {
                TempData["Error"] = "Mã OTP không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("ConfirmOtp");
            }

            // Lấy mật khẩu mới từ TempData
            if (TempData["NewPassword"] == null)
            {
                TempData["Error"] = "Không tìm thấy mật khẩu mới. Vui lòng thử lại.";
                return RedirectToAction("ForgotPassword");
            }

            var newPassword = TempData["NewPassword"].ToString();

            // Tìm người dùng
            var user = _db.Users.Find(userId);
            if (user != null)
            {
                // Đổi mật khẩu
                user.PasswordHash = new Microsoft.AspNet.Identity.PasswordHasher().HashPassword(newPassword);

                // Đánh dấu OTP đã sử dụng
                otp.IsUsed = true;
                _db.SaveChanges();

                // Đăng xuất người dùng
                var authManager = HttpContext.GetOwinContext().Authentication;
                authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                TempData["Success"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "User", new { area = "Customer" });
            }

            TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
            return RedirectToAction("ConfirmOtp");
        }




    }
}