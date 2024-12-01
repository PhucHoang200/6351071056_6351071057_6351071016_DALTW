using EllipticCurve.Utils;
using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Customer.CustomerVM
{
    public class UserProfileViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public IEnumerable<OrderDetail> orderItem_success { get; set; }
        public IEnumerable<OrderDetail> orderItem_waitingAccept { get; set; }
        public IEnumerable<OrderDetail> orderItem_shipping { get; set; }
    }
}