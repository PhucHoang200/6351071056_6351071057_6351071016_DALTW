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
        public ActionResult UpdateProfile(UserProfileViewModel model)
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
                    TempData["Success"] = "Thông tin của bạn đã được cập nhật thành công.";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendOtp(string newPassword)
        {
            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || !user.EmailConfirmed)
            {
                TempData["Error"] = "Không tìm thấy người dùng hoặc email chưa được xác nhận.";
                return RedirectToAction("ChangePassword");
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
            TempData["NewPassword"] = newPassword;

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
        public ActionResult ConfirmOtp(string otpCode)
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

            // Lấy mật khẩu mới từ TempData
            if (TempData["NewPassword"] == null)
            {
                TempData["Error"] = "Không tìm thấy mật khẩu mới. Vui lòng thử lại.";
                return RedirectToAction("ChangePassword");
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendOtp()
        {
            // Lấy UserId từ thông tin đăng nhập hiện tại
            var userId = User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId); // Sử dụng UserManager để tìm người dùng

            // Kiểm tra nếu không tìm thấy người dùng
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            // Kiểm tra nếu email chưa xác nhận
            if (user.EmailConfirmed == false)
            {
                // Tạo mã OTP ngẫu nhiên
                Random random = new Random();
                string otpCode = random.Next(100000, 999999).ToString();

                // Lưu OTP vào cơ sở dữ liệu hoặc Session
                var otp = new OTP
                {
                    UserId = userId,
                    OTPCode = otpCode,
                    ExpiryDate = DateTime.Now.AddMinutes(5), // Hạn sử dụng OTP (5 phút)
                    IsUsed = false
                };

                // Lưu OTP vào cơ sở dữ liệu
                _db.oTPs.Add(otp);
                _db.SaveChanges();

                // Tạo nội dung email
                string subject = "Mã OTP của bạn";
                string body = $"Mã OTP của bạn là: {otpCode}. Vui lòng nhập mã này để tiếp tục.";

                // Gửi OTP qua email bằng phương thức SendMail.SendEmail
                bool emailSent = SendMail.SendEmail(user.Email, subject, body, "");

                if (emailSent)
                {
                    return Json(new { success = true, message = "Mã OTP đã được gửi lại." });
                }
                else
                {
                    return Json(new { success = false, message = "Đã có lỗi khi gửi email. Vui lòng thử lại." });
                }
            }

            return Json(new { success = false, message = "Email đã được xác nhận." });
        }



    }
}