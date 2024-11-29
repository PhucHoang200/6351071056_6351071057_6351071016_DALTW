using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Areas.Admin.Admin_ViewModel;
using ShoeWeb.Data;
using ShoeWeb.Models;
using ShoeWeb.Utility;

namespace ShoeWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminRole)]

    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        public OrderController() : this(new ApplicationDbContext())
        {

        }

        public async Task<OrderVM> GetOrderVM()
        {
            var orderVM = new OrderVM()
            {
                order = await _db.Orders.ToListAsync(),
                orderDetail = await _db.OrderDetails.ToListAsync(),
            };

            return orderVM;
        }
        // GET: Admin/Order
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var orderVM = await GetOrderVM();
            return View(orderVM);
        }

        [HttpGet]
        public async Task<ActionResult> WaitingOrder()
        {
            var orderVM = await GetOrderVM();

            return View(orderVM);
        }

        public async Task<ActionResult> TatCa()
        {
            var orderVM = await GetOrderVM();
            
            return PartialView("TatCa", orderVM);
        }
        [HttpGet]
        public async Task<ActionResult> ChoXacNhan()
        {
            var orderVM = await GetOrderVM();
            orderVM.order = orderVM.order.Where(o => o.isAccept == false).ToList();
            return PartialView("ChoXacNhan", orderVM);
        }

        [HttpPost]
        public async Task<ActionResult> DuyetDon(int orderId)
        {
            try
            {
                var order = await _db.Orders.FindAsync(orderId);
                if(order == null)
                {
                    return Json(new { success = false , Message = "Không tìm thấy đơn hàng"});
                }

                order.isAccept = true;
                await _db.SaveChangesAsync();

                return Json(new { success = true, Message = "Duyệt đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = ex.Message });

            }
        }

        [HttpPost]
        public async Task<ActionResult> HuyDon(int orderId)
        {

            try
            {
                var order = await _db.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return Json(new { success = false, Message = "Không tìm thấy đơn hàng" });
                }

                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();

                return Json(new { success = true, Message = "Hủy đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = ex.Message });

            }
        }
        
     


        public async Task<ActionResult> ChoLayHang()
        {
            var orderVM = await GetOrderVM();
            orderVM.order = orderVM.order.Where(o => o.StatusShipping == 0 && o.isAccept == true).ToList();
            return PartialView("ChoLayHang", orderVM);
        }

        [HttpPost]
        public async Task<ActionResult> GiaoHang(int orderId)
        {

            try
            {
                var order = await _db.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return Json(new { success = false, Message = "Không tìm thấy đơn hàng" });
                }

                order.StatusShipping = 2;
                await _db.SaveChangesAsync();

                return Json(new { success = true, Message = "Bàn giao cho đơn vị vận chuyển thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = ex.Message });

            }
        }

        public async Task<ActionResult> DangGiao()
        {
            var orderVM = await GetOrderVM();
            orderVM.order = orderVM.order.Where(o => o.StatusShipping == 2 && o.isAccept == true).ToList();

            return PartialView("DangGiao",orderVM);
        }

        public async Task<ActionResult> DaGiao()
        {
            var orderVM = await GetOrderVM();
            orderVM.order = orderVM.order.Where(o => o.StatusShipping == 3 && o.isAccept == true).ToList();
            return PartialView("DaGiao", orderVM);
        }


        public async Task<ActionResult> DetailOrder(int orderId)
        {
            try
            {
                var item = await _db.Orders.FindAsync(orderId);
                var orderItem = await _db.OrderDetails.Where(o => o.OrderId == orderId)
                    .Include(o => o.Product)
                    .ToListAsync();
                var orderVM = new OrderVM
                {
                    Order = item,
                    orderDetail = orderItem,
                    order = new List<Order>()
                };
                return View(orderVM);
            }
            catch (Exception ex) {
                return View("Error");

            }

        }

    }
}