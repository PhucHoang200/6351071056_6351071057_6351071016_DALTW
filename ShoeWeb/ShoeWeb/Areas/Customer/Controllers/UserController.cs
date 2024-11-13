﻿using Microsoft.AspNet.Identity;
using ShoeWeb.Data;
using ShoeWeb.Identity;
using ShoeWeb.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShoeWeb.Areas.Customer.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public UserController() : this(new ApplicationDbContext())
        {
        }

        // GET: Customer/User
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<ActionResult> Register(string userName, string name, string phoneNumber, string email, string password)
        //{
        //    string salt = PasswordHasher.GenerateSalt();
        //    string hashedPassword = PasswordHasher.HashPasswordWithSalt(password, salt);

        //    try
        //    {
        //        var checkUser = await _db.users.FirstOrDefaultAsync(u => u.userName == userName);
        //        if (checkUser != null) {
        //            return Json(new {success = false, Message = "Tên người dùng đã tồn tại!"});
        //        }

        //        var user = new User
        //        {
        //            userName = userName,
        //            name = name,
        //            phoneNumber = phoneNumber,
        //            email = email,
        //            password = hashedPassword,
        //            randomKey = salt
        //        };  

        //        _db.users.Add(user);
        //        await _db.SaveChangesAsync();

        //        var roleUser = new UserRole()
        //        {
        //            IdRole = await _db.roles.Where(r => r.NameRole == SD.AdminRole).Select(r => r.RoleId).FirstOrDefaultAsync(),
        //            UserId = user.userId
        //        };

        //        _db.userRoles.Add(roleUser);
        //        await _db.SaveChangesAsync();

        //        return Json(new { success = true, Message = "Đăng ký thành công" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, Message = "Đăng ký thất bại. Lỗi: " + ex.Message });
        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult> Login(string userName, string password)
        //{
        //    try
        //    {
        //        userName = userName.Trim();
        //        password = password.Trim();

        //        var user = await _db.users.FirstOrDefaultAsync(u => u.userName.Equals(userName, StringComparison.OrdinalIgnoreCase));

        //        if (user == null)
        //            return Json(new { success = false, error = "User not found" });

        //        var roleUser = await _db.userRoles.FirstOrDefaultAsync(r => r.UserId == user.userId);
        //        if (roleUser == null)
        //            return Json(new { success = false, error = "User role not found" });

        //        bool isValid = ValidateUser(user, password);
        //        if (isValid)
        //        {
        //            // Đăng nhập thành công, sinh token
        //            var role = await _db.roles.FirstOrDefaultAsync(r => r.RoleId == roleUser.IdRole);
        //            var token = JwtAuthManagerProvider.JwtAuthManager.GenerateToken(user, role);
        //            return Json(new { success = true, token = token, Message = "Đăng nhập thành công" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, error = "Invalid credentials" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, error = $"An error occurred: {ex.Message}" });
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> Register(string userName, string name, string phoneNumber, string email, string password)
        {
            try
            {
                var appDbContext = new AppDbContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);

                // Tạo đối tượng User
                var user = new AppUser
                {
                    UserName = userName,
                    PhoneNumber = phoneNumber,
                    Email = email
                };

                // Tạo user mới và gán mật khẩu
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Gán vai trò mặc định cho user
                    await userManager.AddToRoleAsync(user.Id, SD.CustomerRole);

                    return Json(new { success = true, message = "Đăng ký thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Đăng ký thất bại: " + string.Join(", ", result.Errors) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<ActionResult> Login(string userName, string password)
        {
            try
            {
                var appDbContext = new AppDbContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);

                // Tìm user theo userName
                var user = await userManager.FindByNameAsync(userName);

                // Kiểm tra user có tồn tại và mật khẩu có hợp lệ không
                if (user != null && await userManager.CheckPasswordAsync(user, password))
                {
                    // Tiến hành đăng nhập
                    var authenManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties(), userIdentity);

                    //Lưu tên người dùng vào session
                    Session["LoggedUserName"] = user.UserName;

                    // Lấy vai trò của user
                    var roles = await userManager.GetRolesAsync(user.Id); // Lấy danh sách các vai trò
                    var role = roles.FirstOrDefault(); // Lấy vai trò đầu tiên (nếu có)

                    return Json(new { success = true, role = role, message = "Đăng nhập thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Đăng nhập thất bại" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }

        
        public ActionResult Logout()
        {
            //var authenManager = HttpContext.GetOwinContext().Authentication;
            //authenManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Remove("LoggedUserName"); // Xóa tất cả dữ liệu trong Session
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UserInformation()
        {
            return View();
        }

        //private bool ValidateUser(User user, string password)
        //{
        //    try
        //    {
        //        var hashedPassword = PasswordHasher.HashPasswordWithSalt(password, user.randomKey);

        //        // In ra thông tin
        //        System.Diagnostics.Debug.WriteLine($"Muối: {user.randomKey}");
        //        System.Diagnostics.Debug.WriteLine($"Mật khẩu nhập vào: {password}");
        //        System.Diagnostics.Debug.WriteLine($"Mật khẩu đã băm: {hashedPassword}");

        //        return user.password.Equals(hashedPassword);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error validating user: {ex.Message}");
        //        return false;
        //    }
        //}
    }
}