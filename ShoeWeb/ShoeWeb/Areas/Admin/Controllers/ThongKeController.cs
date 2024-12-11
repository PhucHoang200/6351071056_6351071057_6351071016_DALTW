using ShoeWeb.Areas.Admin.Admin_ViewModel;
using ShoeWeb.Data;
using ShoeWeb.Models;
using ShoeWeb.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShoeWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminRole)]

    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ThongKeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public ThongKeController() : this(new ApplicationDbContext())
        {

        }
        // GET: Admin/ThongKe
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DoanhSo()
        {

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Statistic_TheoVung()
        {
            try
            {
                List<StatisticVM> orders;
                int currentYear = DateTime.Now.Year; // Lấy năm hiện tại

                // Lấy dữ liệu của năm nay
                var monthData = await _db.Orders.Where(p => p.StatusShipping == 3)
                    .Where(o => o.CreatedDate.Year == currentYear) // Lọc đơn hàng trong năm nay
                    .ToListAsync();

                // Nhóm dữ liệu theo tháng và tính tổng số tiền theo mỗi tháng
                orders = monthData
                    .GroupBy(o => o.CreatedDate.Month)
                    .Select(g => new StatisticVM
                    {
                        TotalAmount = g.Sum(o => o.TotalAmount),
                        CreatedDate = new DateTime(currentYear, g.Key, 1) // Mốc ngày đầu tháng
                    })
                    .OrderBy(o => o.CreatedDate) // Sắp xếp theo tháng
                    .ToList();

                // Đảm bảo có đầy đủ 12 tháng
                for (int i = 1; i <= 12; i++)
                {
                    if (!orders.Any(o => o.CreatedDate.Month == i))
                    {
                        orders.Add(new StatisticVM
                        {
                            CreatedDate = new DateTime(currentYear, i, 1),
                            TotalAmount = 0 // Thêm tháng chưa có dữ liệu với số tiền bằng 0
                        });
                    }
                }

                // Trả về dữ liệu dưới dạng JSON
                var data = orders.OrderBy(o => o.CreatedDate).Select(o => new {
                    Date = o.CreatedDate.ToString("yyyy-MM"),
                    TotalAmount = o.TotalAmount
                }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content($"Lỗi: {ex.Message}");
            }
        }




        [HttpGet]
        public async Task<ActionResult> Statistic_LuongDon()
        {
            try
            {
                List<StatisticVM> orders;
                int currentYear = DateTime.Now.Year; // Lấy năm hiện tại

                // Lấy dữ liệu của năm nay
                var monthData = await _db.Orders
                    .Where(o => o.CreatedDate.Year == currentYear) // Lọc đơn hàng trong năm nay
                    .ToListAsync();

                // Nhóm dữ liệu theo tháng và tính tổng số lượng đơn hàng theo mỗi tháng
                orders = monthData.Where(p => p.StatusShipping == 3)
                    .GroupBy(o => o.CreatedDate.Month)
                    .Select(g => new StatisticVM
                    {
                        // Thay vì tính tổng số tiền, ta tính tổng số lượng đơn hàng
                        TotalAmount = g.Count(), // Đếm số lượng đơn hàng trong tháng
                        CreatedDate = new DateTime(currentYear, g.Key, 1) // Mốc ngày đầu tháng
                    })
                    .OrderBy(o => o.CreatedDate) // Sắp xếp theo tháng
                    .ToList();

                // Đảm bảo có đầy đủ 12 tháng
                for (int i = 1; i <= 12; i++)
                {
                    if (!orders.Any(o => o.CreatedDate.Month == i))
                    {
                        orders.Add(new StatisticVM
                        {
                            CreatedDate = new DateTime(currentYear, i, 1),
                            TotalAmount = 0 // Thêm tháng chưa có dữ liệu với số lượng đơn bằng 0
                        });
                    }
                }

                // Trả về dữ liệu dưới dạng JSON
                var data = orders.OrderBy(o => o.CreatedDate).Select(o => new {
                    Date = o.CreatedDate.ToString("yyyy-MM"),
                    TotalAmount = o.TotalAmount // Sử dụng TotalAmount để chứa số lượng đơn hàng
                }).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content($"Lỗi: {ex.Message}");
            }
        }




        public async Task<ActionResult> SanPhamBanChay()
        {

            return View();
        }
        public async Task<ActionResult> ThongKeTheoVung()
        {

            return View();
        }
    }
}