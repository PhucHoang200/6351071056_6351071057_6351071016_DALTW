using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ShoeWeb.App_Start;
using ShoeWeb.Data;
using ShoeWeb.Identity;
using ShoeWeb.Models.Identity;
using ShoeWeb.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Areas.Customer.CustomerVM;
using System.Data.Entity;
using System.Web.Security;
using ShoeWeb.Helper;
using ShoeWeb.Models;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace ShoeWeb.Areas.Customer.Controllers
{
    [Authorize]

    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public UserController() : this(new ApplicationDbContext())
        {
        }

        public UserController(ApplicationDbContext db, ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _db = db;
            UserManager = userManager;
            SignInManager = signInManager;
        }

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

        //
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new RegisterViewModel());
        }

        //
        // POST: /u/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            // Kiểm tra định dạng email
            if (!new EmailAddressAttribute().IsValid(model.Email))
            {
                ModelState.AddModelError("", "Email không đúng định dạng.");
                //Xóa thông tin trong trường email
                ModelState.Remove("Email");
                model.Email = string.Empty;

            }

            // Nếu ModelState không hợp lệ, trả về lại trang login với các lỗi
            if (!ModelState.IsValid)

            {
                return View(model);
            }

            // Tìm người dùng dựa trên Email
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại.");
                //Xóa thông tin trong trường email
                ModelState.Remove("Email");
                model.Email = string.Empty;
                return View(model);
            }

            // Kiểm tra trạng thái tài khoản
            if (user.Status) // Nếu `Status` là true, tài khoản bị vô hiệu hóa
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.");
                //Xóa thông tin trong trường email
                ModelState.Remove("Email");
                model.Email = string.Empty;
                return View(model);
            }

            // Lấy vai trò của người dùng
            var role = await UserManager.GetRolesAsync(user.Id);

            // Kiểm tra xác nhận Email
            if (!user.EmailConfirmed)
            {
                Response.Cookies.Remove("__RequestVerificationToken");
                SignInManager.AuthenticationManager.SignOut();
                ViewBag.ErrorMessage = "Email của bạn chưa được xác nhận.";
                ViewBag.ResendEmail = true;
                ViewBag.UserEmail = model.Email; // Chuyển email cho liên kết gửi lại mail
                return View(model); // Giữ người dùng lại trang Login
            }

            // Thực hiện đăng nhập
            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:

                    if (role.Contains(SD.AdminRole))
                    {
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });
                    }
                    TempData["LoginMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");

                    //Xóa thông tin trong trường email
                    ModelState.Remove("Email");
                    model.Email = string.Empty;

                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ResendConfirmationEmail(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null && !user.EmailConfirmed)
            {
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "User", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                SendMail.SendEmail(email, "Xác nhận tài khoản của bạn", $"Vui lòng nhấp vào <a href='{callbackUrl}'>đây</a> để xác nhận tài khoản.", "");
            }
            return View("EmailCheck");
        }



        //
        // POST: /User/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {


                // Kiểm tra xem email đã tồn tại trong cơ sở dữ liệu chưa
                var existingUser = await UserManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    // Nếu email đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Email", "Email này đã được đăng ký trước đó. Vui lòng sử dụng email khác.");
                    return View(model);
                }

                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    await UserManager.AddToRoleAsync(user.Id, SD.CustomerRole);

                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "User", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Xác thực tài khoản", "Vui lòng click vào <a href=\"" + callbackUrl + "\">here</a> để xác thực đăng kí tài khoản");
                    //ViewBag.ThongBao = "Chúng tôi đã gửi cho bạn 1 email để xác thực. Vui lòng kiểm tra!";

                    SendMail.SendEmail(model.Email, "Xác nhận tài khoản của bạn", $"Vui lòng nhấp vào <a href='{callbackUrl}'>đây</a> để xác nhận tài khoản.", "");
                    return View("EmailCheck");
                }

                // Nếu tạo tài khoản không thành công, thêm lỗi vào ModelState
                AddErrors(result);
            }

            // Nếu model không hợp lệ hoặc có lỗi, hiển thị lại form đăng ký với lỗi
            return View(model);
        }


        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }


        // GET: ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ShoeWeb.Areas.Customer.CustomerVM.ForgotPasswordViewModel model)
        {
            // Kiểm tra validation từ DataAnnotations
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra xem email có tồn tại trong cơ sở dữ liệu không
            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                // Thêm lỗi tùy chỉnh khi email không tồn tại
                ModelState.AddModelError("Email", "Email không tồn tại.");
                return View(model);
            }

            // Lưu email vào TempData trước khi chuyển hướng
            TempData["Email"] = model.Email;

            // Tiếp tục xử lý logic gửi OTP
            var otpCode = GenerateOtp();
            var otp = new OTP
            {
                UserId = user.Id,
                OTPCode = otpCode,
                ExpiryDate = DateTime.Now.AddMinutes(5),
                IsUsed = false
            };
            _db.oTPs.Add(otp);
            _db.SaveChanges();

            // Gửi OTP qua email
            string subject = "Your OTP Code";
            string body = $"Mã OTP của bạn là: {otpCode}. Vui lòng sử dụng mã này để đặt lại mật khẩu.";
            SendMail.SendEmail(user.Email, subject, body, "");

            HttpContext.Session["Email"] = model.Email;
            return RedirectToAction("ConfirmOtp");
        }


        // GET: ConfirmOtp
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmOtp()
        {
            return View();
        }


        // POST: ConfirmOtp
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmOtp(ShoeWeb.Areas.Customer.CustomerVM.ConfirmOtpViewModel model)
        {
            // Kiểm tra validation từ DataAnnotations
            if (!ModelState.IsValid)
            {
                return View(model); // Trả lại view nếu OtpCode trống hoặc không hợp lệ
            }

            // Lấy email từ TempData
            var email = HttpContext.Session["Email"]?.ToString();
            Debug.WriteLine(email);
            if (email == null)
            {
                Debug.WriteLine("no");

                return RedirectToAction("ForgotPassword"); // Nếu email không tồn tại trong TempData, chuyển hướng về trang ForgotPassword
            }

            // Tìm user dựa trên email
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return View(model);
            }

            // Kiểm tra mã OTP còn hạn không
            var otp = _db.oTPs.FirstOrDefault(o =>
                o.UserId == user.Id &&
                o.ExpiryDate > DateTime.Now && // Kiểm tra OTP còn hạn
                !o.IsUsed); // Kiểm tra OTP chưa sử dụng

            if (otp == null)
            {
                ModelState.AddModelError("OtpCode", "Mã OTP không hợp lệ hoặc đã hết hạn.");
                HttpContext.Session["Email"] = email;
                return View(model);
            }

            // Kiểm tra mã OTP có khớp không
            if (otp.OTPCode != model.OtpCode)
            {
                ModelState.AddModelError("OtpCode", "Mã OTP không chính xác.");
                HttpContext.Session["Email"] = email;
                return View(model);
            }

            // Đánh dấu OTP đã sử dụng
            otp.IsUsed = true;
            _db.SaveChanges();

            TempData["UserId"] = user.Id; // Lưu UserId để sử dụng trong trang ResetPassword
            HttpContext.Session["UserId"] = user.Id;

            return RedirectToAction("ResetPassword"); // Chuyển hướng đến trang reset mật khẩu
        }






        // GET: ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            var email = HttpContext.Session["Email"]?.ToString();
            var user = UserManager.FindByEmail(email);

          

            // Kiểm tra nếu người dùng không tồn tại hoặc chưa xác nhận email
            if (user == null || !user.EmailConfirmed || email == null)
            {
                TempData["Error"] = "Vui lòng xác nhận email trước khi thực hiện thao tác này.";
                return RedirectToAction("ForgotPassword", "User", new { area = "Customer" });
            }

            
            // Nếu mọi thứ hợp lệ, trả về view
            return View();
        }


        //// POST: ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ShoeWeb.Areas.Customer.CustomerVM.ResetPasswordViewModel model)
        {
            // Kiểm tra validation từ DataAnnotations
            if (!ModelState.IsValid)
            {
                return View(model); // Trả lại view nếu có lỗi validation từ ViewModel
            }

            // Kiểm tra mật khẩu mới và mật khẩu xác nhận có trùng khớp không
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu không trùng khớp.");
                return View(model); // Trả lại view và hiển thị lỗi
            }

            // Lấy userId từ TempData
            var userId = HttpContext.Session["UserId"]?.ToString();
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null) return RedirectToAction("ForgotPassword");

            // Mã hóa mật khẩu mới và lưu vào database
            user.PasswordHash = new Microsoft.AspNet.Identity.PasswordHasher().HashPassword(model.NewPassword);
            _db.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }


        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Response.Cookies.Remove("__RequestVerificationToken");
            SignInManager.AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

    }
}