using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.EwalletMerchant.Models
{
    public class SetActiveCommissionMerchantRequest
    {
        public int AssociateId { get; set; }
        public int MerchantId { get; set; }
    }
}
