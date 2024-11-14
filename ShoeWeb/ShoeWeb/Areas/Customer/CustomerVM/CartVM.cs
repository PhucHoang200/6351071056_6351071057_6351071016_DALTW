using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Customer.CustomerVM
{
    public class CartVM
    {
        public IEnumerable<ShoppingCartItem> Products { get; set; }
        public Decimal TotalPrice { get; set; }
    }
}