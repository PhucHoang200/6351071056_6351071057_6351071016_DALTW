using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using ShoeWeb.App_Start;
using ShoeWeb.Data;
using ShoeWeb.Identity;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Services.Description;



[assembly: OwinStartup(typeof(ShoeWeb.Startup))]

namespace ShoeWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();

            
        }

        

        public void CreateRolesAndUsers()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<AppUser>(new UserStore<AppUser>(new ApplicationDbContext()));

            // Tạo vai trò Admin nếu chưa có
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole("Admin");
                roleManager.Create(role);
            }

            // Tạo vai trò Customer nếu chưa có
            if (!roleManager.RoleExists("Customer"))
            {
                var role = new IdentityRole("Customer");
                roleManager.Create(role);
            }
        }
        public void ConfigureAuth(IAppBuilder app)
        {
            // Cấu hình xác thực cho ứng dụng
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Đảm bảo rằng các token providers đã được cấu hình trong UserManager
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Customer/User/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, AppUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<AppUser>(context.Get<ApplicationDbContext>()));

            // Đảm bảo bạn đăng ký các token providers
            manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(options.DataProtectionProvider.Create("ASP.NET Identity"));

            // Cấu hình các tính năng khác cho UserManager (ví dụ: password, validation, etc.)
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            manager.EmailService = new EmailService(); // Nếu có dịch vụ gửi email

            return manager;
        }

    }
}
