using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoeWeb.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cateId { get; set; }
        [Required]
        [StringLength(100)]
        public string cateName { get; set; }
        [Required]
        public string cateDescription { get; set; }

    }
}