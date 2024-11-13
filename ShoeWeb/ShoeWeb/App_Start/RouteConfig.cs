using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShoeWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Cấu hình route cho trang chi tiết sản phẩm
            routes.MapRoute(
                name: "ProductDetails",
                url: "product/details/{productId}",
                defaults: new { controller = "Home", action = "ProductDetails" },
                constraints: new { productId = @"\d+" } // Giới hạn productId chỉ nhận giá trị là số
            );

            routes.MapRoute(
                name: "Default",
                url: "Customer/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ShoeWeb.Areas.Customer.Controllers" }
            );

        }

    }

}
