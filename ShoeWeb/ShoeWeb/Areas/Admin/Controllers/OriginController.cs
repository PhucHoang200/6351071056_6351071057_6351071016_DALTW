using Antlr.Runtime.Tree;
using ShoeWeb.Areas.Admin.Admin_ViewModel;
using ShoeWeb.Data;
using ShoeWeb.Helper;
using ShoeWeb.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ShoeWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class OriginController : Controller
    {


        private readonly ApplicationDbContext _db;
        public OriginController(ApplicationDbContext db)
        {
            _db = db;
        }
        public OriginController() : this(new ApplicationDbContext())
        {

        }

        public async Task<OriginVM> GetOriginVM()
        {
            return new OriginVM()
            {
                origins = await _db.origin.ToListAsync()
            };
        }

        // GET: Admin/Origin
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var countries = new List<string>
            {
                "Afghanistan", "Albania", "Algeria", "Andorra", "Angola",
                "Argentina", "Australia", "Austria", "Azerbaijan", "Bahamas",
                "Bangladesh", "Barbados", "Belarus", "Belgium", "Bhutan",
                "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil",
                "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia",
                "Canada", "Chile", "China", "Colombia", "Costa Rica",
                "Croatia", "Cuba", "Cyprus", "Czech Republic", "Denmark",
                "Dominica", "Dominican Republic", "Ecuador", "Egypt",
                "El Salvador", "Estonia", "Ethiopia", "Fiji", "Finland",
                "France", "Germany", "Ghana", "Greece", "Guatemala",
                "Honduras", "Hungary", "Iceland", "India", "Indonesia",
                "Iran", "Iraq", "Ireland", "Israel", "Italy",
                "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya",
                "Kuwait", "Kyrgyzstan", "Laos", "Latvia", "Lebanon",
                "Lesotho", "Liberia", "Libya", "Lithuania", "Luxembourg",
                "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali",
                "Malta", "Mauritius", "Mexico", "Moldova", "Monaco",
                "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar",
                "Namibia", "Nepal", "Netherlands", "New Zealand", "Nicaragua",
                "Nigeria", "North Korea", "Norway", "Oman", "Pakistan",
                "Panama", "Paraguay", "Peru", "Philippines", "Poland",
                "Portugal", "Qatar", "Romania", "Russia", "Rwanda",
                "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Singapore",
                "Slovakia", "Slovenia", "Somalia", "South Africa", "South Korea",
                "Spain", "Sri Lanka", "Sudan", "Sweden", "Switzerland",
                "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand",
                "Togo", "Trinidad and Tobago", "Tunisia", "Turkey", "Uganda",
                "Ukraine", "United Arab Emirates", "United Kingdom",
                "United States", "Uruguay", "Uzbekistan", "Vatican City",
                "Venezuela", "Vietnam", "Yemen", "Zambia", "Zimbabwe"
            };

            ViewData["Countries"] = countries;

            var origins = await GetOriginVM();
            return View(origins);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateOrigin(int id, string country)
        {

            try
            {
                var origin = await _db.origin.FindAsync(id);
                origin.nameCountry = country;

                await _db.SaveChangesAsync();

                var originVM = await GetOriginVM();
                return Json(new { success = true, origins = originVM.origins });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

            }
        }

        [HttpPost]
        public async Task<ActionResult> AddOrigin(string nameCountry)
        {
            if (string.IsNullOrWhiteSpace(nameCountry))
            {
                return Json(new { success = false, message = "Tên quốc gia không hợp lệ." });
            }

            try
            {
                // Kiểm tra sự tồn tại của tên quốc gia
                var existingOrigin = await _db.origin.FirstOrDefaultAsync(o => o.nameCountry == nameCountry);
                if (existingOrigin != null)
                {
                    return Json(new { success = false, message = "Tên quốc gia đã tồn tại!" });
                }

                // Nếu không tồn tại, thêm mới
                var newOrigin = new Models.Origin { nameCountry = nameCountry };
                _db.origin.Add(newOrigin);
                await _db.SaveChangesAsync();

                // Lấy danh sách Origin mới nhất
                var origins = await _db.origin.ToListAsync();

                return Json(new { success = true, origins = origins });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteOrigin(int id)
        {
            try
            {
                var origin = await _db.origin.FindAsync(id);
                if (origin == null)
                {
                    return Json(new { success = false });
                }

                var isProduct = await _db.products.Where(p => p.idOrigin == id).FirstOrDefaultAsync();
                if (isProduct != null)
                {
                    return Json(new { success = false });
                }
                _db.origin.Remove(origin);
                await _db.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

    }
}