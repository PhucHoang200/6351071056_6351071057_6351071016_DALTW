using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoeWeb.Models
{
    
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userId { get; set; }
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100, ErrorMessage = "User name cannot exceed 100 characters.")]
        [Index(IsUnique = true)]
        public string userName { get; set; }
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string name { get; set; }
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string email { get; set; }
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        public string password { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(30, ErrorMessage = "User name cannot exceed 30 characters.")]
        public string phoneNumber { get; set; }
        [Required]
        public string randomKey { get; set; }
        
    }
}