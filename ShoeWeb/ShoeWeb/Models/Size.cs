using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoeWeb.Models
{
    public class Size
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sizeId { get; set; }
        [Required]
        [Range(0.01, float.MaxValue, ErrorMessage = "Size phải lớn hơn 0")]
        public float numberSize { get; set; }
    }
}