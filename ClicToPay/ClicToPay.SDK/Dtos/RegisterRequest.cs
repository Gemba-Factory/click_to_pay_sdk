using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClicToPay.SDK.Dtos
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string OrderNumber { get; set; }
        public long Amount { get; set; }
        public int Currency { get; set; }
        public string ReturnUrl { get; set; }
        public string FailUrl { get; set; }
        public string Description { get; set; }
        public string Language { get; set; } = "en";
        public string PageView { get; set; } = "DESKTOP";
        public Dictionary<string, string> JsonParams { get; set; }
        public string ExpirationDate { get; set; } // ISO format
    }
}
