//using System;
//using System.Web.Mvc;
//using ShoeWeb.Utility;

//namespace ShoeWeb.Helper
//{
//    public class AdminAutho : AuthorizeAttribute
//    {
//        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
//        {
//            if (filterContext.HttpContext.User.Identity.IsAuthenticated && !filterContext.HttpContext.User.IsInRole(SD.AdminRole))
//            {
//                // Nếu đã đăng nhập nhưng không phải Admin, chuyển hướng về Home
//                filterContext.Result = new RedirectResult("~/Home/Index");
//            }
//            else
//            {
//                base.HandleUnauthorizedRequest(filterContext);
//            }
//        }

//    }
//}
