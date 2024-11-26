using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ShoeWeb.Identity;

namespace ShoeWeb.Models
{
    public class OTP
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Liên kết với người dùng

        [Required]
        [StringLength(6, MinimumLength = 6)] // Mã OTP 6 ký tự
        public string OTPCode { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; } // Thời hạn OTP

        public bool IsUsed { get; set; } = false; // Đánh dấu OTP đã được sử dụng

        // Quan hệ với bảng User (nếu cần)
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }
    }
}