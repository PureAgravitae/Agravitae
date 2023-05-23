using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Merchants.CambridgeMerchant.Models
{
    public class TokenModel
    {
        public int Issuedat { get; set; }
        public int Expiration { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
    }
}
