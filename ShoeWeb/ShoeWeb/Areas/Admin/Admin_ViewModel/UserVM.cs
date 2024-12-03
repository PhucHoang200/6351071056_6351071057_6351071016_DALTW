using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoeWeb.Areas.Admin.Admin_ViewModel
{
    public class UserVM
    {
        public string Id { get; set; } // ID người dùng
        public string Email { get; set; } // Email người dùng
        public string PhoneNumber { get; set; } // Số điện thoại người dùng
        public string UserName { get; set; } // Tên tài khoản
        public bool Status { get; set; } // Trạng thái (true: đã vô hiệu hóa, false: chưa vô hiệu hóa)
        public string RoleName { get; set; } // Tên vai trò hiện tại
        public List<string> AvailableRoles { get; set; } // Danh sách các vai trò để chọn trong combobox

        // Thuộc tính hiển thị trạng thái
        public string DisplayStatus => Status ? "Đã vô hiệu hóa" : "Chưa vô hiệu hóa";

        public class UserListVM
        {
            public IEnumerable<UserVM> Users { get; set; }
        }
    }
}