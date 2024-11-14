using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Customer.CustomerVM
{
    public class DetailProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SizeOfProduct> sizeOfProduct { get; set; }
        public IEnumerable<Product> products { get; set; }
        public IEnumerable<Size> sizes { get; set; }
    }
}