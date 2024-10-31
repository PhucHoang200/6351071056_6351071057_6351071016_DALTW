using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeWeb.Helper;
using ShoeWeb.Utility;

namespace ShoeWeb.Areas.Admin.Controllers
{
   
    [JwtAuthorize(SD.AdminRole)]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}