using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoeWeb.Models
{
    public class SizeOfProduct
    {
        [Required]
        [Key, Column(Order = 0)]
        public int productId { get; set; }
        [Required]
        [Key, Column(Order = 1)]
        public int sizeId { get; set; }
        [ForeignKey("productId")]
        public Product product { get; set; }
        [ForeignKey("sizeId")]
        public Size size { get; set; }

    }
}