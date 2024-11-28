using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Customer.CustomerVM
{
    public class ConfirmOtpViewModel
    {
        [Required]
        public string OtpCode { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}