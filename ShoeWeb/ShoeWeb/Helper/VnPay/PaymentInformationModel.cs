﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeWeb.Helper.VnPay
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }

    }
}