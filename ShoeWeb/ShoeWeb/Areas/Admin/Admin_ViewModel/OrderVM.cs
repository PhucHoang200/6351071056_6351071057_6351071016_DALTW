using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Admin.Admin_ViewModel
{
    public class OrderVM
    {
        public IEnumerable<Order> order {  get; set; }
        public Order Order { get; set; }
        public IEnumerable<OrderDetail> orderDetail { get; set; }
    }
}