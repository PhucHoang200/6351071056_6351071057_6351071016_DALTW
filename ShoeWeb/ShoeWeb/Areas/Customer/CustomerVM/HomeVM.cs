using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Customer.CustomertVM
{
    public class HomeVM
    {
        public List<List<Product>> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Brand> Brands { get; set; }
    }
}