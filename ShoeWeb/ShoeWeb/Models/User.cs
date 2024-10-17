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
        [Required]
        [StringLength(100)]
        public string name { get; set; }
        [Required]
        [StringLength(100)]
        public string email { get; set; }
        [Required]
        [StringLength(100)]
        public string password { get; set; }
        [Required]
        [StringLength(30)]
        public string phoneNumber { get; set; }
        [Required]
        public string randomKey { get; set; }
        [Required]
        public string role { get; set; }
    }
}