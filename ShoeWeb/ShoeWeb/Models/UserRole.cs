using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoeWeb.Models
{
    public class UserRole
    {
        [Required]
        [Key, Column(Order = 0)]
        public int IdRole { get; set; }
        [Required]
        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("IdRole")]

        public Role Role { get; set; }
    }
}