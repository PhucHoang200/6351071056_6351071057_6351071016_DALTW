using ShoeWeb.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeWeb.Helper.Payment.Response
{
    public class CreateZaloPayResponse
    {
        public int returnCode {  get; set; }
        public string returnMessage { get; set; } = string.Empty;
        public string orderUrl { get; set; } = string.Empty ;
       
    }
}
