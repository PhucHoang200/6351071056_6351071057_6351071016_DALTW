using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShoeWeb.Models;

namespace ShoeWeb.Areas.Customer.CustomertVM
{
    public class ProductVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }

        public IEnumerable<Size> Sizes { get; set; }

        public IEnumerable<SizeOfProduct> SizeOfProducts { get; set; }

    }
}