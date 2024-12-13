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
            // Lấy vai trò Customer
            var roleCustomer = await _db.Roles.FirstOrDefaultAsync(rl => rl.Name == "Customer");
            if (roleCustomer == null)
            {
                ViewBag.Roles = new List<IdentityRole>(); // Khởi tạo danh sách rỗng nếu không có vai trò
                return View(new List<UserVM>()); // Trả về danh sách rỗng
            }

            // Lấy danh sách người dùng có vai trò Customer
            var userRoles = await _db.UserRoles
                .Where(ur => ur.RoleId == roleCustomer.Id)
                .ToListAsync();

            var userIds = userRoles.Select(ur => ur.UserId).ToList();

            var users = await _db.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            // Lấy danh sách tất cả vai trò
            var roles = await _db.Roles.ToListAsync();
            ViewBag.Roles = roles; // Gán danh sách vai trò cho ViewBag.Roles

            // Chuẩn bị danh sách UserVM
            var userVMList = users.Select(user => new UserVM
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Status = user.Status, // Nếu có trường Status
                RoleName = "Customer" // Mặc định là Customer vì đã lọc
            }).ToList();

            // Gửi danh sách UserVM đến View
            return View(userVMList);
        }


        [HttpPost]
        public async Task<ActionResult> ChangeStatus(string userId, bool status)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if(user == null)
                {
                    return Json(new { success = false, message = "Người dùng không tồn tại!" });
                }

                user.Status = status;
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message});

            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUser(string userId, string roleId)
        {
            // Tìm người dùng trong bảng AspNetUsers
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại!" });
            }



            // Tìm vai trò trong bảng AspNetRoles dựa trên tên vai trò
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
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