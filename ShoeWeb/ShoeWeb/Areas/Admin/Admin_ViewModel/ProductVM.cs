using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShoeWeb.Models;

namespace ShoeWeb.Areas.Admin.Admin_ViewModel
{
    public class ProductVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<Brand> brands { get; set; }
        public IEnumerable<Size> sizes { get; set; }
        public IEnumerable<Origin> origins { get; set; }


    }
}