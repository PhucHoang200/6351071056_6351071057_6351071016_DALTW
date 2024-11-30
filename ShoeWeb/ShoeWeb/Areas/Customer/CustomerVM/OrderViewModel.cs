using ShoeWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Customer.CustomerVM
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Tên là bắt buộc!")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc!")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc!")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn một Tỉnh/Thành phố hợp lệ.")]
        public string Province { get; set; }  // ID của Tỉnh

        public string ProvinceText { get; set; }  // Text của Tỉnh

        [Required(ErrorMessage = "Vui lòng chọn một Quận/Huyện hợp lệ.")]
        public string District { get; set; }  // ID của Quận

        public string DistrictText { get; set; }  // Text của Quận

        [Required(ErrorMessage = "Vui lòng chọn một Phường/Xã hợp lệ.")]
        public string Ward { get; set; }  // ID của Phường

        public string WardText { get; set; }  // Text của Phường

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public string PaymentMethod { get; set; }
    }

}