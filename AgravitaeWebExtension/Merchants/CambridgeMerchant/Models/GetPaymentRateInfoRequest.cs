using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Merchants.CambridgeMerchant.Models
{
    public class GetPaymentRateInfoRequest
    {
        public int AssociateID { get; set; }
        public double Amount { get; set; }
    }
}
