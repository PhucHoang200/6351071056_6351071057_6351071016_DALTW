using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using ShoeWeb.Identity;
using ShoeWeb.Utility;


[assembly: OwinStartup(typeof(ShoeWeb.Startup))]

namespace ShoeWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Customer/User/Index")
            });
            CreateRolesAndUser();
        }

        public void CreateRolesAndUser()
        {
            var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new AppDbContext()));

            var appDbContext = new AppDbContext();
            var appUserStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(appUserStore);

            if (!roleManager.RoleExists(SD.AdminRole))
             { 
                var role  = new IdentityRole();
                role.Name = SD.AdminRole;
                roleManager.Create(role);
            }

            if (userManager.FindByName(SD.AdminRole) == null) {
                var user = new AppUser();

                user.UserName = "admin";
                user.Email = "admin@gmail.com";
                string userPassword = "admin123";
                var chkUser = userManager.Create(user, userPassword);
                if (chkUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, SD.AdminRole);
                }

            }

            if (!roleManager.RoleExists(SD.CustomerRole))
            {
                var role = new IdentityRole();
                role.Name = SD.CustomerRole;
                roleManager.Create(role);
            }
            if (userManager.FindByName(SD.CustomerRole) == null)
            {
                var user = new AppUser();

                user.UserName = "customer1";
                user.Email = "cus@gmail.com";
                string userPassword = "customer123";
                var chkUser = userManager.Create(user, userPassword);
                if (chkUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, SD.CustomerRole);
                }

            }


        }
    }
}
