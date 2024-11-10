using System;
using System.Web.Mvc;
using ShoeWeb.Utility;

namespace ShoeWeb.Helper
{
    public class AdminAutho : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    filterContext.Result = new RedirectToRouteResult(
            //        new System.Web.Routing.RouteValueDictionary
            //        {
            //            { "controller", "Account" },
            //            { "action", "Login" }
            //        }
            //    );
            //    return;
            //}

            if (filterContext.HttpContext.User.IsInRole(SD.AdminRole) == false)
            {
                filterContext.Result = new  HttpUnauthorizedResult();
            }
        }

    }
}
