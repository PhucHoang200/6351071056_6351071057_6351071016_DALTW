using ShoeWeb.Helper;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ShoeWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Di chuyển việc khởi tạo secret key vào Application_Start
            string secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            JwtAuthManagerProvider.Initialize(secretKey);
        }

        // Sự kiện này sẽ được gọi khi mỗi yêu cầu HTTP bắt đầu
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Kiểm tra nếu URL là trang chủ và chuyển hướng đến /Customer/Home
            if (HttpContext.Current.Request.Url.AbsolutePath == "/")
            {
                HttpContext.Current.Response.Redirect("/Customer/Home");
            }
        }
    }
}
