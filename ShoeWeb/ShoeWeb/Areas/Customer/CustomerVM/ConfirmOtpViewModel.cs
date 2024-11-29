using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Customer.CustomerVM
{
    public class ConfirmOtpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã OTP.")]
        public string OtpCode { get; set; }
    }
}