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
        public async Task<ActionResult> Statistic_TheoVung(string type)
        {
            try
            {
                List<StatisticVM> orders = new List<StatisticVM>();

                switch (type)
                {
                    case "Thang":
                        // Lấy tất cả dữ liệu
                        var monthData = await _db.Orders
                            .ToListAsync(); // Lấy toàn bộ dữ liệu

                        // Thực hiện nhóm và tính toán trong bộ nhớ
                        orders = monthData
                            .GroupBy(o => new { o.CreatedDate.Year, o.CreatedDate.Month })
                            .Select(g => new StatisticVM
                            {
                                TotalAmount = g.Sum(o => o.TotalAmount),
                                CreatedDate = new DateTime(g.Key.Year, g.Key.Month, 1) // Lấy ngày đầu tháng
                            })
                            .OrderBy(o => o.CreatedDate)
                            .ToList();
                        foreach (var item in orders)
                        {
                            Debug.WriteLine(item.TotalAmount);
                            Debug.WriteLine(item.CreatedDate);
                        }
                        break;

                    case "Quy":
                        // Lấy tất cả dữ liệu
                        var quarterData = await _db.Orders
                            .ToListAsync(); // Lấy toàn bộ dữ liệu

                        // Thực hiện nhóm và tính toán trong bộ nhớ
                        orders = quarterData
                            .GroupBy(o => new
                            {
                                o.CreatedDate.Year,
                                Quarter = (o.CreatedDate.Month - 1) / 3 + 1 // Tính quý
                            })
                            .Select(g => new StatisticVM
                            {
                                TotalAmount = g.Sum(o => o.TotalAmount),
                                CreatedDate = new DateTime(g.Key.Year, (g.Key.Quarter - 1) * 3 + 1, 1) // Lấy tháng đầu quý
                            })
                            .OrderBy(o => o.CreatedDate)
                            .ToList();
                        break;

                    case "Nam":
                        // Lấy tất cả dữ liệu
                        var yearData = await _db.Orders
                            .ToListAsync(); // Lấy toàn bộ dữ liệu

                        // Thực hiện nhóm và tính toán trong bộ nhớ
                        orders = yearData
                            .GroupBy(o => o.CreatedDate.Year)
                            .Select(g => new StatisticVM
                            {
                                TotalAmount = g.Sum(o => o.TotalAmount),
                                CreatedDate = new DateTime(g.Key, 1, 1) // Lấy ngày đầu năm
                            })
                            .OrderBy(o => o.CreatedDate)
                            .ToList();
                        break;

                    default:
                        return Content("Loại thống kê không hợp lệ");
                }

                // Trả về Partial View với dữ liệu
                return PartialView("Statistic_TheoVung", orders);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
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