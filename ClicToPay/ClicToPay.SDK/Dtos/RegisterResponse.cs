using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClicToPay.SDK.Dtos
{
    public class RegisterResponse
    {
        public string OrderId { get; set; }
        public string FormUrl { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
