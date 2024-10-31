using ShoeWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ShoeWeb.Data;
using ShoeWeb.Models;
using ShoeWeb.Utility;
using System.Data.Entity;

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

        [HttpPost]
        public async Task<ActionResult> Register(string userName, string name, string phoneNumber, string email, string password)
        {
            string salt = PasswordHasher.GenerateSalt();
            string hashedPassword = PasswordHasher.HashPasswordWithSalt(password, salt);

            try
            {
                var checkUser = await _db.users.FirstOrDefaultAsync(u => u.userName == userName);
                if (checkUser != null) {
                    return Json(new {success = false, Message = "Tên người dùng đã tồn tại!"});
                }

                var user = new User
                {
                    userName = userName,
                    name = name,
                    phoneNumber = phoneNumber,
                    email = email,
                    password = hashedPassword,
                    randomKey = salt
                };  

                _db.users.Add(user);
                await _db.SaveChangesAsync();

                var roleUser = new UserRole()
                {
                    IdRole = await _db.roles.Where(r => r.NameRole == SD.AdminRole).Select(r => r.RoleId).FirstOrDefaultAsync(),
                    UserId = user.userId
                };

                _db.userRoles.Add(roleUser);
                await _db.SaveChangesAsync();

                return Json(new { success = true, Message = "Đăng ký thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = "Đăng ký thất bại. Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Login(string userName, string password)
        {
            try
            {
                userName = userName.Trim();
                password = password.Trim();

                var user = await _db.users.FirstOrDefaultAsync(u => u.userName.Equals(userName, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                    return Json(new { success = false, error = "User not found" });

                var roleUser = await _db.userRoles.FirstOrDefaultAsync(r => r.UserId == user.userId);
                if (roleUser == null)
                    return Json(new { success = false, error = "User role not found" });

                bool isValid = ValidateUser(user, password);
                if (isValid)
                {
                    // Đăng nhập thành công, sinh token
                    var role = await _db.roles.FirstOrDefaultAsync(r => r.RoleId == roleUser.IdRole);
                    var token = JwtAuthManagerProvider.JwtAuthManager.GenerateToken(user, role);
                    return Json(new { success = true, token = token, Message = "Đăng nhập thành công" });
                }
                else
                {
                    return Json(new { success = false, error = "Invalid credentials" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }

        private bool ValidateUser(User user, string password)
        {
            try
            {
                var hashedPassword = PasswordHasher.HashPasswordWithSalt(password, user.randomKey);

                // In ra thông tin
                System.Diagnostics.Debug.WriteLine($"Muối: {user.randomKey}");
                System.Diagnostics.Debug.WriteLine($"Mật khẩu nhập vào: {password}");
                System.Diagnostics.Debug.WriteLine($"Mật khẩu đã băm: {hashedPassword}");

                return user.password.Equals(hashedPassword);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating user: {ex.Message}");
                return false;
            }
        }
    }
}
