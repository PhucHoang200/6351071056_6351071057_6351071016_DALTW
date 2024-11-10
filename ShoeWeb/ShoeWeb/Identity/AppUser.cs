using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ShoeWeb.Identity
{
    public class AppUser : IdentityUser
    {
        public DateTime? BirthDay { get; set; }
    }
}