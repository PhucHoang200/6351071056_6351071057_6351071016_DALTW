using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShoeWeb.Models;

namespace ShoeWeb.Areas.Admin.Admin_ViewModel
{
    public class BrandVM
    {
        public IEnumerable<Brand> brands { get; set; }
    }
}