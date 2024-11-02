using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShoeWeb.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int productId { get; set; }

        [Required]
        [StringLength(100)]
        public string productName { get; set; }

        [Required]
        public string productDescription { get; set; }

        [Required]
        public decimal price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int quantity { get; set; }

        [Required]
        public string image { get; set; }

        [Required]
        public DateTime? createdDate { get; set; }

        [Required]
        public DateTime? updatedDate { get; set; }

        [Required]
        public int cateId { get; set; }

        [Required]
        public int brandId { get; set; }

        [ForeignKey("cateId")]
        public Category Category { get; set; }

        [ForeignKey("brandId")]
        public Brand Brand { get; set; }

        public int idOrigin { get; set; }

        [ForeignKey("idOrigin")]
        public Origin Origin { get; set; }
    }
}