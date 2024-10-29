using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Customer.CustomertVM
{
    public class HomeVM
    {
        public IEnumerable<IEnumerable<Product>> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
    }
}