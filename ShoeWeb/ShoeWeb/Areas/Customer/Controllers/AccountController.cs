using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoeWeb.Areas.Customer.Controllers
{
    public class AccountController : Controller
    {
        // GET: Customer/Account
        public ActionResult UserInformation()
        {
            return View();
        }
    }
}