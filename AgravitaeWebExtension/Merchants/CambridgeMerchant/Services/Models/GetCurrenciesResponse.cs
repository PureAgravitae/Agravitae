using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class GetCurrenciesResponse
    {
        public string curr { get; set; }
        public string desc { get; set; }
        public string error { get; set; }
    }
}
