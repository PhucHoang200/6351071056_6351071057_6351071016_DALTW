using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ShoeWeb.Models.Identity;

namespace ShoeWeb.Areas.Customer.CustomerVM
{
    public class UserVM
    {
       public LoginViewModel Login { get; set; }

       public RegisterViewModel Register { get; set; }
    }
}