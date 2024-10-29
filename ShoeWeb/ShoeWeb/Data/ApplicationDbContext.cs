using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ShoeWeb.Models;
using System.Web.UI;

namespace ShoeWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            
        }
        public DbSet<Product> products { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Size> sizes { get; set; }
        public DbSet<Origin> origin { get; set; }
        public DbSet<SizeOfProduct> sizeOfProducts { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<UserRole> userRoles { get; set; }

    }
}