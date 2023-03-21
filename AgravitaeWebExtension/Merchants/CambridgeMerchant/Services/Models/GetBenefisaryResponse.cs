using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class GetBenefisaryResponse
    {
        public string beneficiaryID { get; set; }
        public string beneficiaryEmailID { get; set; }
        public string orderPaymentNumber { get; set; }
    }
}
