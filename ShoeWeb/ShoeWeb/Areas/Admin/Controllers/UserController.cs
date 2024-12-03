using ShoeWeb.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Models;
using ShoeWeb.Areas.Admin.Admin_ViewModel;
using ShoeWeb.Utility;
using System.Net.Http;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ShoeWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public UserController() : this(new ApplicationDbContext()) { }

        // GET: Admin/User
        /*    public ActionResult Index()
            {
                return View();
            }
        */

        public async Task<ActionResult> Index()
        {
            // Lấy danh sách người dùng
            var users = await _db.Users.ToListAsync();

            // Lấy vai trò
            var userRoles = await _db.UserRoles.ToListAsync();
            var roles = await _db.Roles.ToListAsync();

            var availableRoles = roles.Select(r => r.Name).ToList();

            ViewBag.Roles = roles; // Gửi danh sách vai trò đến View

            // Kết hợp dữ liệu để tạo danh sách UserVM
            var userVMList = users.Select(user => new UserVM
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Status = user.Status, // Thêm trường này nếu tồn tại trong AspNetUsers
                RoleName = userRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Join(roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .FirstOrDefault() // Lấy tên vai trò đầu tiên nếu có nhiều
            }).ToList();

            return View(userVMList);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUser(string userId, bool status, string roleName)
        {
            // Tìm người dùng trong bảng AspNetUsers
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại!" });
            }

            // Cập nhật trạng thái người dùng
            if (user.Status != status)
            {
                user.Status = status;
                _db.Entry(user).State = EntityState.Modified;
            }

            // Tìm vai trò trong bảng AspNetRoles dựa trên tên vai trò
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleName);
            if (role == null)
            {
                return Json(new { success = false, message = "Vai trò không tồn tại!" });
            }

            try
            {
                // Xóa vai trò hiện tại trong bảng AspNetUserRoles
                var currentUserRole = await _db.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);
                if (currentUserRole != null)
                {
                    _db.UserRoles.Remove(currentUserRole);
                }

                // Thêm vai trò mới vào bảng AspNetUserRoles
                _db.UserRoles.Add(new IdentityUserRole
                {
                    UserId = userId,
                    RoleId = role.Id // Lưu ý rằng ở đây là RoleId từ bảng AspNetRoles
                });

                // Lưu thay đổi vào cơ sở dữ liệu
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }




    }
}