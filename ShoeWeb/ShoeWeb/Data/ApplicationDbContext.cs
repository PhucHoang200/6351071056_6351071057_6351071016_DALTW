using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ShoeWeb.Models;
using System.Web.UI;
using Microsoft.AspNet.Identity.EntityFramework;
using ShoeWeb.Identity;
using ShoeWeb.Models.Identity;

namespace ShoeWeb.Data
{
    public class ApplicationDbContext   : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Product> products { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<Size> sizes { get; set; }
        public DbSet<Origin> origin { get; set; }
        public DbSet<SizeOfProduct> sizeOfProducts { get; set; }
        
        public DbSet<ShoppingCart> shoppingCarts {  get; set; }
        public DbSet<ShoppingCartItem> shoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OTP> oTPs { get; set; }
        public DbSet<IdentityUserRole> UserRoles { get; set; } // DbSet cho AspNetUserRoles
    }
}