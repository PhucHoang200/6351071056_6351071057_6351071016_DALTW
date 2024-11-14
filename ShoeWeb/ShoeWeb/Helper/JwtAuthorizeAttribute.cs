//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ShoeWeb.Helper
//{
//    public class JwtAuthorizeAttribute : AuthorizeAttribute
//    {
//        private readonly JwtAuthenticationManager _jwtAuthManager;
//        private readonly string[] _allowedRoles;

//        public JwtAuthorizeAttribute(params string[] roles)
//        {
//            string secretKey = System.Configuration.ConfigurationManager.AppSettings["JwtSecretKey"];
//            _jwtAuthManager = new JwtAuthenticationManager(secretKey);
//            _allowedRoles = roles;
//        }
//        protected override bool AuthorizeCore(HttpContextBase httpContext)
//        {
//            var authHeader = httpContext.Request.Headers["Authorization"];
//            if (authHeader != null && authHeader.StartsWith("Bearer "))
//            {
//                var token = authHeader.Substring("Bearer ".Length).Trim();
//                var principal = _jwtAuthManager.ValidateToken(token);
//                if (principal != null)
//                {
//                    httpContext.User = principal;

//                    if (_allowedRoles.Any(role => principal.IsInRole(role)))
//                    {
//                        return true; 
//                    }
//                }
//            }
//            return false;
//        }

//        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
//        {
//            filterContext.Result = new HttpStatusCodeResult(403, "Forbidden"); // Không có quyền
//        }
//    }
//}